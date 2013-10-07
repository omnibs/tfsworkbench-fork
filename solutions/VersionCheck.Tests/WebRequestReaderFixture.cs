// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebRequestReaderFixture.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   The web request reader test fixture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.VersionCheck.Services;

    /// <summary>
    /// The web request reader test fixture.
    /// </summary>
    [TestFixture]
    public class WebRequestReaderFixture
    {
        /// <summary>
        /// The test edn point.
        /// </summary>
        private const string TestEndPoint = "http://dummy";

        /// <summary>
        /// Calling the constructor, with a single null parameter, throws an exception.
        /// </summary>
        [Test]
        public void Constructor_WithSingleNullParamter_ThrowsException()
        {
            // Arrange

            // Act
            try
            {
                new WebRequestReader(null);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Calling the constructor, with a single non url parameter, throws an exception.
        /// </summary>
        [Test]
        public void Constructor_WithSingleNonUrlParamter_ThrowsException()
        {
            // Arrange
            const string InvalidUrl = "NotAUrl";

            // Act
            try
            {
                new WebRequestReader(InvalidUrl);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentException)
            {
            }

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Calling the constructor, with a single valid url parameter, creates a valid instance.
        /// </summary>
        [Test]
        public void Constructor_WithSingleValidUrlParamter_CreatesAValidInstance()
        {
            // Arrange
            const string ValidUrl = @"topic://host/path";

            // Act
            var webRequestReader = new WebRequestReader(ValidUrl);

            // Assert
            webRequestReader.ShouldNotBeNull();
        }

        /// <summary>
        /// Constructor, with single valid URL paramter, creates instance with default request creator.
        /// </summary>
        [Test]
        public void Constructor_WithSingleValidUrlParamter_CreatesInstanceWithDefaultRequestCreator()
        {
            // Arrange
            const string EndPoint = "http://host/path";
            var webRequestReader = new WebRequestReader(EndPoint);

            // Act
            var webRequest = webRequestReader.WebRequestCreator.Create(new Uri(EndPoint));

            // Assert
            webRequest.ShouldNotBeNull();
        }

        /// <summary>
        /// Calling the constructor, with two valid parameters, creates a valid instance.
        /// </summary>
        [Test]
        public void Constructor_WithTwoValidParamters_CreatesAValidInstance()
        {
            // Arrange
            const string ValidUrl = @"topic://host/path";
            var requestor = MockRepository.GenerateStub<IWebRequestCreate>();

            // Act
            var webRequestReader = new WebRequestReader(ValidUrl, requestor);

            // Assert
            webRequestReader.ShouldNotBeNull();
        }

        /// <summary>
        /// Calling the constructor, with a null second parameter, throws an exception.
        /// </summary>
        [Test]
        public void Constructor_WithNullSecondParamter_ThrowsException()
        {
            // Arrange
            const string ValidUrl = @"topic://host/path";

            // Act
            try
            {
                new WebRequestReader(ValidUrl, null);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Try read first line, with valid web response, returns true.
        /// </summary>
        [Test]
        public void TryGetFirstLine_WithValidWebResponse_ReturnsTrue()
        {
            // Arrange
            var webResponseCreator = GetWebRequestCreator("Response");
            var webResponseReader = new WebRequestReader(TestEndPoint, webResponseCreator);
            string response;

            // Act
            var result = webResponseReader.TryReadFirstLine(out response);

            // Assert
            result.ShouldBeTrue();
        }

        /// <summary>
        /// Try read first line, with valid web response, sets response to first line of returned value.
        /// </summary>
        [Test]
        public void TryGetFirstLine_WithValidWebResponse_SetsResponseToFirstLineOfReturnedValue()
        {
            // Arrange
            const string WebResponse = "Expected Response\r\nNext Line";
            var expectedResponse = WebResponse.Substring(0, WebResponse.IndexOf("\r\n"));

            var webResponseCreator = GetWebRequestCreator(WebResponse);
            var webResponseReader = new WebRequestReader(TestEndPoint, webResponseCreator);
            string response;

            // Act
            webResponseReader.TryReadFirstLine(out response);

            // Assert
            response.ShouldEqual(expectedResponse);
        }

        /// <summary>
        /// Try read first line, with valid web response, calls close on web request.
        /// </summary>
        [Test]
        public void TryGetFirstLine_WithValidWebResponse_CallsCloseOnWebRequest()
        {
            // Arrange
            const string WebResponseText = "Expected Response";
            string response;

            var webResponse = GetWebResponse(WebResponseText);
            webResponse.Expect(wr => wr.Close()).Repeat.Once();

            // Act
            new WebRequestReader(TestEndPoint, GetWebRequestCreator(webResponse)).TryReadFirstLine(out response);

            // Assert
            webResponse.VerifyAllExpectations();
        }

        /// <summary>
        /// Try read first line, when web exception thrown, returns false.
        /// </summary>
        [Test]
        public void TryGetFirstLine_WhenWebExceptionThrown_ReturnsFalse()
        {
            // Arrange
            var webResponseCreator = GetWebRequestCreator(new WebException());
            var webResponseReader = new WebRequestReader(TestEndPoint, webResponseCreator);
            string response;

            // Act
            var result = webResponseReader.TryReadFirstLine(out response);

            // Assert
            result.ShouldBeFalse();
        }

        /// <summary>
        /// Try read first line, when web exception thrown, sets response to exception message.
        /// </summary>
        [Test]
        public void TryGetFirstLine_WhenWebExceptionThrown_SetsResponseToExceptionMessage()
        {
            // Arrange
            const string ErrorMessage = "Invalid request";
            var webResponseCreator = GetWebRequestCreator(new WebException(ErrorMessage));
            var webResponseReader = new WebRequestReader(TestEndPoint, webResponseCreator);
            string response;

            // Act
            webResponseReader.TryReadFirstLine(out response);

            // Assert
            response.ShouldEqual(ErrorMessage);
        }

        /// <summary>
        /// Try get first line, when exception other than web exception thrown, throws exception to caller.
        /// </summary>
        [Test]
        public void TryGetFirstLine_WhenExceptionOtherThanWebExceptionThrown_ThrowsExceptionToCaller()
        {
            // Arrange
            var exceptionToThrow = new Exception();
            Exception thrownException = null;
            var webResponseCreator = GetWebRequestCreator(exceptionToThrow);
            var webResponseReader = new WebRequestReader(TestEndPoint, webResponseCreator);

            // Act
            try
            {
                string response;
                webResponseReader.TryReadFirstLine(out response);
                Assert.Fail("Exception not thrown");
            }
            catch (Exception ex)
            {
                thrownException = ex;
            }

            // Assert
            thrownException.ShouldNotBeNull();
            thrownException.ShouldEqual(exceptionToThrow);
        }

        /// <summary>
        /// Try read first line, with valid web response, requests the specified uri.
        /// </summary>
        [Test]
        public void TryGetFirstLine_WithValidWebResponse_RequestsTheSpecifiedUrl()
        {
            // Arrange
            var webResponseCreator = GetWebRequestCreatorThatExpects(new Uri(TestEndPoint));
            var webResponseReader = new WebRequestReader(TestEndPoint, webResponseCreator);
            string response;

            // Act
            webResponseReader.TryReadFirstLine(out response);

            // Assert
            webResponseCreator.VerifyAllExpectations();
        }

        /// <summary>
        /// Gets the web request creator.
        /// </summary>
        /// <param name="responseString">The required response.</param>
        /// <returns>A mock instance of web request create.</returns>
        private static IWebRequestCreate GetWebRequestCreator(string responseString)
        {
            var webRequestCreator = MockRepository.GenerateMock<IWebRequestCreate>();

            webRequestCreator
                .Expect(wrc => wrc.Create(null))
                .IgnoreArguments()
                .Return(GetWebRequest(GetWebResponse(responseString)));

            return webRequestCreator;
        }

        /// <summary>
        /// Gets the web request creator that expects response close.
        /// </summary>
        /// <param name="webResponse">The web response.</param>
        /// <returns>A mock instance of web request create.</returns>
        private static IWebRequestCreate GetWebRequestCreator(WebResponse webResponse)
        {
            var webRequestCreator = MockRepository.GenerateMock<IWebRequestCreate>();

            webRequestCreator
                .Expect(wrc => wrc.Create(null))
                .IgnoreArguments()
                .Return(GetWebRequest(webResponse));

            return webRequestCreator;
        }

        /// <summary>
        /// Gets the web request creator.
        /// </summary>
        /// <param name="expectedUri">The expected URI.</param>
        /// <returns>A mock instance of web request create.</returns>
        private static IWebRequestCreate GetWebRequestCreatorThatExpects(Uri expectedUri)
        {
            var webRequestCreator = MockRepository.GenerateMock<IWebRequestCreate>();

            webRequestCreator
                .Expect(wrc => wrc.Create(null))
                .IgnoreArguments()
                .Constraints(Rhino.Mocks.Constraints.Is.Equal(expectedUri))
                .Return(GetWebRequest(GetWebResponse(string.Empty)))
                .Repeat.Once();

            return webRequestCreator;
        }

        /// <summary>
        /// Gets the web request creator.
        /// </summary>
        /// <param name="exceptionToThrow">The exception to throw.</param>
        /// <returns>A mock instance of web request create.</returns>
        private static IWebRequestCreate GetWebRequestCreator(Exception exceptionToThrow)
        {
            var webRequestCreator = MockRepository.GenerateMock<IWebRequestCreate>();

            webRequestCreator
                .Expect(wrc => wrc.Create(null))
                .IgnoreArguments()
                .Return(GetWebRequest(exceptionToThrow));

            return webRequestCreator;
        }

        /// <summary>
        /// Gets the web request.
        /// </summary>
        /// <param name="webResponse">The web response.</param>
        /// <returns>And instance of the web request class.</returns>
        private static WebRequest GetWebRequest(WebResponse webResponse)
        {
            var webRequest = MockRepository.GenerateMock<WebRequest>();
            webRequest
                .Expect(wr => wr.GetResponse())
                .Return(webResponse);
            return webRequest;
        }

        /// <summary>
        /// Gets the web request.
        /// </summary>
        /// <param name="exceptionToThrow">The exception to throw.</param>
        /// <returns>And instance of the web request class.</returns>
        private static WebRequest GetWebRequest(Exception exceptionToThrow)
        {
            var webRequest = MockRepository.GenerateMock<WebRequest>();
            webRequest
                .Expect(wr => wr.GetResponse())
                .Throw(exceptionToThrow);

            return webRequest;
        }

        /// <summary>
        /// Gets the web response.
        /// </summary>
        /// <param name="requiredResponse">The required response.</param>
        /// <returns>An instance of the web response class.</returns>
        private static WebResponse GetWebResponse(string requiredResponse)
        {
            var webResponse = MockRepository.GenerateMock<WebResponse>();
            webResponse
                .Expect(wr => wr.GetResponseStream())
                .Return(new MemoryStream(Encoding.UTF8.GetBytes(requiredResponse)));

            return webResponse;
        }
    }
}
