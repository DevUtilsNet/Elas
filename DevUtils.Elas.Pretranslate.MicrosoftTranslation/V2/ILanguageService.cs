using System.ServiceModel;

namespace DevUtils.Elas.Pretranslate.MicrosoftTranslation.V2
{
	[ServiceContract(Name = "LanguageService", Namespace = "http://api.microsofttranslator.com/V2")]
	interface ILanguageService
	{
		[OperationContract]
		TranslationsResponse GetTranslations(string appId, string text, string from, string to, int maxTranslations, TranslateOptions options);
	}
}