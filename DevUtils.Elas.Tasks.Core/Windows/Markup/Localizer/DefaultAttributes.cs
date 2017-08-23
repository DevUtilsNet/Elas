using System;
using System.Collections.Generic;
using System.Windows;

namespace DevUtils.Elas.Tasks.Core.Windows.Markup.Localizer
{
	static class DefaultAttributes
	{
		private static readonly LocalizabilityAttribute _defaultAttributeUnmodifiable;
		private static readonly Dictionary<object, LocalizabilityAttribute> DefinedAttributes;
		private static readonly LocalizabilityAttribute _defaultAttribute = new LocalizabilityAttribute(LocalizationCategory.None);

		static DefaultAttributes()
		{
			_defaultAttributeUnmodifiable = new LocalizabilityAttribute(LocalizationCategory.Inherit)
			{
				Modifiability = Modifiability.Unmodifiable
			};

			var notReadable = new LocalizabilityAttribute(LocalizationCategory.None) { Readability = Readability.Unreadable };
			var notModifiable = new LocalizabilityAttribute(LocalizationCategory.None) { Modifiability = Modifiability.Unmodifiable };

			DefinedAttributes = new Dictionary<object, LocalizabilityAttribute>()
			{
				{typeof (Byte), notReadable},
				{typeof (Char), notReadable},
				{typeof (SByte), notReadable},
				{typeof (Int32), notReadable},
				{typeof (Int64), notReadable},
				{typeof (Int16), notReadable},
				{typeof (Uri), notModifiable},
				{typeof (Double), notReadable},
				{typeof (Single), notReadable},
				{typeof (UInt32), notReadable},
				{typeof (UInt64), notReadable},
				{typeof (UInt16), notReadable},
				{typeof (Boolean), notReadable},
				{typeof (Decimal), notReadable}
			};

		}

		/// <summary>
		/// Get the localizability attribute for a type
		/// </summary>
		internal static LocalizabilityAttribute GetDefaultAttribute(object type)
		{
			LocalizabilityAttribute ret;
			if (DefinedAttributes.TryGetValue(type, out ret))
			{
				return ret;
			}

			var targetType = type as Type;

			if (targetType != null && targetType.IsValueType)
			{
				return _defaultAttributeUnmodifiable;
			}

			return _defaultAttribute;
		}
	}
}
