﻿// Resta.UriTemplates
// Copyright(c) 2013 Pavel Shkarin
// Source: https://github.com/a7b0/uri-templates
// License: The MIT License(MIT)

using System;
using System.Collections.Generic;

namespace TypedRest.UriTemplates
{
    public sealed class UriTemplateResolver
    {
        private readonly UriTemplate template;
        private readonly Dictionary<string, object> variables;

        internal UriTemplateResolver(UriTemplate template)
        {
            this.template = template;
            this.variables = new Dictionary<string, object>(StringComparer.Ordinal);
        }

        public string Resolve()
        {
            return template.Resolve(variables);
        }

        public UriTemplate ResolveTemplate()
        {
            return template.ResolveTemplate(variables);
        }

        public UriTemplateResolver Bind(string name, string value)
        {
            return BindVariable(name, value);
        }

        public UriTemplateResolver Bind(string name, IEnumerable<string> values)
        {
            return BindVariable(name, values);
        }

        public UriTemplateResolver Bind(string name, IDictionary<string, string> values)
        {
            return BindVariable(name, values);
        }

        public UriTemplateResolver Ignore(string name)
        {
            return BindVariable(name, null);
        }

        public UriTemplateResolver Clear()
        {
            variables.Clear();
            return this;
        }

        private UriTemplateResolver BindVariable(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            variables.Add(name, value);
            return this;
        }
    }
}