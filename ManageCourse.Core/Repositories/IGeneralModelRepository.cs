using ManageCourse.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Repositories
{
    public interface IGeneralModelRepository
    {
        IQueryable<T> GetQueryable<T>() where T : class;

        Task<T> Get<T>(long id, bool raiseException = true, params string[] includeNavigationPaths)
            where T : class, IHasId;

        Task<TEntity> GetAndCheckExisting<TEntity>(long id,
            params string[] includeNavigationPaths) where TEntity : class, IHasId;

        Task<T> GetByKeys<T>(params object[] keys) where T : class;

        Task<T> Create<T>(T instance) where T : class;

        Task<T> Update<T>(T instance) where T : class;

        Task SaveAll<T>() where T : class;

        Task Delete<T>(long id) where T : class, IHasId;

        Task Delete<T>(T instance) where T : class, IHasId;
    }
}
