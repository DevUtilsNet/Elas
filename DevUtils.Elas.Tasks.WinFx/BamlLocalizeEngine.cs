using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Markup.Localizer;

namespace DevUtils.Elas.Tasks.WinFx
{
	class AssemblyResolver : MarshalByRefObject
	{
		static private string[] _references;

		public void Initialize(String[] references)
		{
			if (references != null)
			{
				_references = references;
				AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
			}
		}

		private static Assembly OnAssemblyResolve(Object sender, ResolveEventArgs args)
		{
			var name = new AssemblyName(args.Name);

// ReSharper disable LoopCanBeConvertedToQuery
			foreach (var item in _references)
// ReSharper restore LoopCanBeConvertedToQuery
			{
				if (String.Compare(Path.GetFileNameWithoutExtension(item), name.Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (File.Exists(item))
					{
						return Assembly.UnsafeLoadFrom(item);
					}
				}
			}
			return null;
		}
	}
	class BamlLocalizeEngine : MarshalByRefObject
	{
		private BamlLocalizer _bamlLocalizer;

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public void OpenFile(string bamlPath)
		{
			_bamlLocalizer = new BamlLocalizer(File.OpenRead(bamlPath), new BamlLocalizabilityByReflection());
		}

		public bool WriteResxFile(string fileName)
		{
			var hasResources = false;

			using (var resxWritter = new ResXResourceWriter(fileName))
			{
				var resources = _bamlLocalizer.ExtractResources();

				foreach (var itemRes in resources)
				{
					var key = (BamlLocalizableResourceKey)itemRes.Key;
					var value = (BamlLocalizableResource)itemRes.Value;

					if (!Misc.IsLocalizable(key, value))
					{
						continue;
					}

					var resxData = new ResXDataNode(key.Uid + ":" + key.ClassName + ":" + key.PropertyName, value.Content) { Comment = value.Comments };

					resxWritter.AddResource(resxData);

					hasResources = true;
				}
			}

			return hasResources;
		}

		public void UpdateBamlFileByResxFile(string resxFilePath, string bamlFilePath)
		{
			var resources = _bamlLocalizer.ExtractResources();
			using (var resxReader = new ResXResourceReader(resxFilePath))
			{
				foreach (DictionaryEntry resxItem in resxReader)
				{
					var keyTuple = ((string)resxItem.Key).Split(':').ToArray();
					if (keyTuple.Length != 3)
					{
						throw new ArgumentOutOfRangeException(resxItem.Key.ToString(), keyTuple.Length, "Invalid key length");
					}

					var bamlKey = new BamlLocalizableResourceKey(keyTuple[0], keyTuple[1], keyTuple[2]);
					if (resources.Contains(bamlKey))
					{
						var val = resources[bamlKey];
						val.Content = resxItem.Value.ToString();
					}
				}
			}

			using (var bamlWritter = File.OpenWrite(bamlFilePath))
			{
				_bamlLocalizer.UpdateBaml(bamlWritter, resources);
			}
		}
	}
}
