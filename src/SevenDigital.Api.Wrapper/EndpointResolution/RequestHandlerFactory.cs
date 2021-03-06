using System.Collections.Generic;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;
using SevenDigital.Api.Wrapper.EndpointResolution.RequestHandlers;

namespace SevenDigital.Api.Wrapper.EndpointResolution
{
	public static class RequestHandlerFactory
	{
		public static IEnumerable<RequestHandler> AllRequestHandlers(IOAuthCredentials oAuthCredentials, IApiUri apiUri)
		{
			yield return new GetRequestHandler(apiUri, oAuthCredentials, new OAuthSignatureGenerator());
			yield return new PostRequestHandler(apiUri, oAuthCredentials, new OAuthSignatureGenerator());
		}
	}
}