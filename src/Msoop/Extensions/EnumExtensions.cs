using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Msoop.Extensions
{
    public static class EnumExtensions
    {
        public static string GetEnumDisplayName(this Enum enumType)
        {
            var displayName = enumType.GetType()
                .GetMember(enumType.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                ?.Name ?? enumType.ToString();

            return displayName;
        }
    }
}
