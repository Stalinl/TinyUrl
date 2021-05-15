namespace TinyUrl.Core.UnitTests
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using FluentAssertions;
    using Xunit;

    public sealed class UrlHelpersTests
    {
        public static TheoryData<string, Uri> TryGetUrlTestData()
        {
            return new TheoryData<string, Uri>
            {
                { "http://foo", new Uri("http://foo") },
                { "http://foo.com/abc?q=abc123%20def", new Uri("http://foo.com/abc?q=abc123%20def") },
            };
        }

        [Theory]
        [InlineData(1, "MQ==")]
        [InlineData(1000, "MTAwMA==")]
        public void HelpersEncode_ValidData_ReturnsExpected(int data, string expected)
        {
            var urlsafe = string.Empty;
            Enumerable.Range(48, 75)
              .Where(i => i < 58 || i > 64 && i < 91 || i > 96)
              .OrderBy(o => new Random().Next())
              .ToList()
              .ForEach(i => urlsafe += Convert.ToChar(i));
            var v = urlsafe.Substring(new Random().Next(0, urlsafe.Length), new Random().Next(2, 6));

            var id = Guid.NewGuid();
            var resl1 = Convert.ToBase64String(id.ToByteArray(), Base64FormattingOptions.None); //PC7Mms9ahk+yx0JhVVRtEg 22char

            var vv = GetHashBytes(id.ToString("N"));

            var result = Helpers.Encode(data);
            result.Should().Be(expected);

            byte[] GetHashBytes(string s)
            {
                var bytes = Encoding.UTF8.GetBytes(s);
                using (var sha1 = SHA1.Create())
                    return sha1.ComputeHash(bytes);
            }
        }

        [Theory]
        [InlineData("", typeof(ArgumentException))]
        [InlineData(null, typeof(ArgumentNullException))]
        public void HelpersTryDecode_InvalidParameters_ThrowsExpected(string encodedData, Type expectedType)
        {
            Action action = () => Helpers.TryDecode(encodedData, out int actual);

            Record.Exception(action).Should().BeOfType(expectedType);
        }

        [Theory]
        [InlineData("MQ==", 1)]
        [InlineData("MTAwMA==", 1000)]
        public void HelpersTryDecode_ValidData_ReturnsExpected(string encodedData, int expected)
        {
            var result = Helpers.TryDecode(encodedData, out int actual);
            result.Should().BeTrue();
            actual.Should().Be(expected);
        }


        [Theory]
        [InlineData("$MQ==!~")]
        [InlineData("#MTAwMA==!~")]
        public void HelpersTryDecode_InvalidData_ReturnsFalse(string encodedData)
        {
            Helpers.TryDecode(encodedData, out int _).Should().BeFalse();
        }


        [Theory]
        [InlineData("", typeof(ArgumentException))]
        [InlineData(null, typeof(ArgumentNullException))]
        public void HelpersTryGetUrl_InvalidParameters_ThrowsExpected(string urlString, Type expectedType)
        {
            Action action = () => Helpers.TryDecode(urlString, out int actual);

            Record.Exception(action).Should().BeOfType(expectedType);
        }

        [Theory]
        [MemberData(nameof(TryGetUrlTestData))]
        public void HelpersTryGetUrl_ValidData_ReturnsExpected(string urlString, Uri expectedUri)
        {
            var result = Helpers.TryGetUrl(urlString, out Uri actual);
            result.Should().BeTrue();
            actual.Should().Be(expectedUri);
        }

        [Theory]
        [InlineData("invalid url")]
        [InlineData("http:foo.com")]
        public void HelpersTryGetUrl_InvalidData_ReturnsFalse(string urlString)
        {
            Helpers.TryGetUrl(urlString, out Uri _).Should().BeFalse();
        }
    }
}