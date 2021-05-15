namespace TinyUrl.Core.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Moq;
    using Xunit;

    public sealed class TinyUrlServiceTests
    {
        private const int RepositoryExistingRecordId = 1000;
        private const string RepositoryExstingRecordIdEncodedValue = "MTAwMA==";

        private readonly TinyUrlServiceSteps steps = new TinyUrlServiceSteps();

        public static IEnumerable<object[]> ValidTinyUrlTestData()
        {
            return new TheoryData<Uri>()
            {
                { TinyUrlServiceSteps.TinyUrlBaseAddress },
                { new UriBuilder(TinyUrlServiceSteps.TinyUrlBaseAddress){ Path ="something" }.Uri }
            };
        }


        [Theory]
        [InlineData(true, false, typeof(ArgumentNullException))]
        [InlineData(false, true, typeof(ArgumentNullException))]
        public void Constructor_InvalidParameter_ThrowsException(bool isNullCreditBalanceEndpoint, bool isNullLogger, Type expectedExceptionType)
        {
            this.steps
                .WhenIInitialize(isNullCreditBalanceEndpoint, isNullLogger)
                .ThenIShouldExpectExceptionToBeThrown(expectedExceptionType);
        }

        [Fact]
        public void IsTinyUrl_NonTinyUrl_ReturnsFalse()
        {
            var nonTinyUrl = new Uri("https://nontinyUrl.com");

            this.steps
                 .GivenIHaveTinyUrlService()
                 .WhenICallIsTinyUrl(nonTinyUrl)
                 .ThenTheExpectedResultShouldBe(false);
        }

        [Theory]
        [MemberData(nameof(ValidTinyUrlTestData))]
        public void IsTinyUrl_TinyUrl_ReturnsTrue(Uri tinyUrl)
        {
            this.steps
                 .GivenIHaveTinyUrlService()
                 .WhenICallIsTinyUrl(tinyUrl)
                 .ThenTheExpectedResultShouldBe(true);
        }

        [Fact]
        public async Task GetOriginalUrl_InvalidEncodedData_ReturnsEmpty()
        {
            var tinyUri = new UriBuilder(TinyUrlServiceSteps.TinyUrlBaseAddress) { Path = "InvalidEncodedData" }.Uri;

            await this.steps
                 .GivenIHaveTinyUrlService()
                 .WhenICallGetOriginalUrlAsync(tinyUri)
                 .ConfigureAwait(false);

            this.steps
                .ThenTheExpectedResultShouldBe(string.Empty);
        }

        [Fact]
        public async Task GetOriginalUrl_ValidEncodedData_ReturnsExpected()
        {
            var tinyUri = new UriBuilder(TinyUrlServiceSteps.TinyUrlBaseAddress) { Path = RepositoryExstingRecordIdEncodedValue }.Uri;

            await this.steps
                 .GivenIHaveTinyUrlService()
                 .GivenISetupRepository(x =>
                    x.Setup(s =>
                        s.GetByIdAsync(It.IsAny<int>()))
                        .ReturnsAsync("OriginalUrl")
                        .Verifiable())
                 .WhenICallGetOriginalUrlAsync(tinyUri)
                 .ConfigureAwait(false);

            this.steps
                .ThenTheExpectedResultShouldBe("OriginalUrl")
                .ThenTheRepositoryShouldBeVerified();
        }

        [Fact]
        public async void GetTinyUrl_ExistingUrl_ReturnsExpected()
        {
            var longUri = new Uri("http://longUrl");
            var tinyUri = new UriBuilder(TinyUrlServiceSteps.TinyUrlBaseAddress) { Path = RepositoryExstingRecordIdEncodedValue }.Uri.AbsoluteUri;

            await this.steps
                 .GivenIHaveTinyUrlService()
                 .GivenISetupRepository(x =>
                    x.Setup(s =>
                        s.GetByPathAsync(It.IsAny<string>()))
                        .ReturnsAsync(RepositoryExistingRecordId)
                        .Verifiable())
                 .WhenICallGetTinyUrlAsync(longUri)
                 .ConfigureAwait(false);

            this.steps
                .ThenTheExpectedResultShouldBe(tinyUri)
                .ThenTheRepositoryShouldBeVerified();
        }

        [Fact]
        public async void GetTinyUrl_NewUrl_ReturnsExpected()
        {
            var longUri = new Uri("http://longUrl");
            var tinyUri = new UriBuilder(TinyUrlServiceSteps.TinyUrlBaseAddress) { Path = RepositoryExstingRecordIdEncodedValue }.Uri.AbsoluteUri;

            await this.steps
                 .GivenIHaveTinyUrlService()
                 .GivenISetupRepository(x =>
                    x.Setup(s =>
                        s.GetByPathAsync(It.IsAny<string>()))
                        .ReturnsAsync(0)
                        .Verifiable())
                 .GivenISetupRepository(x =>
                    x.Setup(s =>
                        s.CreateAsync(It.IsAny<string>()))
                        .ReturnsAsync(RepositoryExistingRecordId)
                        .Verifiable())
                 .WhenICallGetTinyUrlAsync(longUri)
                 .ConfigureAwait(false);

            this.steps
                .ThenTheExpectedResultShouldBe(tinyUri)
                .ThenTheRepositoryShouldBeVerified();
        }
    }
}