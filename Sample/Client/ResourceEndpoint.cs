﻿using System;
using TypedRest;
using TypedRestSample.Model;

namespace TypedRestSample.Client
{
    /// <summary>
    /// REST endpoint that represents a <see cref="Resource"/>.
    /// </summary>
    public class ResourceEndpoint : ElementEndpoint<Resource>
    {
        public ResourceEndpoint(ResourceCollectionEndpoint referrer, Uri relativeUri)
            : base(referrer, relativeUri)
        {
        }

        /// <summary>
        /// Represents the <see cref="ResourceRevision"/>s.
        /// </summary>
        public ResourceRevisionCollectionEndpoint Revisions => new ResourceRevisionCollectionEndpoint(this);

        /// <summary>
        /// Exposes all <see cref="LogEvent"/>s that relate to this resource.
        /// </summary>
        public IStreamEndpoint<LogEvent> Events => new StreamEndpoint<LogEvent>(this, Link("events"));
    }
}