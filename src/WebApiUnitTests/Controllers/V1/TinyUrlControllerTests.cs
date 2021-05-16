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

        public static TheoryData<string, string> InValidUrlTestData()
        {
            return new TheoryData<string, string>()
            {
                { null, "Error: The URL field is required." },
                { string.Empty, "Error: The URL field is required." },
                { " ", "Error: The URL field is required." },
                { new string('a', 5000) , "Error: The URL is too long" },
                { "http:InvalidUrl.com", "Error: Invalid URL" },
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
                .ThenIShouldExpectResponseContentBodyToMatchAsync(longUrl)
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
                .ThenIShouldExpectResponseContentBodyToMatchAsync(tinyUrl)
                .ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(InValidUrlTestData))]
        public async Task GetAsync_InValidParameters_ReturnsBadRequest(string tinyUrl, string expectedErrorMessage)
        {
            await this.steps
                .WhenIRequestGetAsync(tinyUrl)
                .ConfigureAwait(false);

            await this.steps
                .ThenIShouldExpecHttpResponseStatusCodeToBe(HttpStatusCode.BadRequest)
                .ThenIShouldExpectResponseContentBodyToMatchAsync(expectedErrorMessage)
                .ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(InValidUrlTestData))]
        public async Task PostAsync_InValidParameters_ReturnsBadRequest(string longUrl, string expectedErrorMessage)
        {
            await this.steps
                .WhenIRequestPostAsync(longUrl)
                .ConfigureAwait(false);

            await this.steps
                .ThenIShouldExpecHttpResponseStatusCodeToBe(HttpStatusCode.BadRequest)
                .ThenIShouldExpectResponseContentBodyToMatchAsync(expectedErrorMessage)
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task GetAsync_WithLongUrl_ReturnsBadRequest()
        {
            var longUrl = "http://foo";
            var expectedErrorMessage = "Error: Invalid tiny Url";

            await this.steps
                .GivenTinyUrlServiceChecksIsTinyUrl(false)
                .WhenIRequestGetAsync(longUrl)
                .ConfigureAwait(false);

            await this.steps
                .ThenIShouldExpecHttpResponseStatusCodeToBe(HttpStatusCode.BadRequest)
                .ThenIShouldExpectResponseContentBodyToMatchAsync(expectedErrorMessage)
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task PostAsync_WithTinyUrl_ReturnsBadRequest()
        {
            var tinyUrl = "http://ti.ny";
            var expectedErrorMessage = "Error: URL domain banned";

            await this.steps
                .GivenTinyUrlServiceChecksIsTinyUrl(true)
                .WhenIRequestPostAsync(tinyUrl)
                .ConfigureAwait(false);

            await this.steps
                .ThenIShouldExpecHttpResponseStatusCodeToBe(HttpStatusCode.BadRequest)
                .ThenIShouldExpectResponseContentBodyToMatchAsync(expectedErrorMessage)
                .ConfigureAwait(false);
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