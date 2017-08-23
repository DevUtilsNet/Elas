namespace DevUtils.Elas.Tasks.Core.Xliff
{
	/// <summary> The status of a particular translation in a <see cref="XliffTarget"/> class. </summary>
	public enum XliffTargetState
	{
		/// <summary> Indicates that the item is new. For example, translation units that were not in a previous version of the document. </summary>
		[StringValue("new")]
		New,
		/// <summary> Indicates that the item needs to be translated. </summary>
		[StringValue("needs-translation")]
		NeedsTranslation,
		/// <summary> Indicates only non-textual information needs adaptation. </summary>
		[StringValue("needs-adaptation")] 
		NeedsAdaptation,
		/// <summary> Indicates both text and non-textual information needs adaptation. </summary>
		[StringValue("needs-l10n")] 
		NeedsL10N,
		/// <summary> Indicates only non-textual information needs review. </summary>
		[StringValue("needs-review-adaptation")] 
		NeedsReviewAdaptation,
		/// <summary> Indicates both text and non-textual information needs review. </summary>
		[StringValue("needs-review-l10n")] 
		NeedsReviewL10N,
		/// <summary> Indicates that only the text of the item needs to be reviewed. </summary>
		[StringValue("needs-review-translation")] 
		NeedsReviewTranslation,
		/// <summary> Indicates that the item has been translated. </summary>
		[StringValue("translated")] 
		Translated,
		/// <summary> Indicates the terminating state. </summary>
		[StringValue("final")]
		Final,
		/// <summary> Indicates that changes are reviewed and approved. </summary>
		[StringValue("signed-off")] 
		SignedOff,
	}
}
