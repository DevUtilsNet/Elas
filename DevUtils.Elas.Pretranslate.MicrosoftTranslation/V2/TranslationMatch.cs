using System.Diagnostics;
using System.Runtime.Serialization;
using DevUtils.Elas.Tasks.Core.Diagnostics;

namespace DevUtils.Elas.Pretranslate.MicrosoftTranslation.V2
{
	[DebuggerDisplay("{GetDump()}")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2")]
	sealed class TranslationMatch
	{
		[DataMember(IsRequired = true)]
		public int Count { get; set; }

		[DataMember]
		public string Error { get; set; }

		[DataMember(IsRequired = true)]
		public int MatchDegree { get; set; }

		[DataMember]
		public string MatchedOriginalText { get; set; }

		[DataMember(IsRequired = true)]
		public int Rating { get; set; }

		[DataMember(IsRequired = true)]
		public string TranslatedText { get; set; }

		private string GetDump()
		{
			var ret = ObjectDumper.Dump(this);
			return ret;
		}
	}
}
