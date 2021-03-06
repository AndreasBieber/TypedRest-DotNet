﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;

namespace TypedRest.Wpf
{
    /// <summary>
    /// Builds <see cref="EntryEndpoint"/>s using config files, interactive authentication, OAuth tokens, etc.
    /// </summary>
    /// <typeparam name="T">The type of entry endpoint created. Must have constructors with the following signatures: (<see cref="Uri"/>, <see cref="ICredentials"/>) for HTTP Basic Auth and (<see cref="Uri"/>, <see cref="string"/>) for OAuth token.</typeparam>
    public class EndpointProvider<T> : EndpointProviderBase<T>
        where T : EntryEndpoint
    {
        protected override Uri RequestUri()
        {
            Uri uri = null;
            while (uri == null)
            {
                string input = InputBox.Show("Endpoint URI:");
                if (input == null) return null;
                try
                {
                    uri = new Uri(input, UriKind.Absolute);
                }
                catch (UriFormatException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return uri;
        }

        protected override string RequestToken(Uri uri)
        {
            ShowTokenProvider(uri);

            return InputBox.Show("OAuth token:");
        }

        /// <summary>
        /// Tries to determine a website that provides tokens for <paramref name="uri"/> and opens it in the default browser.
        /// </summary>
        protected virtual void ShowTokenProvider(Uri uri)
        {
            var endpoint = NewEndpoint(uri, new NetworkCredential());
            try
            {
                Process.Start(endpoint.Link("token-provider").AbsoluteUri);
            }
            catch (KeyNotFoundException)
            {
            }
        }
    }
}