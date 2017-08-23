using System;
using System.Windows;
using System.Windows.Markup.Localizer;

namespace DevUtils.Elas.Tasks.WinFx
{
	static class Misc
	{
		internal static bool IsLocalizable(BamlLocalizableResourceKey resourceKey, BamlLocalizableResource resource)
		{
			if (!resource.Readable)
			{
				return false;
			}

			if (!resource.Modifiable)
			{
				return false;
			}

			return !String.IsNullOrEmpty(resource.Content) &&
				resource.Category != LocalizationCategory.None &&
				resource.Category != LocalizationCategory.NeverLocalize &&
				resource.Category != LocalizationCategory.Ignore &&
				resource.Category != LocalizationCategory.Font &&
				resource.Category != LocalizationCategory.XmlData &&
				resource.Category != LocalizationCategory.Hyperlink;
		}
	}
}
