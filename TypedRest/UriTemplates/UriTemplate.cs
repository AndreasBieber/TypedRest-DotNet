﻿// Resta.UriTemplates
// Copyright(c) 2013 Pavel Shkarin
// Source: https://github.com/a7b0/uri-templates
// License: The MIT License(MIT)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TypedRest.UriTemplates
{
    /// <summary>
    /// URI template processor (RFC6570).
    /// </summary>
    public sealed class UriTemplate
    {
        private readonly string template;
        private readonly List<IUriComponent> components;
        private List<VarSpec> variables;

        public UriTemplate(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            this.template = template;
            this.components = UriTemplateParser.Parse(template);
        }

        internal UriTemplate(IEnumerable<IUriComponent> components)
        {
            if (components == null)
            {
                throw new ArgumentNullException("components");
            }

            this.components = new List<IUriComponent>(components);
            this.template = GetTemplateString(this.components);
        }

        public string Template
        {
            get { return template; }
        }

        public IEnumerable<VarSpec> Variables
        {
            get
            {
                if (variables == null)
                {
                    variables = GetVariables(components);
                }

                return variables;
            }
        }

        internal IEnumerable<IUriComponent> Components
        {
            get { return components; }
        }

        public UriTemplateResolver GetResolver()
        {
            return new UriTemplateResolver(this);
        }

        public string Resolve(IDictionary<string, object> variables)
        {
            if (variables == null)
            {
                throw new ArgumentNullException("variables");
            }

            var builder = new StringBuilder(template.Length*2);

            try
            {
                foreach (var component in components)
                {
                    component.Resolve(builder, variables);
                }
            }
            catch (UriTemplateException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new UriTemplateException("Error at resolve URI template.", exception);
            }

            return builder.ToString();
        }

        public string Resolve(object variables)
        {
            if (variables == null)
            {
                throw new ArgumentNullException("variables");
            }

            return Resolve(variables.GetType().GetRuntimeProperties().Where(property => property.GetMethod != null).ToDictionary(x => x.Name, x => x.GetValue(variables)));
        }

        public UriTemplate ResolveTemplate(IDictionary<string, object> variables)
        {
            if (variables == null)
            {
                throw new ArgumentNullException("variables");
            }

            if (variables.Count == 0)
            {
                return this;
            }

            var partialComponents = new List<IUriComponent>();

            try
            {
                foreach (var component in components)
                {
                    partialComponents.AddRange(component.ResolveTemplate(variables));
                }
            }
            catch (UriTemplateException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new UriTemplateException("Error at partial resolve URI template.", exception);
            }

            return new UriTemplate(partialComponents);
        }

        public override string ToString()
        {
            return template;
        }

        private static string GetTemplateString(List<IUriComponent> components)
        {
            var builder = new StringBuilder();

            foreach (var component in components)
            {
                builder.Append(component);
            }

            return builder.ToString();
        }

        private static List<VarSpec> GetVariables(List<IUriComponent> components)
        {
            var variables = new List<VarSpec>();

            foreach (var component in components)
            {
                var expression = component as Expression;

                if (expression != null)
                {
                    foreach (var varSpec in expression.VarSpecs)
                    {
                        variables.Add(varSpec);
                    }
                }
            }

            return variables;
        }
    }
}