using ManageCourse.Core.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class DbContextContainer
    {
        private readonly IServiceProvider _serviceProvider;
        private static ConcurrentDictionary<Type, Type> _entityMappingDictionary = new ConcurrentDictionary<Type, Type>();

        public DbContextContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public DbContext GetContextFor<TEntity>()
        {
            return GetContextFor(typeof(TEntity));
        }
        public DbContext GetContextFor(Type entityType)
        {
            var contextType = _entityMappingDictionary.GetOrAdd(entityType, MappingDictionary_GetOrAdd);
            if (contextType == null)
            {
                throw new InvalidOperationException($"No context is available for entity type");
            }
            return (DbContext)_serviceProvider.GetService(contextType);
        }
        private Type MappingDictionary_GetOrAdd(Type entityType)
        {
            var contextTypes = new[]
            {
                typeof(AuthDbContext),
                typeof(AppDbContext),
            };
            foreach (var contextType in contextTypes)
            {
                var result = IsEntityMapped(entityType, contextType);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private static Type IsEntityMapped(Type entityType, Type contextType)
        {
            var properties = contextType.GetProperties();
            var isMapped = properties.Any(prop => prop.PropertyType.IsGenericType
                && prop.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
                && prop.PropertyType.GetGenericArguments().Length == 1
                && prop.PropertyType.GetGenericArguments().First() == entityType);
            return isMapped ? contextType : null;
        }

    }
}
