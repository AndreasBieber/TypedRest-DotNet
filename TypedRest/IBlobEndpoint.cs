﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents a single binary blob that can downloaded and uploaded.
    /// </summary>
    public interface IBlobEndpoint : IEndpoint
    {
        /// <summary>
        /// Queries the server about capabilities of the endpoint without performing any action.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task ProbeAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Shows whether the server has indicated that <seealso cref="DownloadAsync"/> is currently allowed.
        /// </summary>
        /// <remarks>Uses cached data from last response.</remarks>
        /// <returns>An indicator whether the method is allowed. If no request has been sent yet or the server did not specify allowed methods <c>null</c> is returned.</returns>
        bool? DownloadAllowed { get; }

        /// <summary>
        /// Downloads the blob's content.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>A stream with the blob's content.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<Stream> DownloadAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Shows whether the server has indicated that <seealso cref="UploadFromAsync"/> is currently allowed.
        /// </summary>
        /// <remarks>Uses cached data from last response.</remarks>
        /// <returns>An indicator whether the method is allowed. If no request has been sent yet or the server did not specify allowed methods <c>null</c> is returned.</returns>
        bool? UploadAllowed { get; }

        /// <summary>
        /// Uploads content as the blob's content.
        /// </summary>
        /// <param name="stream">The stream to read the upload data from.</param>
        /// <param name="mimeType">The MIME type of the data to upload.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task UploadFromAsync(Stream stream, string mimeType = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Shows whether the server has indicated that <seealso cref="DeleteAsync"/> is currently allowed.
        /// </summary>
        /// <remarks>Uses cached data from last response.</remarks>
        /// <returns>An indicator whether the method is allowed. If no request has been sent yet or the server did not specify allowed methods <c>null</c> is returned.</returns>
        bool? DeleteAllowed { get; }

        /// <summary>
        /// Deletes the blob from the server.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException"><see cref="HttpStatusCode.Conflict"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}