using System.ComponentModel;
using System.Reflection;

namespace ChaTai.Blazor.Data
{
    public static class Extensions
    {
        public static string GetDescription(this System.Enum value)
        {
            return value.GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DescriptionAttribute>()?
                .Description;
        }
    }
}
