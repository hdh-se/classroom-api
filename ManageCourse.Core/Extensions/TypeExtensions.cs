using System.ComponentModel;
using System.Linq;

namespace System
{
    public static class TypeExtensions
    {
        public static string GetClassDescription<T>(this T type)
            where T : Type
        {
            var attribute = type.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
            return attribute == null ? type.Name : attribute.Description;
        }
    }
}
