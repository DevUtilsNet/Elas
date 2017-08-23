using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using EnvDTE;

namespace DevUtils.Elas.Tasks.Core.EnvDTE.Extensions
{
	static class ProjectExtensions
	{
		private static ProjectType GetDetailProjectTypeForVBCS(XContainer doc, XNamespace docNS)
		{
			const string windowsFormsOutputType = ";WinExe;Exe;Library;";
			var ret = ProjectType.Unknown;
			var xElement = doc.Descendants(docNS + "ProjectTypeGuids").FirstOrDefault();
			var text = string.Empty;
			if (xElement != null)
			{
				text = xElement.Value.ToUpper(CultureInfo.InvariantCulture);
			}
			if (text.Contains("{BC8A1FFA-BEE3-4634-8014-F334798102B3}"))
			{
				ret = ProjectType.WindowsStorePri;
			}
			else
			{
				if (text.Contains("{76F1466A-8B6D-4E39-A767-685A06062A39}"))
				{
					ret = ProjectType.WindowsPhonePri;
				}
				else
				{
					if (text.Contains("{C089C8C0-30E0-4E22-80C0-CE093F111A43}") || text.Contains("{DB03555F-0C8B-43BE-9FF9-57896B3C5E56}"))
					{
						ret = ProjectType.WindowsPhoneSilverlight;
					}
					else
					{
						if (text.Contains("{60DC8134-EBA5-43B8-BCC9-BB4BC16C2548}"))
						{
							ret = ProjectType.WpfApplication;
						}
						else
						{
							if (text.Contains("{349C5851-65DF-11DA-9384-00065B846F21}"))
							{
								ret = ProjectType.WebApplication;
							}
							else
							{
								if (text.Contains("{786C830F-07A1-408B-BD7F-6EE04809D6DB}"))
								{
									var xElement2 = doc.Descendants(docNS + "OutputType").FirstOrDefault();
									if (xElement2 != null && xElement2.Value.Equals("winmdobj", StringComparison.OrdinalIgnoreCase))
									{
										ret = ProjectType.UniversalPortableClassLibrary;
									}
									else
									{
										var xElement3 = doc.Descendants(docNS + "TargetFrameworkProfile").FirstOrDefault();
										if (xElement3 != null && xElement3.Value.Equals("Profile32", StringComparison.OrdinalIgnoreCase))
										{
											ret = ProjectType.UniversalPortableClassLibrary;
										}
										else
										{
											ret = ProjectType.PortableClassLibrary;
										}
									}
								}
								else
								{
									if (string.IsNullOrEmpty(text) && (
										from query in doc.Descendants(docNS + "OutputType")
										where windowsFormsOutputType.Contains(";" + query.Value + ";")
										select query).FirstOrDefault<XElement>() != null)
									{
										ret = ProjectType.WindowsApplication;
									}
								}
							}
						}
					}
				}
			}
			return ret;
		}

		public static ProjectType GetProjectType(this Project project)
		{
			var ret = ProjectType.Unknown;

			if (string.IsNullOrEmpty(project.Kind))
			{
				return ret;
			}

			var doc = XDocument.Load(project.FullName);
			var docNS = doc.Root.GetDefaultNamespace();

			var kind = project.Kind.ToUpperInvariant();
			if (!(Equals(kind, "{262852C6-CD72-467D-83FE-5EEB1973A190}")))
			{
				if (kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}" || kind == "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}")
				{
					ret = GetDetailProjectTypeForVBCS(doc, docNS);
					return ret;
				}
				if (kind == "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}")
				{
					if ((
						from query in doc.Descendants(docNS + "AppContainerApplication")
						where query.Value != null && query.Value.Contains("true")
						select query).FirstOrDefault<XElement>() != null)
					{
						ret = ProjectType.WindowsStoreManagedCPP;
						return ret;
					}
					return ret;
				}
			}
			else
			{
				var source = doc.Descendants(docNS + "TargetPlatformIdentifier").ToArray();
				if (!source.Any())
				{
					return ret;
				}
				var xElement = source.FirstOrDefault();
				if (xElement.Value.Equals("Windows", StringComparison.OrdinalIgnoreCase))
				{
					ret = ProjectType.WindowsStoreJavaScript;
					return ret;
				}
				if (xElement.Value.Equals("WindowsPhoneApp", StringComparison.OrdinalIgnoreCase))
				{
					ret = ProjectType.WindowsPhoneJavaScript;
					return ret;
				}
				return ret;
			}
			return ret;
		}

		public static bool IsFileExistsInProject(this Project project, string fullPath)
		{
			var ret = project.ProjectItems.GetAllItems().Any(a => a.FileNames[0].Equals(fullPath, StringComparison.InvariantCultureIgnoreCase));
			return ret;
		}

		public static void AddToProject(this Project project, string fullPath, string dependentUpon = null)
		{
			if (project.IsFileExistsInProject(fullPath))
			{
				return;
			}

			ProjectItems projectItems = null;

			if (!string.IsNullOrEmpty(dependentUpon))
			{
				var projectItem = project.DTE.Solution.FindProjectItem(dependentUpon);
				if (projectItem != null)
				{
					projectItems = projectItem.ProjectItems;
				}
			}

			if (projectItems == null)
			{
				projectItems = project.ProjectItems;
			}

			var newItem = projectItems.AddFromFile(fullPath);

			var projectType = project.GetProjectType();

			switch (projectType)
			{
				case ProjectType.Unknown:
				case ProjectType.WindowsPhoneSilverlight:
				case ProjectType.WpfApplication:
				case ProjectType.WebApplication:
				case ProjectType.WindowsApplication:
				case ProjectType.PortableClassLibrary:
				{
					return;
				}
				case ProjectType.WindowsStoreJavaScript:
				case ProjectType.WindowsPhoneJavaScript:
				{
					newItem.Properties.Item("{ItemType}").Value = "None";
					return;
				}
			}

			newItem.Properties.Item("ItemType").Value = "None";

		}
	}
}
