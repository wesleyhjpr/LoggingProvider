using loggingProvider.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace loggingProvider.Dados
{
    public class CriarQuery<TEntidade> : ICriarQuery<TEntidade> where TEntidade : class
    {
        private IDictionary<string, string> _colunas;
        private string _nomeTabela;
        public string PK { get; private set; }

        public CriarQuery()
        {
            _colunas = new Dictionary<string, string>();
            DefinirColunas();
        }

        public void DefinirColunas()
        {
            var tipoEntidade = typeof(TEntidade);

            _nomeTabela = tipoEntidade.GetCustomAttribute<TableAttribute>()?.Name ?? tipoEntidade.Name;

            var propriedades = tipoEntidade.GetProperties();
            // limpar dicionario para que a fn seja chamada mais de uma vez
            _colunas.Clear();
            //definir nome da PK
            PK = propriedades.Where(x => x.GetCustomAttribute<KeyAttribute>() != null).FirstOrDefault()?.Name;            

            foreach (var item in propriedades)
            {
                if (item.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;

                var colunaAtributo = item.GetCustomAttribute<ColumnAttribute>();
                if (colunaAtributo != null)
                    _colunas.Add(colunaAtributo.Name, item.Name);
                else
                    _colunas.Add(item.Name, item.Name);
            }
        }

        public string ObterTodos(params Expression<Func<TEntidade, object>>[] colunaExpre)
        {
            var queryBuilder = new StringBuilder();

            var colunas = _colunas;
            if (colunaExpre.Count() > 0)
                colunas = CriarColunas(colunaExpre);

            queryBuilder.AppendLine("SELECT ");
            foreach (var item in colunas)
            {
                if (colunas.First().Value == item.Value)
                    queryBuilder.AppendLine($"{item.Key} AS {item.Value}");
                else
                    queryBuilder.AppendLine($",{item.Key} AS {item.Value}");
            }
            queryBuilder.AppendLine($" FROM {_nomeTabela}");

            return queryBuilder.ToString();
        }

        public string ObterPorId(params Expression<Func<TEntidade, object>>[] colunaExpre)
        {
            VerificarNomePK();

            var queryBuilder = new StringBuilder();

            queryBuilder.Append(ObterTodos(colunaExpre));
            queryBuilder.AppendLine($" WHERE {_colunas.First(x => x.Value == PK).Key} = @{PK}");
            return queryBuilder.ToString();
        }

        public string Adicionar()
        {
            var queryBuilder = new StringBuilder();

            queryBuilder.Append($@"INSERT INTO {_nomeTabela} ( ");
            foreach (var item in _colunas)
            {
                queryBuilder.Append(item.Value == item.Key ? item.Value : item.Key);
                if (_colunas.Last().Value != item.Value)
                    queryBuilder.Append(",");
            }

            queryBuilder.Append(")");
            queryBuilder.AppendLine(" VALUES ( ");
            foreach (var item in _colunas)
            {
                queryBuilder.Append($"@{item.Value}");
                if (_colunas.Last().Value != item.Value)
                    queryBuilder.Append(",");
            }

            queryBuilder.Append(")");
            return queryBuilder.ToString();
        }

        public string Atualizar(params Expression<Func<TEntidade, object>>[] colunaExpre)
        {
            VerificarNomePK();

            var queryBuilder = new StringBuilder();

            //Remove a chave primária do comando update
            var colunas = _colunas.Where(p => p.Key != PK).ToDictionary(p => p.Key, p => p.Value);

            //Define somente a coluna que será atualizada
            if (colunaExpre.Count() > 0)
                colunas = CriarColunas(colunaExpre).ToDictionary(p => p.Key, p => p.Value);

            //Constrói a string sql
            queryBuilder.Append($" UPDATE {_nomeTabela} SET ");
            foreach (var item in colunas)
            {
                if (colunas.First().Value == item.Value)
                    queryBuilder.AppendLine($"{item.Key} = @{item.Value}");
                else
                    queryBuilder.AppendLine($",{item.Key} = @{item.Value}");
            }
            queryBuilder.Append($" WHERE {_colunas.First(x => x.Value == PK).Key} = @{PK}");

            return queryBuilder.ToString();
        }

        public string Remover()
        {
            VerificarNomePK();
            var queryBuilder = new StringBuilder();

            queryBuilder.Append($"DELETE FROM {_nomeTabela} WHERE {_colunas.First(x => x.Value == PK).Key} = @{PK}");
            return queryBuilder.ToString();
        }        

        private IDictionary<string, string> CriarColunas<T>(params Expression<Func<T, object>>[] ex)
        {
            var colunas = new Dictionary<string, string>();

            foreach (var item in ex)
            {                
                var expression = item.Body is MemberExpression ?
                                (MemberExpression)item.Body :
                                (MemberExpression)((UnaryExpression)item.Body).Operand;

                if (expression.Member.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;

                var coluna = expression.Member.GetCustomAttribute<ColumnAttribute>();
                if (coluna != null)
                    colunas.Add(coluna.Name, expression.Member.Name);
                else
                    colunas.Add(expression.Member.Name, expression.Member.Name);
            }

            return colunas;
        }

        private void VerificarNomePK()
        {
            _ = PK ?? throw new Exception($"Atributo [Key] não definido na entidade: {typeof(TEntidade).FullName}");
        }
    }
}
