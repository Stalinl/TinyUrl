namespace TinyUrl.WebApi.UnitTests.Controllers.V1.UnitTests
{
    using System;

    using TinyUrl.WebApi.Controllers.V1;
    using Xunit;

    public class TinyUrlControllerTests
    {
        [Fact]
        public void TinyUrlController_InitializeNullObject_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new TinyUrlController(null));
        }
    }
}