using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Utilities
{
    public static class ExtensionMethods
    {
        public static void CopyPropertiesTo<T, TU>(this T source, TU destination)
        {
            Guards.NotNull(source, nameof(source));
            Guards.NotNull(destination, nameof(destination));
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            var destProps = typeof(TU).GetProperties()
                    .Where(x => x.CanWrite)
                    .ToList();

            foreach (var sourceProp in sourceProps)
            {
                var p = destProps.FirstOrDefault(x => x.Name == sourceProp.Name);
                if (p != null && p.PropertyType == sourceProp.PropertyType)
                {
                    p.SetValue(destination, sourceProp.GetValue(source, null), null);
                }
            }
        }

        public static TU MapTo<T, TU>(this T source)
        {
            var dest = Activator.CreateInstance<TU>();
            source.CopyPropertiesTo(dest);
            return dest;
        }
    }
}
