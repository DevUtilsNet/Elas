namespace DevUtils.Elas.Tasks.Wix
{
	///// <summary>
	///// Elas wix update by resx.
	///// </summary>
	//public class ElasWixUpdateByResx : TaskExtension
	//{
	//	/// <summary>
	//	/// Gets or sets the sources.
	//	/// </summary>
	//	/// <value>
	//	/// The sources.
	//	/// </value>
	//	[Required]
	//	public ITaskItem[] Sources { get; set; }

	//	/// <summary>
	//	/// Gets or sets the resx files.
	//	/// </summary>
	//	/// <value>
	//	/// The resx files.
	//	/// </value>
	//	[Required]
	//	public ITaskItem[] ResxFiles { get; set; }

	//	/// <summary>
	//	/// Gets or sets the targets.
	//	/// </summary>
	//	/// <value>
	//	/// The targets.
	//	/// </value>
	//	[Required]
	//	public ITaskItem[] Targets { get; set; }

	//	/// <summary>
	//	/// Try execute.
	//	/// </summary>
	//	/// <exception cref="ArgumentException">		 Thrown when one or more arguments have unsupported or
	//	/// 																				 illegal values. </exception>
	//	/// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
	//	/// <exception cref="Exception">						 Thrown when an exception error condition occurs. </exception>
	//	protected override void TryExecute()
	//	{
	//		if (Sources.Length != ResxFiles.Length || Sources.Length != Targets.Length)
	//		{
	//			throw new ArgumentException("Length of 'Sources', 'ResxFile' and 'Target' should be equals");
	//		}

	//		for (var i = 0; i < Sources.Length; i++)
	//		{
	//			var ext = Sources[i].GetMetadata("Extension");
	//			if (!String.Equals(ext, ".wxl", StringComparison.OrdinalIgnoreCase))
	//			{
	//				throw new ArgumentException("BalBamlUpdateByResx: Unsupported document type", Sources[i].ToString());
	//			}

	//			var xDocument = XDocument.Load(Sources[i].ToString());

	//			var docElement = (XElement)xDocument.FirstNode;
	//			if (docElement == null)
	//			{
	//				throw new ArgumentNullException(Sources[i].ToString(), "Empty document");
	//			}

	//			var culture = new CultureInfo(ResxFiles[i].GetMetadata("Culture"));
	//			if (culture.Equals(CultureInfo.InvariantCulture))
	//			{
	//				throw new Exception("Resx file not have culture");
	//			}

	//			{
	//				var attr = docElement.Attribute("Culture");
	//				if (attr == null)
	//				{
	//					attr = new XAttribute("Culture", culture.Name);
	//					docElement.Add(attr);
	//				}
	//				else
	//				{
	//					attr.Value = culture.Name;
	//				}
	//			}

	//			using (var resxReader = new ResXResourceReader(ResxFiles[i].ToString()))
	//			{
	//				foreach (var item in docElement.Elements(XName.Get("String", docElement.GetDefaultNamespace().NamespaceName)))
	//				{
	//					var id = item.Attribute("Id");
	//					if (id == null)
	//					{
	//						continue;
	//					}
	//					if (id.Value == "Language")
	//					{
	//						item.ReplaceNodes(new XText(culture.LCID.ToString(CultureInfo.InvariantCulture)));
	//						continue;
	//					}

	//					if (id.Value == "Culture")
	//					{
	//						item.ReplaceNodes(new XText(culture.Name));
	//						continue;
	//					}

	//					var entry = resxReader.Cast<DictionaryEntry>().FirstOrDefault(f => f.Key.ToString() == id.Value);
	//					if (!entry.Equals(default(DictionaryEntry)))
	//					{
	//						item.ReplaceNodes(new XText(entry.Value.ToString()));
	//					}
	//				}
	//			}

	//			xDocument.Save(Targets[i].ToString());
	//		}
	//	}
	//}
}
