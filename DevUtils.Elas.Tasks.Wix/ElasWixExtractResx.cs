namespace DevUtils.Elas.Tasks.Wix
{
	///// <summary>
	///// Elas wix extract resx.
	///// </summary>
	//public class ElasWixExtractResx : TaskExtension
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
	//	/// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
	//	/// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
	//	protected override void TryExecute()
	//	{
	//		if (Sources.Length != Targets.Length)
	//		{
	//			throw new ArgumentException("Length of 'Sources' and 'Targets' should be equals");
	//		}

	//		for (var i = 0; i < Sources.Length; i++)
	//		{
	//			var ext = Sources[i].GetMetadata("Extension");
	//			if (!String.Equals(ext, ".wxl", StringComparison.OrdinalIgnoreCase))
	//			{
	//				throw new ArgumentException("BalBamlToResx: Unsupported document type", Sources[i].ToString());
	//			}

	//			var xDocument = XDocument.Load(Sources[i].ToString());

	//			var docElement = (XElement)xDocument.FirstNode;
	//			if (docElement == null)
	//			{
	//				throw new ArgumentNullException(Sources[i].ToString(), "Empty document");
	//			}

	//			var resxFile = new TaskItem(Targets[i].ToString());

	//			using (var resxWritter = new ResXResourceWriter(resxFile.ToString()))
	//			{
	//				foreach (var itemNode in ((XElement)xDocument.FirstNode).Elements(XName.Get("String", docElement.GetDefaultNamespace().NamespaceName)))
	//				{
	//					var id = itemNode.Attribute("Id");

	//					if (id == null || id.Value == "Language" || id.Value == "Culture")
	//					{
	//						continue;
	//					}

	//					var resxData = new ResXDataNode(id.Value, itemNode.Value);

	//					var comment = itemNode.Nodes().OfType<XComment>().FirstOrDefault();
	//					if (comment != null)
	//					{
	//						resxData.Comment = comment.Value;
	//					}

	//					resxWritter.AddResource(resxData);
	//				}
	//			}
	//		}
	//	}
	//}
}
