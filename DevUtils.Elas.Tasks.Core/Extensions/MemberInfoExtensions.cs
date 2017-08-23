using System;
using System.Linq;
using System.Reflection;

namespace DevUtils.Elas.Tasks.Core.Extensions
{
	static class MemberInfoExtensions
	{
		public static T GetCustomAttributeT<T>(this MemberInfo member) where T : Attribute
		{
			var attributeType = typeof(T);

			if (!member.Module.Assembly.ReflectionOnly)
			{
				return (T)Attribute.GetCustomAttribute(member, attributeType, false);
			}

			var fullName = attributeType.FullName;

			foreach (var item in CustomAttributeData.GetCustomAttributes(member))
			{
				var fullName2 = item.Constructor.ReflectedType.FullName;
				if (string.Equals(fullName2, fullName, StringComparison.Ordinal))
				{
					var ctor = attributeType.GetConstructors().Single(s => s.GetParameters().Length == item.ConstructorArguments.Count);

					var attribute = (Attribute)ctor.Invoke(item.ConstructorArguments.Select(s => s.Value).ToArray());

					var type = attribute.GetType();
					foreach (var arguments in item.NamedArguments)
					{
						var property = type.GetProperty(arguments.MemberInfo.Name, BindingFlags.Instance | BindingFlags.Public);
						var obj = arguments.TypedValue.Value;
						if (property.PropertyType.IsEnum)
						{
							obj = Enum.Parse(property.PropertyType, obj.ToString());
						}
						property.SetValue(attribute, obj, null);
					}
					return (T)attribute;
				}
			}
			return null;
		}
	}
}
