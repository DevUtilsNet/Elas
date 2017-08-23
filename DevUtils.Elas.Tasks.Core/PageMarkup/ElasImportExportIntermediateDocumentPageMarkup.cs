
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup.Localizer;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using DevUtils.Elas.Tasks.Core.Extensions;
using Microsoft.Build.Framework;

namespace DevUtils.Elas.Tasks.Core.PageMarkup
{
	/// <summary> The elas import export intermediate document page markup. </summary>
	public abstract class ElasImportExportIntermediateDocumentPageMarkup : AppDomainIsolatedTaskExtension
	{
		private Tuple<string, string>[] _references;

		/// <summary> Gets or sets the references. </summary>
		///
		/// <value> The references. </value>
		public ITaskItem[] References { get; set; }

		/// <summary> Gets or sets the localizability definitions. </summary>
		///
		/// <value> The localizability definitions. </value>
		public ITaskItem[] LocalizabilityDefinitions { get; set; }

		/// <summary> Gets or sets the processed files. </summary>
		///
		/// <value> The processed files. </value>
		[Output]
		public ITaskItem[] ProcessedFiles { get; set; }

		/// <summary> The localizability. </summary>
		protected Dictionary<string, LocalizabilityAttribute> Localizability;

		/// <summary> Gets an identifier. </summary>
		///
		/// <param name="key"> The key. </param>
		///
		/// <returns> The identifier. </returns>
		protected static string GetId(BamlLocalizableResourceKey key)
		{
			var ret = key.Uid + "." + key.PropertyName;
			return ret;
		}

		private static LocalizabilityAttribute GetLocalizabilityAttribute(ITaskItem taskItem)
		{
			var ret = new LocalizabilityAttribute(
						taskItem.GetMetadata("Category").TryParse(LocalizationCategory.Inherit))
			{
				Readability = taskItem.GetMetadata("Readability").TryParse(Readability.Inherit),
				Modifiability = taskItem.GetMetadata("Modifiability").TryParse(Modifiability.Inherit)
			};

			return ret;
		}

		#region Overrides of TaskExtension

		/// <summary>
		/// Try execute.
		/// </summary>
		protected override void TryExecute()
		{
			if (References == null)
			{
				return;
			}

			if (LocalizabilityDefinitions != null)
			{
				Localizability = new Dictionary<string, LocalizabilityAttribute>(LocalizabilityDefinitions.Length);
				foreach (var item in LocalizabilityDefinitions)
				{
					var attribute = GetLocalizabilityAttribute(item);
					Localizability[item.ItemSpec] = attribute;
				}
			}

			_references = References.Select(Create).ToArray();

			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyAssemblyResolve;
			try
			{
				Process();
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
				AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= OnReflectionOnlyAssemblyResolve;
			}
		}

		#endregion

		private static Tuple<string, string> Create(ITaskItem ti)
		{
			var fn = ti.GetMetadata("FusionName");
			var ret = Tuple.Create(new AssemblyName(string.IsNullOrEmpty(fn) ? ti.RequestMetadata(MSBuildWellKnownItemMetadates.Filename) : fn).ToString(), ti.ItemSpec);
			return ret;
		}

		private string GetAssemblyFile(string assemblyName)
		{
			for (var i = 0; i < 2; ++i)
			{
				var reference = _references.FirstOrDefault(f => f.Item1.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase));

				if (reference != null)
				{
					return reference.Item2;
				}

				var an = new AssemblyName(assemblyName);
				assemblyName = an.Name;
			}
			
			return null;
		}

		private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			var assemblyFile = GetAssemblyFile(args.Name);
			if (!string.IsNullOrEmpty(assemblyFile))
			{
				var ret = Assembly.UnsafeLoadFrom(assemblyFile);
				return ret;
			}
			return null;
		}

		private Assembly OnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
		{
			var assemblyFile = GetAssemblyFile(args.Name);
			if (!string.IsNullOrEmpty(assemblyFile))
			{
				var ret = Assembly.ReflectionOnlyLoadFrom(assemblyFile);
				return ret;
			}
			return null;
		}

		/// <summary> Process this object. </summary>
		protected abstract void Process();
	}
}
