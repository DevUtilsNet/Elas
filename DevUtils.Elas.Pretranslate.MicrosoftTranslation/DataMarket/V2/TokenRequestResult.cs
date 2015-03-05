using System.Runtime.Serialization;

namespace DevUtils.Elas.Pretranslate.MicrosoftTranslation.DataMarket.V2
{
	[DataContract]
	sealed class TokenRequestResult
	{
		[DataMember(Name = "access_token")]
		public string AccessToken { get; set; }
		[DataMember(Name = "token_type")]
		public string TokenType { get; set; }
		[DataMember(Name = "expires_in")]
		public int ExpiresIn { get; set; }
		[DataMember(Name = "scope")]
		public string Scope { get; set; }
	}
}