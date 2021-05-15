namespace TinyUrl.Core.UnitTests
{
    using System;
    using System.Threading.Tasks;

    using FluentAssertions;
    using Moq;
    using Xunit;

    internal sealed class TinyUrlServiceSteps
    {
        public static Uri TinyUrlBaseAddress = new Uri("https://tiny.ul/");

        private readonly Mock<IRepository> repository = new Mock<IRepository>();

        private object result;
        private TinyUrlService target;
        private Exception recordedException;

        public TinyUrlServiceSteps GivenIHaveTinyUrlService()
        {
            this.target = new TinyUrlService(this.repository.Object, TinyUrlBaseAddress);

            return this;
        }

        public TinyUrlServiceSteps GivenISetupRepository(Action<Mock<IRepository>> setup)
        {
            setup(this.repository);

            return this;
        }

        public TinyUrlServiceSteps WhenIInitialize(bool isNullRepository, bool isNullTinyUrlBaseAddress)
        {
            var repository = isNullRepository ? null : Mock.Of<IRepository>();
            var tinyUrlBaseAddress = isNullTinyUrlBaseAddress ? null : TinyUrlBaseAddress;

            this.recordedException = Record.Exception(() => new TinyUrlService(repository, tinyUrlBaseAddress));

            return this;
        }

        public TinyUrlServiceSteps WhenICallIsTinyUrl(Uri uri)
        {
            this.result = this.target.IsTinyUrl(uri);

            return this;
        }

        public async Task<TinyUrlServiceSteps> WhenICallGetTinyUrlAsync(Uri uri)
        {
            this.result = await this.target.GetTinyUrlAsync(uri).ConfigureAwait(false);

            return this;
        }

        public async Task<TinyUrlServiceSteps> WhenICallGetOriginalUrlAsync(Uri uri)
        {
            this.result = await this.target.GetOriginalUrlAsync(uri).ConfigureAwait(false);

            return this;
        }

        public TinyUrlServiceSteps ThenIShouldExpectExceptionToBeThrown(Type expectedType)
        {
            this.recordedException.Should().BeOfType(expectedType);

            return this;
        }

        public TinyUrlServiceSteps ThenTheExpectedResultShouldBe<T>(T value)
        {
            this.result.Should().BeOfType<T>();
            this.result.Should().Be(value);

            return this;
        }

        public TinyUrlServiceSteps ThenTheRepositoryShouldBeVerified()
        {
            this.repository.Verify();

            return this;
        }
    }
}