using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;

namespace Vesuvio.Core
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQuerable();

        IQueryable<T> GetQuerable(Expression<Func<T, bool>> criteria);

        T GetById(object id);

        T GetSingle(Expression<Func<T, bool>> criteria);

        T GetFirst(Expression<Func<T, bool>> criteria);

        IEnumerable<T> GetAll();

        IEnumerable<T> Get(Expression<Func<T, bool>> criteria);

        IEnumerable<T> Get(Expression<Func<T, object>> orderBy, SortOrder sortOrder);

        IEnumerable<T> Get(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy, SortOrder sortOrder);

        IEnumerable<T> Get(Expression<Func<T, object>> orderBy, SortOrder sortOrder, int pageIndex, int pageSize);

        IEnumerable<T> Get(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy, SortOrder sortOrder, int pageIndex, int pageSize);

        IQueryable<T> Get(Expression<Func<T, bool>> criteria, Dictionary<Expression<Func<T, object>>, SortOrder> orders, int pageIndex, int pageSize);
        
        IEnumerable<T> GetWithRawSql(string query, params object[] parameters);

        int Count();

        int Count(Expression<Func<T, bool>> criteria);

        void Add(T entity);

        void Add(IEnumerable<T> entities);

        void Update(T entity);

        void Delete(T entity);

        void Delete(IEnumerable<T> entities);
    }
}