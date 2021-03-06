﻿using System;
using System.Net.Http;
using System.Text;

namespace TypedRest
{
    /// <summary>
    /// Provides extension methods for <seealso cref="Exception"/>.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Concatenates the <seealso cref="Exception.Message"/>s of the entire <seealso cref="Exception.InnerException"/> tree.
        /// </summary>
        public static string GetFullMessage(this Exception ex)
        {
            var builder = new StringBuilder();
            do
            {
                builder.AppendLine(ex.Message);
                ex = ex.InnerException;
            } while (ex != null && !(ex is HttpRequestException));
            return builder.ToString();
        }
    }
}