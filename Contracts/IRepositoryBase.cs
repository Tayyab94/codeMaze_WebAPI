using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Contracts
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);


        Task<T> GetEntity(Expression<Func<T, bool>> where);


        IQueryable<T> Filter(Expression<Func<T, bool>> filter,
                                        int skip = 0,
                                        int take = int.MaxValue,
                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);


        Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int first = 0, int offset = 0);

    }
}