﻿using System;
using System.Net;
using TypedRest;
using TypedRestSample.Model;

namespace TypedRestSample.Client
{
    /// <summary>
    /// Entry point for the IRUS REST API.
    /// </summary>
    public class SampleEntryEndpoint : EntryEndpoint
    {
        public SampleEntryEndpoint(Uri uri, ICredentials credentials = null) : base(uri, credentials)
        {
        }

        public ResourceCollectionEndpoint Resources => new ResourceCollectionEndpoint(this);

        public ICollectionEndpoint<Target> Targets => new CollectionEndpoint<Target>(this, Link("targets"));
    }
}