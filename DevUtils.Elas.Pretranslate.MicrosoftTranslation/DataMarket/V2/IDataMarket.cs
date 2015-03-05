using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace DevUtils.Elas.Pretranslate.MicrosoftTranslation.DataMarket.V2
{
	[ServiceContract]
	interface IDataMarket
	{
		[OperationContract]
		[WebInvoke(UriTemplate = "v2/OAuth2-13")]
		TokenRequestResult AuthRaw(Stream body);
	}
}