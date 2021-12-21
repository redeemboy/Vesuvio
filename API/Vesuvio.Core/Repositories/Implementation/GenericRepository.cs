using Vesuvio.DatabaseMigration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Vesuvio.Core
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        private readonly AppDatabaseContext _dbContext;

        public GenericRepository(AppDatabaseContext dbContext)
        {
            this._dbContext = dbContext ?? throw new ArgumentNullException("dbContext was not supplied");
            _dbSet = _dbContext.Set<T>();
        }

        public IQueryable<T> GetQuerable()
        {
            return _dbSet.AsQueryable();
        }

        public IQueryable<T> GetQuerable(Expression<Func<T, bool>> criteria)
        {
            return _dbSet.Where(criteria).AsQueryable();
        }

        public T GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public T GetSingle(Expression<Func<T, bool>> criteria)
        {
            return this.GetQuerable().SingleOrDefault<T>(criteria);
        }

        public T GetFirst(Expression<Func<T, bool>> criteria)
        {
            return this.GetQuerable().FirstOrDefault<T>(criteria);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.AsEnumerable();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> criteria)
        {
            return this.GetQuerable().Where(criteria);
        }

        public IEnumerable<T> Get(Expression<Func<T, object>> orderBy, SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Ascending)
            {
                return this.GetQuerable().OrderBy(orderBy).AsEnumerable();
            }
            else
            {
                return this.GetQuerable().OrderByDescending(orderBy).AsEnumerable();
            }
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy, SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Ascending)
            {
                return this.GetQuerable().Where(criteria).OrderBy(orderBy).AsEnumerable();
            }
            else
            {
                return this.GetQuerable().Where(criteria).OrderByDescending(orderBy).AsEnumerable();
            }
        }

        public IEnumerable<T> Get(Expression<Func<T, object>> orderBy, SortOrder sortOrder, int pageIndex, int pageSize)
        {
            if (sortOrder == SortOrder.Ascending)
            {
                return this.GetQuerable().OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
            }
            else
            {
                return this.GetQuerable().OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
            }
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy, SortOrder sortOrder, int pageIndex, int pageSize)
        {
            if (sortOrder == SortOrder.Ascending)
            {
                return this.GetQuerable().Where(criteria).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
            }
            else
            {
                return this.GetQuerable().Where(criteria).OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
            }
        }


        // multiple sorting
        //public IEnumerable<T> Get(Expression<Func<T, bool>> criteria, Expression<Func<T, object>>[] orders, SortOrder sortOrder, int pageIndex, int pageSize)
        public IQueryable<T> Get(Expression<Func<T, bool>> criteria, Dictionary<Expression<Func<T, object>>, SortOrder> orders, int pageIndex, int pageSize)
        {

            var query = this.GetQuerable();

            if (criteria != null)
            {
                query = query.Where(criteria);
            }

            if (orders.Count > 0)
            {
                if (orders.First().Value == SortOrder.Ascending)
                {
                    query = query.OrderBy(orders.First().Key);
                }
                else
                {
                    query = query.OrderByDescending(orders.First().Key);
                }

                foreach (var order in orders.Skip(1))
                {
                    if (order.Value == SortOrder.Ascending)
                    {
                        query = ((IOrderedQueryable<T>)query).ThenBy(order.Key);
                    }
                    else
                    {
                        query = ((IOrderedQueryable<T>)query).ThenByDescending(order.Key);
                    }
                }
            }

            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);//.AsEnumerable();
        }

        public IEnumerable<T> GetWithRawSql(string query, params object[] parameters)
        {

            return _dbSet.FromSqlRaw(query, parameters).AsEnumerable();
        }

        public int Count()
        {
            return this.GetQuerable().Count();
        }

        public int Count(Expression<Func<T, bool>> criteria)
        {
            return this.GetQuerable().Count(criteria);
        }

        public void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity was not supplied");
            }

            _dbSet.Add(entity);
        }

        public void Add(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity was not supplied");
            }

            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity was not supplied");
            }

            _dbSet.Remove(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        /*
        public IQueryable<T> GetAll(
             Expression<Func<T, bool>> filter = null,
             Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
             List<Expression<Func<T, object>>> includes = null,
             int? page = null,
             int? pageSize = null)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return query;
        }

        public async Task<IEnumerable<T>> GetAllAsync(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           List<Expression<Func<T, object>>> includes = null,
           int? page = null,
           int? pageSize = null)
        {
            return await GetAll(filter, orderBy, includes, page, pageSize).ToListAsync();
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate);

        public void Insert(T entity)
        {
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Added;
        }

        public void InsertRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(object id)
        {
            T entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(Guid userId, List<Guid> roleId)
        {
            foreach (var item in roleId)
            {
                T entityToDelete = _dbSet.Find(userId, item);
                Delete(entityToDelete);
            }
        }

        public void DeleteAsync(object id)
        {
            ValueTask<T> entityToDelete = _dbSet.FindAsync(id);
            Delete(entityToDelete);
        }

        public void Delete(T entity)
        {
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Deleted;
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public IQueryable<T> RawSql(string query, params object[] parameters)
        {
            return _dbSet.FromSqlRaw(query, parameters).AsQueryable();
        }
        */
    }
}