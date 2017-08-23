using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Markup.Localizer;

namespace DevUtils.Elas.Tasks.Core.Windows.Markup.Localizer
{
	sealed class BamlLocalizabilityResolverByReflection : BamlLocalizabilityResolver
	{
		private static readonly Dictionary<string, string> FormattedElements;
		private static readonly Type TypeOfLocalizabilityAttribute = typeof(LocalizabilityAttribute);

		private readonly Dictionary<string, LocalizabilityAttribute> _localizability;
		private readonly Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

		static BamlLocalizabilityResolverByReflection()
		{
			FormattedElements = new Dictionary<string, string>()
			{
				{"System.Windows.Documents.Bold", "b"},
				{"System.Windows.Documents.Italic", "i"},
				{"System.Windows.Documents.Inline", "in"},
				{"System.Windows.Documents.Hyperlink", "a"},
				{"System.Windows.Documents.Underline", "u"},
				{"System.Windows.Documents.Subscript", "sub"},
				{"System.Windows.Documents.SmallCaps", "small"},
				{"System.Windows.Documents.Superscript", "sup"}
			};
		}

		public BamlLocalizabilityResolverByReflection(
			Dictionary<string, LocalizabilityAttribute> localizability)
		{
			_localizability = localizability;
		}

		#region Overrides of BamlLocalizabilityResolver

		public override ElementLocalizability GetElementLocalizability(string assembly, string className)
		{
			var ret = new ElementLocalizability();

			var type = GetType(assembly, className);
			if (type != null)
			{
				// We found the type, now try to get the localizability attribte from the type
				ret.Attribute = GetLocalizabilityFromType(type);
			}

			string tag;
			if (FormattedElements.TryGetValue(className, out tag))
			{
				ret.FormattingTag = tag;
			}

			return ret;
		}

		public override LocalizabilityAttribute GetPropertyLocalizability(string assembly, string className, string property)
		{
			var type = GetType(assembly, className); 
			if (type == null)
			{
				return null;
			}

			var prop = type.GetProperty(property);
			if (prop != null)
			{
				LocalizabilityAttribute ret;
				if (_localizability != null)
				{
					var propPath = prop.DeclaringType + "." + property;
					if (_localizability.TryGetValue(propPath, out ret))
					{
						return ret;
					}
				}

				var locAttributes = prop.GetCustomAttributes(TypeOfLocalizabilityAttribute, true);
				ret = locAttributes.Length > 0 ? (LocalizabilityAttribute) locAttributes[0] : GetLocalizabilityFromType(prop.PropertyType);
				return ret;
			}

			var attachedDp = DependencyPropertyFromName(property, type);
			if (attachedDp != null)
			{
				LocalizabilityAttribute ret;
				if (_localizability != null)
				{
					var propPath = attachedDp.OwnerType + "." + attachedDp.Name;
					if (_localizability.TryGetValue(propPath, out ret))
					{
						return ret;
					}
				}

				var fieldInfo = attachedDp.OwnerType.GetField(attachedDp.Name + "Property", 
					BindingFlags.Public | 
					BindingFlags.Static | 
					BindingFlags.NonPublic | 
					BindingFlags.FlattenHierarchy);
				if (fieldInfo != null)
				{
					var locAttributes = fieldInfo.GetCustomAttributes(TypeOfLocalizabilityAttribute, true);
					ret = locAttributes.Length > 0 ? (LocalizabilityAttribute)locAttributes[0] : GetLocalizabilityFromType(attachedDp.PropertyType);
					return ret;
				}
			}
			return null;
		}

		public override string ResolveFormattingTagToClass(string formattingTag)
		{
			throw new NotImplementedException();
		}

		public override string ResolveAssemblyFromClass(string className)
		{
			throw new NotImplementedException();
		}

		#endregion

		private Type GetType(string assemblyName, string className)
		{
			var fullName = assemblyName + ":" + className;

			Type ret;
			if (_typeCache.TryGetValue(fullName, out ret))
			{
				return ret;
			}

			//try
			//{
				var assembly = Assembly.Load(assemblyName);

				ret = assembly.GetType(className);
				_typeCache[fullName] = ret;
			//}
			//catch (FileNotFoundException e)
			//{
				
			//}

			return ret;
		}

		private LocalizabilityAttribute GetLocalizabilityFromType(Type type)
		{
			if (type == null)
			{
				return null;
			}

			LocalizabilityAttribute ret;
			if (_localizability != null && _localizability.TryGetValue(type.ToString(), out ret))
			{
				return ret;
			}

			var locAttributes = type.GetCustomAttributes(TypeOfLocalizabilityAttribute, true);

			ret = locAttributes.Length == 0 ? DefaultAttributes.GetDefaultAttribute(type) : (LocalizabilityAttribute)locAttributes[0];
			return ret;
		}

		private static DependencyProperty DependencyPropertyFromName(string propertyName, Type propertyType)
		{
			var fi = propertyType.GetField(propertyName + "Property",
												 BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static |
												 BindingFlags.FlattenHierarchy);
			var ret = (fi != null) ? fi.GetValue(null) as DependencyProperty : null;
			return ret;
		}
	}
}
