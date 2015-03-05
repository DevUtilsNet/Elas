using System.Runtime.Serialization;

namespace DevUtils.Elas.Pretranslate.MicrosoftTranslation.V2
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2")]
	sealed class TranslateOptions
	{
		[DataMember]
		public string Category { get; set; }

		[DataMember]
		public string ContentType { get; set; }

		[DataMember]
		public bool IncludeMultipleMTAlternatives { get; set; }

		[DataMember]
		public string ReservedFlags { get; set; }

		[DataMember]
		public string State { get; set; }

		[DataMember]
		public string Uri { get; set; }

		[DataMember]
		public string User { get; set; }
	}
}