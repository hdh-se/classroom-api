using IdentityServer4.Extensions;
using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data.Common;
using ManageCourse.Core.Exceptions;
using ManageCourse.Core.Extensions;
using ManageCourse.Core.Helpers;
using ManageCourse.Core.Model.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public static class DbContextExtensions
    {
        private const string AscendingSign = "+";
        private const string DescendingSign = "-";
        public static int CountBy<TEntity>(this DbContext context, BaseEFQuery<TEntity> query)
            where TEntity : class
        {
            var queryable = context.Set<TEntity>().Where(query.GetQueryConditions());
            return queryable.Count();
        }
        public static Task<int> CountByAsync<TEntity>(this DbContext context, BaseEFQuery<TEntity> query)
           where TEntity : class
        {
            var queryable = context.Set<TEntity>().Where(query.GetQueryConditions());
            return queryable.CountAsync();
        }

        public static TEntity GetByKeys<TEntity>(this DbContext context, params object[] keys)
           where TEntity : class
        {
            var entity = context.Set<TEntity>().Find(keys);
            if (entity == null)
            {
                var id = string.Join("_", keys.Select(key => key.ToString()));
                throw CreateObjectNotFoundExpcetion<TEntity>(id);
            }

            return entity;
        }

        public static async Task<TEntity> GetByKeysAsync<TEntity>(this DbContext context, params object[] keys)
            where TEntity : class
        {
            var entity = await context.Set<TEntity>().FindAsync(keys)
                .ConfigureAwait(false);
            if (entity == null)
            {
                var id = string.Join("_", keys.Select(key => key.ToString()));
                throw CreateObjectNotFoundExpcetion<TEntity>(id);
            }

            return entity;
        }

        public static async Task<TEntity> GetByIdOr404Async<TEntity>(this DbContext context, long id,
            bool raiseException = true, params string[] includeNavigationPaths)
            where TEntity : class, IHasId
        {
            var queryable = context.Set<TEntity>().AsQueryable();
            foreach (var includeNavigationPath in includeNavigationPaths)
            {
                queryable = queryable.Include(includeNavigationPath);
            }

            var entity = await queryable.SingleOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            if (raiseException && entity == null)
            {
                throw new NotFoundException(
                    $"The with id '{id}' cannot be found.");
            }

            return entity;
        }
        public static async Task<TEntity> GetAndCheckExisting<TEntity>(this DbContext context, long id,
            params string[] includeNavigationPaths) where TEntity : class, IHasId
        {
            var instance = await context.GetByIdOr404Async<TEntity>(id, false, includeNavigationPaths);
            if (instance == null)
            {
                throw new BusinessRuleException(ApiResultCodes.NotFound,
                    $"The with id '{id}' cannot be found.");
            }
            return instance;
        }

        public static TEntity GetByQuery<TEntity>(this DbContext context, BaseEFGetSingleQuery<TEntity> query)
            where TEntity : class
        {
            var queryable = CreateQueryable(context, query);
            var entity = queryable.FirstOrDefault();
            if (entity == null)
            {
                throw CreateObjectNotFoundExpcetion<TEntity>(query.GetQueryValues());
            }

            return entity;
        }

        public static async Task<TEntity> GetByQueryAsync<TEntity>(this DbContext context,
            BaseEFGetSingleQuery<TEntity> query)
            where TEntity : class
        {
            var queryable = CreateQueryable(context, query);
            var entity = await queryable.FirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (entity == null)
            {
                throw CreateObjectNotFoundExpcetion<TEntity>(query.GetQueryValues());
            }

            return entity;
        }

        public static List<TEntity> QueryBy<TEntity>(this DbContext context, BaseEFQuery<TEntity> query)
            where TEntity : class
        {
            var queryable = CreateQueryable(context, query);
            return queryable.AsNoTracking().ToList();
        }
        public static Task<List<TEntity>> QueryByAsync<TEntity>(this DbContext context, BaseEFQuery<TEntity> query)
            where TEntity : class
        {
            var queryable = CreateQueryable(context, query);
            return queryable.AsNoTracking().ToListAsync();
        }
        private static IQueryable<TEntity> CreateQueryable<TEntity>(this DbContext context, BaseEFQuery<TEntity> query)
            where TEntity : class
        {
            var queryable = context.Set<TEntity>().Where(query.GetQueryConditions());

            if (!string.IsNullOrEmpty(query.SortColumn))
            {
                string property;
                bool isAscending;
                if (query.SortColumn.StartsWith(AscendingSign))
                {
                    property = query.SortColumn.Substring(1).Trim();
                    isAscending = true;
                }
                else if (query.SortColumn.StartsWith(DescendingSign))
                {
                    property = query.SortColumn.Substring(1).Trim();
                    isAscending = false;
                }
                else
                {
                    property = query.SortColumn.Trim();
                    isAscending = true;
                }

                var sortColumnMappings = query.GetSortColumnMappings();
                if (!sortColumnMappings.ContainsKey(property))
                {
                    throw ExceptionHelper.CreateValidationException($"Sort column '{property}'");
                }

                var sortExpression = sortColumnMappings[property];
                queryable = isAscending
                    ? queryable.OrderBy(sortExpression)
                    : queryable.OrderByDescending(sortExpression);
            }

            var includeExpressions = query.GetIncludeExpressions();
            foreach (var expression in includeExpressions)
            {
                queryable = queryable.Include(expression);
            }

            var includeNavigationPaths = query.GetIncludeNavigationPaths();
            foreach (var path in includeNavigationPaths)
            {
                queryable = queryable.Include(path);
            }

            if (query.StartAt.HasValue)
            {
                queryable = queryable.Skip(query.StartAt.Value);
            }

            if (query.MaxResults.HasValue)
            {
                queryable = queryable.Take(query.MaxResults.Value);
            }

            return queryable;
        }
        private static IQueryable<TEntity> CreateQueryable<TEntity>(this DbContext context,
            BaseEFGetSingleQuery<TEntity> query)
            where TEntity : class
        {
            var queryable = context.Set<TEntity>().Where(query.GetQueryConditions());
            var includeExpressions = query.GetIncludeExpressions();
            foreach (var expression in includeExpressions)
            {
                queryable = queryable.Include(expression);
            }

            return queryable;   
        }
        private static BusinessRuleException CreateObjectNotFoundExpcetion<TEntity>(object value)
        {
            return new ObjectNotFoundException(
                $"The {typeof(TEntity).GetClassDescription()} with id '{value}' cannot be found.");
        }

        private static BusinessRuleException CreateObjectNotFoundExpcetion<TEntity>(
            ICollection<Tuple<string, object>> values)
        {
            if (values.IsNullOrEmpty())
            {
                return new ObjectNotFoundException($"The {typeof(TEntity).GetClassDescription()} cannot be found.");
            }

            var value = string.Join(" and ", values.Select(x => $"{x.Item1} '{x.Item2}'"));
            return new ObjectNotFoundException(
                $"The {typeof(TEntity).GetClassDescription()} with {value} cannot be found.");
        }
    }
}
