using System;
using System.Linq.Expressions;

namespace loggingProvider.Interface
{
    public interface ICriarQuery<TEntidade> where TEntidade : class
    {
        string PK { get; }
        string ObterTodos(params Expression<Func<TEntidade, object>>[] colunas);
        string ObterPorId(params Expression<Func<TEntidade, object>>[] colunas);
        string Adicionar();
        string Atualizar(params Expression<Func<TEntidade, object>>[] colunas);
        string Remover();
    }
}
