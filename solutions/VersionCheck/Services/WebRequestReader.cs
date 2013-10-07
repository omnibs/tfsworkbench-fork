// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebRequestReader.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Services
{
    using System;
    using System.Net;

    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Properties;

    /// <summary>
    /// The web request reader class.
    /// </summary>
    internal class WebRequestReader : IWebRequestReader
    {
        /// <summary>
        /// The endpoint.
        /// </summary>
        private readonly Uri endPoint;

        /// <summary>
        /// The web request creator.
        /// </summary>
        private readonly IWebRequestCreate webRequestCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestReader"/> class.
        /// </summary>
        /// <param name="requestEndPoint">The request end point.</param>
        public WebRequestReader(string requestEndPoint)
            : this(requestEndPoint, new DefaultWebRequestWrapper())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequestReader"/> class.
        /// </summary>
        /// <param name="requestEndPoint">The request end point.</param>
        /// <param name="webRequestCreator">The web request creator.</param>
        public WebRequestReader(string requestEndPoint, IWebRequestCreate webRequestCreator)
        {
            if (!Helpers.IsNotNull(requestEndPoint))
            {
                throw new ArgumentNullException("requestEndPoint");
            }

            if (!Helpers.IsNotNull(webRequestCreator))
            {
                throw new ArgumentNullException("webRequestCreator");
            }

            if (!Uri.TryCreate(requestEndPoint, UriKind.Absolute, out this.endPoint))
            {
                throw new ArgumentException(Resources.String014);
            }

            this.webRequestCreator = webRequestCreator;
        }

        /// <summary>
        /// Gets the end point URI.
        /// </summary>
        /// <value>The end point URI.</value>
        /// <remarks>Exposed for testing.</remarks>
        public Uri EndPointUri
        {
            get { return this.endPoint; }
        }

        /// <summary>
        /// Gets the web request creator.
        /// </summary>
        /// <value>The web request creator.</value>
        /// <remarks>Exposed for testing.</remarks>
        public IWebRequestCreate WebRequestCreator
        {
            get { return this.webRequestCreator; }
        }

        /// <summary>
        /// Tries to read the first line of the response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns><c>True</c> if the first line is read; otherwise <c>false</c>.</returns>
        public bool TryReadFirstLine(out string response)
        {
            try
            {
                return Helpers.IsNotNull(response = this.GetWebResponse().ReadFirstLine());
            }
            catch (WebException webEx)
            {
                response = webEx.Message;
                return false;
            }
        }

        /// <summary>
        /// Tries to get the web response from end point.
        /// </summary>
        /// <returns><c>True</c> if a response is returned; ortherwise <c>false</c>.</returns>
        private WebResponse GetWebResponse()
        {
            return this.webRequestCreator.Create(this.endPoint).GetResponse();
        }

        /// <summary>
        /// The default web request proxy class.
        /// </summary>
        private class DefaultWebRequestWrapper : IWebRequestCreate
        {
            /// <summary>
            /// Creates a <see cref="T:System.Net.WebRequest"/> instance.
            /// </summary>
            /// <param name="uri">The uniform resource identifier (URI) of the Web resource.</param>
            /// <returns>
            /// A <see cref="T:System.Net.WebRequest"/> instance.
            /// </returns>
            /// <exception cref="T:System.NotSupportedException">The request scheme specified in <paramref name="uri"/> is not supported by this <see cref="T:System.Net.IWebRequestCreate"/> instance. </exception>
            /// <exception cref="T:System.ArgumentNullException"> <paramref name="uri"/> is null. </exception>
            /// <exception cref="T:System.UriFormatException">The URI specified in <paramref name="uri"/> is not a valid URI. </exception>
            public WebRequest Create(Uri uri)
            {
                return WebRequest.Create(uri);
            }
        }
    }
}