namespace TinyUrl.WebApi.UnitTests.Controllers.V1.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using TinyUrl.WebApi.Controllers.V1;
    using Xunit;

    public class TinyUrlControllerTests : IDisposable
    {
        private readonly TinyUrlControllerSteps steps = new TinyUrlControllerSteps();

        private bool isDisposed;

        public static IEnumerable<object[]> InValidTinyUrlTestData()
        {
            return new TheoryData<string>()
            {
                { string.Empty },
                { null },
                { new string('a', 5000) },
                { "http:InvalidUrl.com" },
            };
        }

        public static IEnumerable<object[]> InValidLongUrlTestData()
        {
            return new TheoryData<string>()
            {
                { string.Empty },
                { null },
                { new string('a', 5000) },
                { "http:InvalidUrl.com" },
            };
        }

        [Fact]
        public void TinyUrlController_InitializeNullObject_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new TinyUrlController(null));
        }

        [Theory]
        [InlineData("http://tiny.ul/aX2c", "http://foo.com/testUrl?q=abc")]
        public async Task GetAsync_ValidParameters_Success(string tinyUrl, string longUrl)
        {
            await this.steps
                .GivenTinyUrlServiceChecksIsTinyUrl(true)
                .GivenTinyUrlServiceGetOriginalUrl(longUrl)
                .WhenIRequestGetAsync(tinyUrl)
                .ConfigureAwait(false);

            await this.steps
                .ThenIShouldExpecHttpResponseStatusCodeToBe(HttpStatusCode.OK)
                .ThenIShouldExpectResponseContentBodyToMatch(longUrl)
                .ConfigureAwait(false);
        }

        [Theory]
        [InlineData("http://foo.com/testUrl?q=abc", "http://tiny.ul/aX2c")]
        public async Task PostAsync_ValidParameters_Success(string longUrl, string tinyUrl)
        {
            await this.steps
                .GivenTinyUrlServiceChecksIsTinyUrl(false)
                .GivenTinyUrlServiceGetTinyUrlAsync(tinyUrl)
                .WhenIRequestPostAsync(longUrl)
                .ConfigureAwait(false);

            await this.steps
                .ThenIShouldExpecHttpResponseStatusCodeToBe(HttpStatusCode.OK)
                .ThenIShouldExpectResponseContentBodyToMatch(tinyUrl)
                .ConfigureAwait(false);
        }

        [Theory]
        [InlineData(nameof(InValidTinyUrlTestData))]
        public async Task GetAsync_InValidParameters_ReturnsBadRequest(string tinyUrl)
        {
            await this.steps
                .WhenIRequestGetAsync(tinyUrl)
                .ConfigureAwait(false);

            this.steps
              .ThenIShouldExpecHttpResponseStatusCodeToBe(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(nameof(InValidLongUrlTestData))]
        public async Task PostAsync_InValidParameters_ReturnsBadRequest(string longUrl)
        {
            await this.steps
                .WhenIRequestPostAsync(longUrl)
                .ConfigureAwait(false);

            this.steps
              .ThenIShouldExpecHttpResponseStatusCodeToBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAsync_WithLongUrl_ReturnsBadRequest()
        {
            var longUrl = "http://foo";
            await this.steps
                .GivenTinyUrlServiceChecksIsTinyUrl(false)
                .WhenIRequestGetAsync(longUrl)
                .ConfigureAwait(false);

            this.steps
              .ThenIShouldExpecHttpResponseStatusCodeToBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostAsync_WithTinyUrl_ReturnsBadRequest()
        {
            var tinyUrl = "http://ti.ny";
            await this.steps
                .GivenTinyUrlServiceChecksIsTinyUrl(true)
                .WhenIRequestPostAsync(tinyUrl)
                .ConfigureAwait(false);

            this.steps
              .ThenIShouldExpecHttpResponseStatusCodeToBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAsync_ValidTinyUrlButDoesNotExists_ReturnsNotFound()
        {
            var tinyUrl = "http://ti.ny";
            await this.steps
                .GivenTinyUrlServiceChecksIsTinyUrl(true)
                .GivenTinyUrlServiceGetOriginalUrl(string.Empty)
                .WhenIRequestGetAsync(tinyUrl)
                .ConfigureAwait(false);

            this.steps
              .ThenIShouldExpecHttpResponseStatusCodeToBe(HttpStatusCode.NotFound);
        }

        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.steps?.Dispose();
            this.isDisposed = true;
        }
    }
}