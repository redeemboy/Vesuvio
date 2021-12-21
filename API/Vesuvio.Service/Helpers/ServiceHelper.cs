using Vesuvio.Core;
using Vesuvio.Domain.DTO;
using LinqKit;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace Vesuvio.Service
{
    public static class ServiceHelper
    {
        private static readonly MethodInfo StringContainsMethod = typeof(string).GetMethod(@"Contains", BindingFlags.Instance |
        BindingFlags.Public, null, new[] { typeof(string) }, null);
        private static readonly MethodInfo BooleanEqualsMethod = typeof(bool).GetMethod(@"Equals", BindingFlags.Instance |
        BindingFlags.Public, null, new[] { typeof(bool) }, null);


        public static Expression<Func<T, bool>> GenerateContainsPredicate<T>(Expression<Func<T, bool>> predicate, string columnName, string searchString) where T : class
        {
            //

            PropertyInfo prop = typeof(T).GetProperty(columnName);// TypeDescriptor.GetProperties(typeof(ApplicationUser)).Find(order.Column, true);

            var dbFieldName = prop.Name;
            // Get the target DB type (table)
            var dbType = typeof(T);
            // Get a MemberInfo for the type's field (ignoring case
            // so "FirstName" works as well as "firstName")
            var dbFieldMemberInfo = dbType.GetMember(dbFieldName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).Single();

            //
            var dbTypeParameter = Expression.Parameter(dbType, @"x");
            // Get at x.firstName
            var dbFieldMember = Expression.MakeMemberAccess(dbTypeParameter, dbFieldMemberInfo);
            // Create the criterion as a constant
            var criterionConstant = new Expression[] { Expression.Constant(searchString) };
            // Create the MethodCallExpression like x.firstName.Contains(criterion)
            var containsCall = Expression.Call(dbFieldMember, StringContainsMethod, criterionConstant);
            // Create a lambda like x => x.firstName.Contains(criterion)
            var lambda = Expression.Lambda(containsCall, dbTypeParameter) as Expression<Func<T, bool>>;

            return predicate.And(lambda);
        }

        public static Expression<Func<T, bool>> GetPredicate<T>(string stringExpression)
        {
            var param = Expression.Parameter(typeof(T), @"x");
            var exp = DynamicExpressionParser.ParseLambda(new[] { param }, null, stringExpression);
            return (Expression<Func<T, bool>>)exp;
        }

        public static Expression<Func<T, object>> GetOrderPredicate<T>(string orderByString) where T : class
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, orderByString);
            var propAsObject = Expression.Convert(property, typeof(object));
            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }

        public static Dictionary<Expression<Func<T, object>>, SortOrder> OrderBy<T>(DataTableRequest request) where T : class
        {
            Dictionary<Expression<Func<T, object>>, SortOrder> orderByDictionary = new Dictionary<Expression<Func<T, object>>, SortOrder>();

            if (request.Order.Count() > 0)
            {
                foreach (var order in request.Order)
                {
                    //Expression<Func<T, object>> orderBy = q => q.Name;
                    if (order.Column != string.Empty)
                    {
                        Expression<Func<T, object>> orderBy = GetOrderPredicate<T>(order.Column);

                        if (order.Dir == "desc")
                        {
                            orderByDictionary.Add(orderBy, SortOrder.Descending);
                        }
                        else
                        {
                            orderByDictionary.Add(orderBy, SortOrder.Ascending);
                        }
                    }
                }

                return orderByDictionary;
            }

            return null;
        }


        public static int GetTotalPageCount<T>(DataTableRequest request, IGenericRepository<T> genericRepository) where T : class
        {
            int totalPageCount = 0;

            if (request != null)
            {
                if (request.Search != null)
                {

                    if (request.Search.Value != string.Empty)
                    {
                        totalPageCount = genericRepository.Count(GetSearchQuery<T>(request));
                    }
                    else
                    {
                        totalPageCount = genericRepository.Count(GetPredicate<T>("True AND DeletedDate == null"));
                    }
                }
                else
                {
                    totalPageCount = genericRepository.Count(GetPredicate<T>("True AND DeletedDate == null"));
                }
            }

            return totalPageCount;
        }

        public static Expression<Func<T, bool>> GetSearchQuery<T>(DataTableRequest request) where T : class
        {

            if (request != null)
            {
                if (request.Search != null)
                {
                    if (request.Search.Value != string.Empty)
                    {
                        string searchValue = request.Search.Value;
                        string query = string.Format("True AND DeletedDate == null AND {0}.Contains(\"{1}\")", request.Search.Column, searchValue);

                        return GetPredicate<T>(query);
                    }
                }

                return GetPredicate<T>("True and DeletedDate == null");
            }

            return null;
        }

    }
}
