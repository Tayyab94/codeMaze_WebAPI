
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly RepositoryContext context;

        public RepositoryBase(RepositoryContext context)
        {
            this.context = context;
        }
        public void Create(T entity)
        {
            this.context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            this.context.Set<T>().Remove(entity);
        }

        public IQueryable<T> FindAll()
        {
            return this.context.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.context.Set<T>().Where(expression).AsNoTracking(); 
        }

        public void Update(T entity)
        {
            this.context.Set<T>().Update(entity);
        }


        public IQueryable<T> Filter(Expression<Func<T, bool>> filter,
               int skip = 0, int take = int.MaxValue,
               Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
               Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            var _resetSet = filter != null ? context.Set<T>().AsNoTracking().Where(filter).AsQueryable() : context.Set<T>().AsNoTracking().AsQueryable();

            if (include != null)
            {
                _resetSet = include(_resetSet);
            }
            if (orderBy != null)
            {
                _resetSet = orderBy(_resetSet).AsNoTracking();
            }
            _resetSet = skip == 0 ? _resetSet.Take(take) : _resetSet.Skip(skip).Take(take);

            return _resetSet.AsNoTracking();
        }

        #region Alternate query

        //public virtual IQueryable<TObject> Filter(Expression<Func<TObject, bool>> filter,
        //                                      int skip = 0,
        //                                      int take = int.MaxValue,
        //                                      Func<IQueryable<TObject>, IOrderedQueryable<TObject>> orderBy = null,
        //                                      IList<string> incudes = null)
        //{
        //    var _resetSet = filter != null ? DbSet.AsNoTracking().Where(filter).AsQueryable() : DbSet.AsNoTracking().AsQueryable();

        //    if (incudes != null)
        //    {
        //        foreach (var incude in incudes)
        //        {
        //            _resetSet = _resetSet.Include(incude);
        //        }
        //    }
        //    if (orderBy != null)
        //    {
        //        _resetSet = orderBy(_resetSet).AsQueryable();
        //    }
        //    _resetSet = skip == 0 ? _resetSet.Take(take) : _resetSet.Skip(skip).Take(take);

        //    return _resetSet.AsQueryable();
        //}


        #endregion


        public async Task<IEnumerable<T>> GetAsync(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "",
           int first = 0, int offset = 0)
        {
            IQueryable<T> query = context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (offset > 0)
            {
                query = query.Skip(offset);
            }
            if (first > 0)
            {
                query = query.Take(first);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        // this is how, we can call the above method
        //YourRepository.GetAsync(filter: w => w.OtherKeyNavigation.Id == 123456, includeProperties: "OtherKeyNavigation", first: 20, offset: 0);



        public async Task<T> GetEntity(Expression<Func<T, bool>> where)
        {
            var data = context.Set<T>().Where(where).AsNoTracking();

            return await data.FirstOrDefaultAsync();
        }
    }
}
