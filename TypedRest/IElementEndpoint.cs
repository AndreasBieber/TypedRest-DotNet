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
    /// REST endpoint that represents a single entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public interface IElementEndpoint<TEntity> : IEndpoint
    {
        /// <summary>
        /// Returns the specific <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<TEntity> ReadAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Determines whether the entity currently exists.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Shows whether the server has indicated that <seealso cref="SetAsync"/> is currently allowed.
        /// </summary>
        /// <remarks>Uses cached data from last response.</remarks>
        /// <returns>An indicator whether the method is allowed. If no request has been sent yet or the server did not specify allowed methods <c>null</c> is returned.</returns>
        bool? SetAllowed { get; }

        /// <summary>
        /// Sets/replaces the <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="entity">The new <typeparamref name="TEntity"/>.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The <typeparamref name="TEntity"/> as returned by the server, possibly with additional fields set. <c>null</c> if the server does not respond with a result entity.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException">The entity has changed since it was last retrieved with <see cref="ReadAsync"/>. Your changes were rejected to prevent a lost update.</exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<TEntity> SetAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        [Obsolete("Use SetAsync() instead")]
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Shows whether the server has indicated that <seealso cref="MergeAsync"/> is currently allowed.
        /// </summary>
        /// <remarks>Uses cached data from last response.</remarks>
        /// <returns>An indicator whether the method is allowed. If no request has been sent yet or the server did not specify allowed methods <c>null</c> is returned.</returns>
        bool? MergeAllowed { get; }

        /// <summary>
        /// Modifies an existing <typeparamref name="TEntity"/> by merging changes.
        /// </summary>
        /// <param name="entity">The <typeparamref name="TEntity"/> data to merge with the existing one.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The modified <typeparamref name="TEntity"/> as returned by the server, possibly with additional fields set. <c>null</c> if the server does not respond with a result entity.</returns>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException">The entity has changed since it was last retrieved with <see cref="ReadAsync"/>. Your changes were rejected to prevent a lost update.</exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task<TEntity> MergeAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Shows whether the server has indicated that <seealso cref="DeleteAsync"/> is currently allowed.
        /// </summary>
        /// <remarks>Uses cached data from last response.</remarks>
        /// <returns>An indicator whether the method is allowed. If no request has been sent yet or the server did not specify allowed methods <c>null</c> is returned.</returns>
        bool? DeleteAllowed { get; }

        /// <summary>
        /// Deletes the <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <exception cref="InvalidDataException"><see cref="HttpStatusCode.BadRequest"/></exception>
        /// <exception cref="AuthenticationException"><see cref="HttpStatusCode.Unauthorized"/></exception>
        /// <exception cref="UnauthorizedAccessException"><see cref="HttpStatusCode.Forbidden"/></exception>
        /// <exception cref="KeyNotFoundException"><see cref="HttpStatusCode.NotFound"/> or <see cref="HttpStatusCode.Gone"/></exception>
        /// <exception cref="InvalidOperationException">The entity has changed since it was last retrieved with <see cref="ReadAsync"/>. Your delete call was rejected to prevent a lost update.</exception>
        /// <exception cref="HttpRequestException">Other non-success status code.</exception>
        Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}