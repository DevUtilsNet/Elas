using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Markup.Localizer;

namespace DevUtils.Elas.Tasks.WinFx
{
	/// <summary>
	/// </summary>
	public class BamlLocalizabilityByReflection : BamlLocalizabilityResolver
	{
		private static readonly string[] WellKnownAssemblyNames = new[]
			{
				"presentationcore",
				"presentationframework",
				"windowsbase"
			};

		// the well known assemblies
		private static readonly Assembly[] WellKnownAssemblies = new Assembly[WellKnownAssemblyNames.Length];

		private static readonly Type TypeOfLocalizabilityAttribute = typeof(LocalizabilityAttribute);

		// supported elements that are formatted inline		
		private static readonly string[] FormattedElements = new[]
			{
					"System.Windows.Documents.Bold",
					"System.Windows.Documents.Hyperlink",
					"System.Windows.Documents.Inline",
					"System.Windows.Documents.Italic",
					"System.Windows.Documents.SmallCaps",
					"System.Windows.Documents.Subscript",
					"System.Windows.Documents.Superscript",
					"System.Windows.Documents.Underline"
			};

		// corresponding tag
		private static readonly string[] FormattingTag = new[]
			{
					"b",
					"a",
					"in",
					"i",
					"small",
					"sub",
					"sup",
					"u"
			};

		private readonly Dictionary<string, Assembly> _assemblies; // the _assemblies table
		private readonly Dictionary<string, Type> _typeCache; // the types cache

		/// <summary>
		/// Take in an optional list of assemblies in addition to the 
		/// default well-known WPF assemblies. The assemblies will be searched first
		/// in order before the well-known WPF assemblies.
		/// </summary>
		/// <param name="assemblies">additinal list of assemblies to search for Type information</param>
		public BamlLocalizabilityByReflection(params Assembly[] assemblies)
		{
			if (assemblies != null)
			{
				// create the table
				_assemblies = new Dictionary<string, Assembly>(assemblies.Length);

				// Assert security permissions
				//var permobj = new FileIOPermission(PermissionState.None)
				//                  {AllFiles = FileIOPermissionAccess.PathDiscovery};
				//CASRemoval:permobj.Assert();

				foreach (var t in assemblies)
				{
					// skip the null ones. 
					if (t != null)
					{
						// index it by the name;
						_assemblies[t.GetName().FullName] = t;
					}
				}
			}

			// create the cache for Type here
			_typeCache = new Dictionary<string, Type>(32);
		}

		/// <summary>
		/// Return the localizability of an element to the BamlLocalizer
		/// </summary>
		public override ElementLocalizability GetElementLocalizability(
			string assembly,
			string className
			)
		{
			var loc = new ElementLocalizability();

			var type = GetType(assembly, className);
			if (type != null)
			{
				// We found the type, now try to get the localizability attribte from the type
				loc.Attribute = GetLocalizabilityFromType(type);
			}

			// fill in the formatting tag
			var index = Array.IndexOf(FormattedElements, className);
			if (index >= 0)
			{
				loc.FormattingTag = FormattingTag[index];
			}

			return loc;
		}

		/// <summary>
		/// return localizability of a property to the BamlLocalizer
		/// </summary>
		public override LocalizabilityAttribute GetPropertyLocalizability(
			string assembly,
			string className,
			string property
			)
		{
			LocalizabilityAttribute attribute = null;

			var type = GetType(assembly, className);

			if (type != null)
			{
				// type of the property. The type can be retrieved from CLR property, or Attached property.
				Type clrPropertyType, attachedPropertyType = null;

				// we found the type. try to get to the property as Clr property                    
				GetLocalizabilityForClrProperty(
					property,
					type,
					out attribute,
					out clrPropertyType
					);

				if (attribute == null)
				{
					// we didn't find localizability as a Clr property on the type,
					// try to get the property as attached property
					GetLocalizabilityForAttachedProperty(
						property,
						type,
						out attribute,
						out attachedPropertyType
						);
				}

				if (attribute == null)
				{
					// if attached property doesn't have [LocalizabilityAttribute] defined,
					// we get it from the type of the property.
					attribute = (clrPropertyType != null)
									? GetLocalizabilityFromType(clrPropertyType)
									: GetLocalizabilityFromType(attachedPropertyType);
				}
			}

			return attribute;
		}

		/// <summary>
		/// Resolve a formatting tag back to the actual class name
		/// </summary>
		public override string ResolveFormattingTagToClass(
			string formattingTag
			)
		{
			var index = Array.IndexOf(FormattingTag, formattingTag);
			if (index >= 0)
				return FormattedElements[index];
			return null;
		}

		/// <summary>
		/// Resolve a class name back to its containing assembly 
		/// </summary>
		public override string ResolveAssemblyFromClass(
			string className
			)
		{
			// search through the well-known assemblies
			for (var i = 0; i < WellKnownAssemblies.Length; i++)
			{
				if (WellKnownAssemblies[i] == null)
				{
					WellKnownAssemblies[i] = Assembly.Load(
						GetCompatibleAssemblyName(WellKnownAssemblyNames[i])
						);
				}

				if (WellKnownAssemblies[i] != null && WellKnownAssemblies[i].GetType(className) != null)
				{
					return WellKnownAssemblies[i].GetName().FullName;
				}
			}

			// search through the custom assemblies
			if (_assemblies != null)
			{
// ReSharper disable LoopCanBeConvertedToQuery
				foreach (var pair in _assemblies)
// ReSharper restore LoopCanBeConvertedToQuery
				{
					if (pair.Value.GetType(className) != null)
					{
						return pair.Value.GetName().FullName;
					}
				}
			}

			return null;
		}

		//-----------------------------------------------
		// Private methods
		//-----------------------------------------------
		// get the type in a specified assembly
		private Type GetType(string assemblyName, string className)
		{
			Debug.Assert(className != null, "classname can't be null");
			Debug.Assert(assemblyName != null, "Assembly name can't be null");

			// combine assembly name and class name for unique indexing
			var fullName = assemblyName + ":" + className;

			if (_typeCache.ContainsKey(fullName))
			{
				// we found it in the cache, so just return
				return _typeCache[fullName];
			}

			// we didn't find it in the table. So let's get to the assembly first
			Assembly assembly = null;
			if (_assemblies != null && _assemblies.ContainsKey(assemblyName))
			{
				// find the assembly in the hash table first
				assembly = _assemblies[assemblyName];
			}

			if (assembly == null)
			{
				// we don't find the assembly in the hashtable
				// try to use the default well known assemblies
				int index;
				if ((index = Array.BinarySearch(
					WellKnownAssemblyNames,
					GetAssemblyShortName(assemblyName).ToLower(CultureInfo.InvariantCulture)
								 )
					) >= 0
					)
				{
					// see if we already loaded the assembly
					if (WellKnownAssemblies[index] == null)
					{
						// it is a well known name, load it from the gac
						WellKnownAssemblies[index] = Assembly.Load(assemblyName);
					}

					assembly = WellKnownAssemblies[index];
				}
			}

			var type = assembly != null ? assembly.GetType(className) : null;

			// remember what we found out.
			_typeCache[fullName] = type;
			return type; // return
		}

		// returns the short name for the assembly
		private static string GetAssemblyShortName(string assemblyFullName)
		{
			var commaIndex = assemblyFullName.IndexOf(',');
			if (commaIndex > 0)
			{
				return assemblyFullName.Substring(0, commaIndex);
			}

			return assemblyFullName;
		}

		private static string GetCompatibleAssemblyName(string shortName)
		{
			AssemblyName asmName = null;
			foreach (var assembly in WellKnownAssemblies)
			{
				if (assembly != null)
				{
					// create an assembly name with the same version and token info
					// as the WPF assembilies
					asmName = assembly.GetName();
					asmName.Name = shortName;
					break;
				}
			}

			if (asmName == null)
			{
				// there is no WPF assembly loaded yet. We will just get the compatible version 
				// of the current PresentationFramework
				var presentationFramework = typeof(BamlLocalizer).Assembly;
				asmName = presentationFramework.GetName();
				asmName.Name = shortName;
			}

			return asmName.ToString();
		}

		/// <summary>
		/// gets the localizabiity attribute of a given the type
		/// </summary>        
		private static LocalizabilityAttribute GetLocalizabilityFromType(Type type)
		{
			if (type == null) return null;

			// let's get to its localizability attribute.
			var locAttributes = type.GetCustomAttributes(
				TypeOfLocalizabilityAttribute, // type of localizability
				true // search for inherited value
				);

			if (locAttributes.Length == 0)
			{
				return DefaultAttributes.GetDefaultAttribute(type);
			}
			Debug.Assert(locAttributes.Length == 1, "Should have only 1 localizability attribute");

			// use the one defined on the class
			return (LocalizabilityAttribute)locAttributes[0];
		}


		/// <summary>
		/// Get the localizability of a CLR property
		/// </summary>
		private static void GetLocalizabilityForClrProperty(
			string propertyName,
			Type owner,
			out LocalizabilityAttribute localizability,
			out Type propertyType
			)
		{
			localizability = null;
			propertyType = null;

			var info = owner.GetProperty(propertyName);
			if (info == null)
			{
				return; // couldn't find the Clr property
			}

			// we found the CLR property, set the type of the property
			propertyType = info.PropertyType;

			var locAttributes = info.GetCustomAttributes(
				TypeOfLocalizabilityAttribute, // type of the attribute
				true // search in base class
				);

			if (locAttributes.Length == 0)
			{
				return;
			}
			Debug.Assert(locAttributes.Length == 1, "Should have only 1 localizability attribute");

			// we found the attribute defined on the property
			localizability = (LocalizabilityAttribute)locAttributes[0];
		}

		/// <summary>
		/// Get localizability for attached property
		/// </summary>
		/// <param name="propertyName">property name</param>
		/// <param name="owner">owner type</param>
		/// <param name="localizability">out: localizability attribute</param>
		/// <param name="propertyType">out: type of the property</param>
		private static void GetLocalizabilityForAttachedProperty(
			string propertyName,
			Type owner,
			out LocalizabilityAttribute localizability,
			out Type propertyType
			)
		{
			localizability = null;
			propertyType = null;

			// if it is an attached property, it should have a dependency property with the name 
			// <attached proeprty's name> + "Property"
			var attachedDp = DependencyPropertyFromName(
				propertyName, // property name
				owner
				); // owner type

			if (attachedDp == null)
				return; // couldn't find the dp.

			// we found the Dp, set the type of the property
			propertyType = attachedDp.PropertyType;

			var fieldInfo = attachedDp.OwnerType.GetField(
				attachedDp.Name + "Property",
				BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.Static | BindingFlags.FlattenHierarchy);

			Debug.Assert(fieldInfo != null);

			var attributes = fieldInfo.GetCustomAttributes(
				TypeOfLocalizabilityAttribute, // type of localizability
				true
				); // inherit

			if (attributes.Length == 0)
			{
				// didn't find it.
				return;
			}
			Debug.Assert(attributes.Length == 1, "Should have only 1 localizability attribute");
			localizability = (LocalizabilityAttribute)attributes[0];
		}

		private static DependencyProperty DependencyPropertyFromName(string propertyName, Type propertyType)
		{
			var fi = propertyType.GetField(propertyName + "Property",
												 BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static |
												 BindingFlags.FlattenHierarchy);
			return (fi != null) ? fi.GetValue(null) as DependencyProperty : null;
		}

		//---------------------------
		// private members
		//---------------------------     
	}
}