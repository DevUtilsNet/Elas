using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using DevUtils.Elas.Tasks.Core.Diagnostics;

namespace DevUtils.Elas.Pretranslate.MicrosoftTranslation.V2
{
	[DebuggerDisplay("{GetDump()}")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2")]
	sealed class TranslationsResponse
	{
		[DataMember]
		public string From { get; set; }

		[DataMember]
		public string State { get; set; }

		[DataMember]
		public IEnumerable<TranslationMatch> Translations { get; set; }

		private string GetDump()
		{
			var ret = ObjectDumper.Dump(this);
			return ret;
		}
	}
}