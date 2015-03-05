using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace DevUtils.Elas.Pretranslate.MicrosoftTranslation.DataMarket.V2.Extensions
{
	static class DataMarketExtensions
	{
		public static TokenRequestResult Auth(this IDataMarket dataMarket, string clientId, string clientSecret, string scope, string grantType)
		{
			using (new OperationContextScope((IContextChannel) dataMarket))
			{
				WebOperationContext.Current.OutgoingRequest.ContentType = "application/x-www-form-urlencoded";

				using (var stream = new MemoryStream())
				{
					var sw = new StreamWriter(stream, Encoding.ASCII);
					{
						sw.Write("grant_type={0}&client_id={1}&client_secret={2}&scope={3}",
							HttpUtility.UrlEncode(grantType),
							HttpUtility.UrlEncode(clientId),
							HttpUtility.UrlEncode(clientSecret),
							HttpUtility.UrlEncode(scope));

						sw.Flush();
						stream.Position = 0;
						var result = dataMarket.AuthRaw(stream);

						return result;
					}
				}
			}
		}
	}
}
