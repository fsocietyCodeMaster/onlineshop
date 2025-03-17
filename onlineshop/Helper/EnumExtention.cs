using System.ComponentModel.DataAnnotations;

namespace shoppingApp.Extention
{
    public static class EnumExtention
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            var attribute = field.GetCustomAttributes(typeof(DisplayAttribute), false)
                                 .Cast<DisplayAttribute>()
                                 .FirstOrDefault();
            return attribute?.Name ?? enumValue.ToString();
        }
    }
}
