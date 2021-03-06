﻿namespace DevUtils.Elas.Tasks.WinFx
{
	///// <summary> The elas baml update by resx. </summary>
	//public class ElasBamlUpdateByResx : TaskExtension
	//{
	//	/// <summary> Gets or sets the sources. </summary>
	//	/// <value> The sources. </value>
	//	[Required]
	//	public ITaskItem[] Sources { get; set; }
	//	/// <summary> Gets or sets the resx files. </summary>
	//	/// <value> The resx files. </value>
	//	[Required]
	//	public ITaskItem[] ResxFiles { get; set; }
	//	/// <summary> Gets or sets the targets. </summary>
	//	/// <value> The targets. </value>
	//	[Required]
	//	public ITaskItem[] Targets { get; set; }

	//	/// <summary>Gets or sets the references to load types in .resx files from.</summary>
	//	/// <returns>The references to load types in .resx files from.</returns>
	//	[Required]
	//	public ITaskItem[] References { get; set; }
	//	/// <summary> Gets or sets the pathname of the directory. </summary>
	//	/// <value> The pathname of the directory. </value>
	//	public String Directory { get; set; }
	//	/// <summary> Try execute. </summary>
	//	/// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or
	//	/// 																		 illegal values. </exception>
	//	protected override void TryExecute()
	//	{
	//		if (Sources.Length != ResxFiles.Length || Sources.Length != Targets.Length)
	//		{
	//			throw new ArgumentException("Length of 'Sources', 'ResxFile' and 'Target' should be equals", "Sources");
	//		}

	//		var appDomain = AppDomain.CreateDomain("BamlLocalizeEngineDomain", null, Directory, String.Empty, false);
	//		try
	//		{

	//			{
	//				var obj = appDomain.CreateInstanceFromAndUnwrap(typeof(AssemblyResolver).Module.FullyQualifiedName, typeof(AssemblyResolver).FullName);
	//				var resolver = (AssemblyResolver)obj;
	//				resolver.Initialize(References.Select(s => s.ToString()).ToArray());
	//			}

	//			BamlLocalizeEngine engine;
	//			{
	//				var obj = appDomain.CreateInstanceFromAndUnwrap(typeof(BamlLocalizeEngine).Module.FullyQualifiedName, typeof(BamlLocalizeEngine).FullName);
	//				engine = (BamlLocalizeEngine)obj;
	//			}

	//			for (var i = 0; i < Sources.Length; i++)
	//			{
	//				var ext = Sources[i].GetMetadata("Extension");
	//				if (!String.Equals(ext, ".baml", StringComparison.OrdinalIgnoreCase))
	//				{
	//					throw new ArgumentException("BalBamlUpdateByResx: Unsupported document type", Sources[i].ToString());
	//				}

	//				engine.OpenFile(Sources[i].ToString());

	//				engine.UpdateBamlFileByResxFile(ResxFiles[i].ToString(), Targets[i].ToString());
	//			}
	//		}
	//		finally
	//		{
	//			AppDomain.Unload(appDomain);
	//		}
	//	}
	//}
}
