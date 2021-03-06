﻿using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TypedRest
{
    /// <summary>
    /// Represents an HTTP Link header.
    /// </summary>
    public class LinkHeader
    {
        private static readonly Regex RegexLinkFields =
            new Regex("(?=^<(?'href'[^>]*)>)|(?'field'[a-z]+)=\"?(?'value'[^\",;]*)\"?", RegexOptions.Compiled);

        /// <summary>
        /// The href/target of the link.
        /// </summary>
        public string Href { get; }

        /// <summary>
        /// The relation type of the link.
        /// </summary>
        public string Rel { get; }

        /// <summary>
        /// The title of the link (optional).
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Indicates whether the link is an URI Template (RFC 6570).
        /// </summary>
        public bool Templated { get; }

        /// <summary>
        /// Parses a header value into an HTTP Link header.
        /// </summary>
        public LinkHeader(string value)
        {
            foreach (var match in RegexLinkFields.Matches(value).Cast<Match>())
            {
                if (Href == null)
                    Href = match.Groups["href"].Value;

                if (match.Groups["field"].Success)
                {
                    if (Rel == null && match.Groups["field"].Value.Equals("rel", StringComparison.OrdinalIgnoreCase))
                        Rel = match.Groups["value"].Value;

                    if (match.Groups["field"].Value.Equals("templated", StringComparison.OrdinalIgnoreCase))
                        Templated = match.Groups["value"].Value.Equals("true", StringComparison.OrdinalIgnoreCase);

                    if (Title == null && match.Groups["field"].Value.Equals("title", StringComparison.OrdinalIgnoreCase))
                        Title = match.Groups["value"].Value;
                }
            }
        }
    }
}