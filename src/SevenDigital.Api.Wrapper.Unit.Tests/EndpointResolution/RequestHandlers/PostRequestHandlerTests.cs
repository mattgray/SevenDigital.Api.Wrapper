﻿using FakeItEasy;
using NUnit.Framework;
using SevenDigital.Api.Wrapper.EndpointResolution.OAuth;
using SevenDigital.Api.Wrapper.EndpointResolution.RequestHandlers;
using SevenDigital.Api.Wrapper.Http;

namespace SevenDigital.Api.Wrapper.Unit.Tests.EndpointResolution.RequestHandlers
{
	[TestFixture]
	public class PostRequestHandlerTests
	{
		private IApiUri _apiUri;
		private IOAuthCredentials _oAuthCredentials;
		private ISignatureGenerator _signatureGenerator;
		private IHttpClient _httpClient;

		private PostRequestHandler _handler;

		[SetUp]
		public void Setup()
		{
			_apiUri = A.Fake<IApiUri>();
			A.CallTo(() => _apiUri.Uri).Returns("http://testuri.com/");
			A.CallTo(() => _apiUri.SecureUri).Returns("https://securetesturi.com/");

			_oAuthCredentials = A.Fake<IOAuthCredentials>();
			A.CallTo(() => _oAuthCredentials.ConsumerKey).Returns("testkey");
			A.CallTo(() => _oAuthCredentials.ConsumerSecret).Returns("testsecret");

			_signatureGenerator = A.Fake<ISignatureGenerator>();

			_httpClient = A.Fake<IHttpClient>();

			_handler = new PostRequestHandler(_apiUri, _oAuthCredentials, _signatureGenerator);
			_handler.HttpClient = _httpClient;
		}

		[Test]
		public void Should_return_uri_when_ConstructEndpoint_is_called()
		{
			var data = PostRequest();

			var endpoint = _handler.ConstructEndpoint(data);

			Assert.That(endpoint, Is.Not.Empty);
			Assert.That(endpoint, Is.StringContaining("testpath"));
		}

		[Test]
		public void Should_use_non_secure_api_uri_by_default()
		{
			var data = PostRequest();

			_handler.ConstructEndpoint(data);

			A.CallTo(() => _apiUri.Uri).MustHaveHappened();
			A.CallTo(() => _apiUri.SecureUri).MustNotHaveHappened();
		}

		[Test]
		public void Should_use_secure_uri_when_requested()
		{
			var data = PostRequest();
			data.UseHttps = true;

			_handler.ConstructEndpoint(data);

			A.CallTo(() => _apiUri.Uri).MustNotHaveHappened();
			A.CallTo(() => _apiUri.SecureUri).MustHaveHappened();
		}

		[Test]
		public void Should_not_put_oauth_data_on_constructed_endpoint()
		{
			var data = PostRequest();

			_handler.ConstructEndpoint(data);

			A.CallTo(() => _oAuthCredentials.ConsumerKey).MustNotHaveHappened();
			A.CallTo(() => _oAuthCredentials.ConsumerSecret).MustNotHaveHappened();
		}

		[Test]
		public void Should_not_sign_constructed_endpoint()
		{
			var data = PostRequest();

			_handler.ConstructEndpoint(data);

			A.CallTo(() => _signatureGenerator.Sign(A<OAuthSignatureInfo>.Ignored)).MustNotHaveHappened();
		}

		[Test]
		public void Should_sign_request_when_hit_endpoint()
		{
			var data = PostRequest();

			_handler.HitEndpoint(data);

			A.CallTo(() => _signatureGenerator.SignWithPostData(A<OAuthSignatureInfo>.Ignored)).MustHaveHappened();
		}

		[Test]
		public void Should_use_http_client_to_hit_endpoint()
		{
			var data = PostRequest();

			_handler.HitEndpoint(data);

			A.CallTo(() => _httpClient.Post(A<PostRequest>.Ignored)).MustHaveHappened();
		}

		private static RequestData PostRequest()
		{
			return new RequestData
			{
				HttpMethod = "POST",
				UriPath = "testpath",
				UseHttps = false,
				IsSigned = true
			};
		}
	}
}