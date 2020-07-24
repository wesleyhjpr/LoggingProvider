using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace loggingProvider.Interface
{
    public interface IRepositorioBase<TEntidade> : IDisposable where TEntidade : class
    {
        Task<IEnumerable<TEntidade>> ObterTodos(params Expression<Func<TEntidade, object>>[] colunas);

        Task<TEntidade> ObterPorId(Guid id, params Expression<Func<TEntidade, object>>[] colunas);
        Task<TEntidade> ObterPorId(TEntidade obj, params Expression<Func<TEntidade, object>>[] colunas);

        Task<int> Adicionar(TEntidade obj);

        Task<int> Atualizar(TEntidade obj, params Expression<Func<TEntidade, object>>[] colunas);

        Task<int> Remover(TEntidade obj);
        Task<int> Remover(Guid id);
    }
}
