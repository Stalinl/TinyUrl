namespace TinyUrl.WebApi.UnitTests.Controllers.V1.UnitTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    using FluentAssertions;
    using Moq;
    using Newtonsoft.Json;
    using TinyUrl.Core;
    using TinyUrl.WebApi.Controllers.V1;

    public sealed class TinyUrlControllerSteps
    {
        private readonly Mock<ITinyUrlService> tinyUrlService;
        private readonly TinyUrlController controller;
        private readonly Mock<HttpRequestMessage> mockHttpRequest;

        private bool isDisposed;
        private HttpResponseMessage httpResponse;

        public TinyUrlControllerSteps()
        {
            this.tinyUrlService = new Mock<ITinyUrlService>();
            this.mockHttpRequest = new Mock<HttpRequestMessage>();
            this.controller = new TinyUrlController(this.tinyUrlService.Object)
            {
                Request = mockHttpRequest.Object,
                Configuration = new HttpConfiguration(),
            };
        }

        public TinyUrlControllerSteps GivenTinyUrlServiceChecksIsTinyUrl(bool isTinyUrl)
        {
            this.tinyUrlService
                .Setup(s => s.IsTinyUrl(It.IsAny<Uri>()))
                .Returns(isTinyUrl);

            return this;
        }

        public TinyUrlControllerSteps GivenTinyUrlServiceGetOriginalUrl(string originalUrl)
        {
            this.tinyUrlService
                .Setup(s => s.GetOriginalUrlAsync(It.IsAny<Uri>()))
                .ReturnsAsync(originalUrl);

            return this;
        }

        public TinyUrlControllerSteps GivenTinyUrlServiceGetTinyUrlAsync(string tinyUrl)
        {
            this.tinyUrlService
                .Setup(s => s.GetTinyUrlAsync(It.IsAny<Uri>()))
                .ReturnsAsync(tinyUrl);

            return this;
        }

        public async Task<TinyUrlControllerSteps> WhenIRequestGetAsync(string url)
        {
            var actionResult = await this.controller.GetAsync(url).ConfigureAwait(false);
            this.httpResponse = await actionResult.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);

            return this;
        }

        public async Task<TinyUrlControllerSteps> WhenIRequestPostAsync(string url)
        {
            var actionResult = await this.controller.PostAsync(url).ConfigureAwait(false);
            this.httpResponse = await actionResult.ExecuteAsync(CancellationToken.None).ConfigureAwait(false);

            return this;
        }

        public TinyUrlControllerSteps ThenIShouldExpecHttpResponseStatusCodeToBe(HttpStatusCode expected)
        {
            this.httpResponse.StatusCode.Should().Be(expected);
            return this;
        }

        public async Task<TinyUrlControllerSteps> ThenIShouldExpectResponseContentBodyToMatchAsync(string expected)
        {
            this.httpResponse.Content.Should().NotBeNull();

            var content = await this.httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            content.Should().NotBeNull();
            content.Should().Contain(expected);
            return this;
        }

        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.httpResponse?.Dispose();
            this.isDisposed = true;
        }
    }
}