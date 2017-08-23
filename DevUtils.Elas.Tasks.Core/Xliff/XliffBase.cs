namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary> An xliff base. </summary>
	public abstract class XliffBase
	{
		/// <summary> Gets or sets the parent. </summary>
		///
		/// <value> The parent. </value>
		public XliffBase Parent { get; private set; }

		/// <summary> Gets or sets the document. </summary>
		///
		/// <value> The document. </value>
		public XliffDocument Document { get; private set; }

		internal XliffBase(XliffDocument document)
		{
			Document = document;
		}

		internal void SetParent(XliffBase parent)
		{
			Parent = parent;
		}

		internal void Dirty()
		{
			if (Document != null)
			{
				Document.IsDirty = true;
			}
		}
	}
}