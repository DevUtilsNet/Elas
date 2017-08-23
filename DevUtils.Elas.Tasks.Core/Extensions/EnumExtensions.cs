using System;
using DevUtils.Elas.Tasks.Core.Xliff;

namespace DevUtils.Elas.Tasks.Core.Extensions
{
	static class EnumExtensions
	{
		public static string GetStringValue(this Enum @enum)
		{
			var field = @enum.GetType().GetField(@enum.ToString());
			var attribute = field.GetCustomAttributeT<StringValueAttribute>();
			if (attribute != null)
			{
				var ret = attribute.Value;
				return ret;
			}
			else
			{
				var ret = @enum.ToString();
				return ret;
			}
		}

		public static T TryParse<T>(this string value, T @default) where T : struct
		{
			T @enum;
			var ret = Enum.TryParse(value, out @enum) ? @enum : @default;
			return ret;
		}

		public static T EnumFromStringValue<T>(this string value, T @default = default(T)) where T : struct
		{
			var fields = typeof (T).GetFields();
			foreach (var item in fields)
			{
				var attribute = item.GetCustomAttributeT<StringValueAttribute>();
				if (attribute != null && attribute.Value == value)
				{
					return (T)item.GetRawConstantValue();
				}
			}
			return @default;
		}
	}
}
