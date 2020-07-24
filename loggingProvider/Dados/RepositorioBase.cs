using Dapper;
using loggingProvider.Entidades;
using loggingProvider.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace loggingProvider.Dados
{
    public class RepositorioBase<TEntidade> : IRepositorioBase<TEntidade> where TEntidade : Entidade, new()
    {
        protected readonly ICriarQuery<TEntidade> _criarQuery;

        //protected readonly IDbConnection _conexao;
        readonly string Connection = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=logging;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public RepositorioBase(ICriarQuery<TEntidade> criarQuery)
        {
            _criarQuery = criarQuery;
        }
        public async Task<IEnumerable<TEntidade>> ObterTodos(params Expression<Func<TEntidade, object>>[] colunas)
        {
            using IDbConnection _conexao = new SqlConnection(Connection);
            return await _conexao.QueryAsync<TEntidade>(_criarQuery.ObterTodos(colunas));
        }
        public async Task<TEntidade> ObterPorId(Guid id, params Expression<Func<TEntidade, object>>[] colunas)
        {
            return await ObterPorId(new TEntidade { Id = id }, colunas);
        }
        public async Task<TEntidade> ObterPorId(TEntidade obj, params Expression<Func<TEntidade, object>>[] colunas)
        {
            using IDbConnection _conexao = new SqlConnection(Connection);

            var rs = await _conexao.QueryAsync<TEntidade>(_criarQuery.ObterPorId(colunas), obj);
            return rs.FirstOrDefault();
        }
        public async Task<int> Adicionar(TEntidade obj)
        {
            using IDbConnection _conexao = new SqlConnection(Connection);
            return await _conexao.ExecuteAsync(_criarQuery.Adicionar(), obj);
        }
        public async Task<int> Atualizar(TEntidade obj, params Expression<Func<TEntidade, object>>[] colunas)
        {
            using IDbConnection _conexao = new SqlConnection(Connection);
            return await _conexao.ExecuteAsync(_criarQuery.Atualizar(colunas), obj);
        }
        public async Task<int> Remover(Guid id)
        {
            return await Remover(new TEntidade { Id = id });
        }
        public async Task<int> Remover(TEntidade obj)
        {
            using IDbConnection _conexao = new SqlConnection(Connection);
            return await _conexao.ExecuteAsync(_criarQuery.Remover(), obj);
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
