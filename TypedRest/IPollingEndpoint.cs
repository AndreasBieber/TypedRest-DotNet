﻿using System;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents an entity that can be polled for state changes.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public interface IPollingEndpoint<TEntity> : IElementEndpoint<TEntity>
    {
        /// <summary>
        /// The interval in which to send requests to the server.
        /// The server can modify this value using the "Retry-After" header.
        /// </summary>
        TimeSpan PollingInterval { get; set; }

        /// <summary>
        /// Provides an observable stream of element states. Compares entities using <see cref="object.Equals(object)"/> to detect changes.
        /// </summary>
        IObservable<TEntity> GetStream();
    }
}