using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.Data.Common;
using ManageCourse.Core.Exceptions;
using ManageCourse.Core.Extensions;
using ManageCourse.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Repositories
{
    public class GeneralModelRepository : IGeneralModelRepository
    {
        private DbContextContainer ContextContainer { get; }
        public GeneralModelRepository(DbContextContainer contextContainer)
        {
            ContextContainer = contextContainer;
        }

        public async Task<T> Create<T>(T instance) where T : class
        {
            Guards.NotNull(instance, nameof(instance));
            var context = ContextContainer.GetContextFor<T>();
            await context.AddAsync(instance).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return instance;
        }

        public async Task<T> GetByKeys<T>(params object[] keys) where T : class
        {
            var entity = await ContextContainer.GetContextFor<T>()
                .Set<T>()
                .FindAsync(keys)
                .ConfigureAwait(false);
            if (entity == null)
            {
                var id = string.Join("_", keys.Select(key => key.ToString()));
                throw new ObjectNotFoundException(
                    $"The {typeof(T).GetClassDescription()} with id '{id}' cannot be found.");
            }

            return entity;
        }

        public IQueryable<T> GetQueryable<T>() where T : class
        {
            return ContextContainer.GetContextFor<T>().Set<T>().AsQueryable();
        }

        public async Task SaveAll<T>() where T : class
        {
            var context = ContextContainer.GetContextFor<T>();
            _ = await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<T> Update<T>(T instance) where T : class
        {
            Guards.NotNull(instance, nameof(instance));
            var context = ContextContainer.GetContextFor<T>();
            await context.SaveChangesAsync().ConfigureAwait(false);
            return instance;
        }

        public async Task Delete<T>(long id) where T : class, IHasId
        {
            var instance = await Get<T>(id);
            await Delete(instance);
        }

        public async Task Delete<T>(T instance) where T : class, IHasId
        {
            var context = ContextContainer.GetContextFor<T>();
            context.Remove(instance);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<T> Get<T>(long id, bool raiseException = true, params string[] includeNavigationPaths) where T : class, IHasId
        {
            return await ContextContainer.GetContextFor<T>()
               .GetByIdOr404Async<T>(id, raiseException, includeNavigationPaths);
        }

        public async Task<TEntity> GetAndCheckExisting<TEntity>(long id, params string[] includeNavigationPaths) where TEntity : class, IHasId
        {
            var instance = await Get<TEntity>(id, false, includeNavigationPaths);
            if (instance == null)
            {
                throw new BusinessRuleException(ApiResultCodes.NotFound,
                    $"The {typeof(TEntity).GetClassDescription()} with id '{id}' cannot be found.");
            }
            return instance;
        }
    }
}
