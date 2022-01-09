// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

// Portions of code are based on .NET Source, shared under MIT license
// Primarily based on: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Core/Formatters/OutputFormatter.cs,8f3819df9b12f4fc

using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Net.Http.Headers;

namespace CodecExample.Common
{
    /// <summary>
    /// Writes an object to the output stream.
    /// </summary>
    public abstract class BaseEncoder : IEncoder
    {
        /// <summary>
        /// Gets the mutable collection of media type elements supported by
        /// this <see cref="OutputFormatter"/>.
        /// </summary>
        public IList<MediaTypeHeaderValue> SupportedMediaTypes { get; } = new List<MediaTypeHeaderValue>();

        protected void AddSupportedMediaType(string mediaType)
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaType));  
        }

        /// <summary>
        /// Returns a value indicating whether or not the given type can be written by this serializer.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <returns><c>true</c> if the type can be written, otherwise <c>false</c>.</returns>
        protected virtual bool CanWriteType(Type? type)
        {
            return true;
        }

        /// <inheritdoc />
        public virtual IReadOnlyList<string>? GetSupportedContentTypes(string contentType, Type objectType)
        {
            if (SupportedMediaTypes.Count == 0)
            {
                throw new InvalidOperationException($"Encoder '{this.GetType().Name}' does not support any media types. It must support at least one media type.");
            }

            if (!CanWriteType(objectType))
            {
                return null;
            }

            List<string>? mediaTypes = null;

            var parsedContentType = contentType != null ? MediaTypeHeaderValue.Parse(contentType) : default;

            foreach (var mediaType in SupportedMediaTypes)
            {
                if (MediaTypeHeaderValueSupport.HasWildcard(mediaType))
                {
                    // For supported media types that are wildcard patterns, confirm that the requested
                    // media type satisfies the wildcard pattern (e.g., if "text/entity+json;v=2" requested
                    // and formatter supports "text/*+json").
                    // Treat contentType like it came from a [Produces] attribute.
                    if (contentType != null && MediaTypeHeaderValueSupport.IsMatch(parsedContentType, mediaType))
                    {
                        if (mediaTypes == null)
                        {
                            mediaTypes = new List<string>(SupportedMediaTypes.Count);
                        }

                        mediaTypes.Add(contentType);
                    }
                }
                else
                {
                    // Confirm this formatter supports a more specific media type than requested e.g. OK if "text/*"
                    // requested and formatter supports "text/plain". Treat contentType like it came from an Accept header.
                    if (contentType == null || MediaTypeHeaderValueSupport.IsMatch(mediaType, parsedContentType))
                    {
                        if (mediaTypes == null)
                        {
                            mediaTypes = new List<string>(SupportedMediaTypes.Count);
                        }

                        mediaTypes.Add(mediaType.ToString());
                    }
                }
            }

            return mediaTypes;
        }

        /// <inheritdoc />
        public virtual bool CanWrite(EncoderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (SupportedMediaTypes.Count == 0)
            {
                throw new InvalidOperationException($"Encoder '{this.GetType().Name}' does not support any media types. It must support at least one media type.");
            }

            if (context.ObjectType is not null && !CanWriteType(context.ObjectType))
            {
                return false;
            }

            if (context.DesiredMediaType is null)
            {
                // If the desired media type is set to null, then the current encoder can write anything it wants.
                // Set the ActualMediaType here in the CanWrite check.
                // This is needed in this phase so that ANC has the correct media type when it writes
                // headers, which happens before calling WriteAsync() to write the body.
                // This feels strange, but it works.
                context.ActualMediaType = SupportedMediaTypes.Single();

                return true;
            }
            else
            {
                for (var i = 0; i < SupportedMediaTypes.Count; i++)
                {
                    if (MediaTypeHeaderValueSupport.HasWildcard(SupportedMediaTypes[i]))
                    {
                        // For supported media types that are wildcard patterns, confirm that the requested
                        // media type satisfies the wildcard pattern (e.g., if "text/entity+json;v=2" requested
                        // and formatter supports "text/*+json").
                        // We only do this when comparing against server-defined content types (e.g., those
                        // from [Produces] or Response.ContentType), otherwise we'd potentially be reflecting
                        // back arbitrary Accept header values.
                        if (MediaTypeHeaderValueSupport.IsMatch(context.DesiredMediaType, SupportedMediaTypes[i]))
                        {
                            // Set the ActualMediaType here in the CanWrite check.
                            // This is needed in this phase so that ANC has the correct media type when it writes
                            // headers, which happens before calling WriteAsync() to write the body.
                            // This feels strange, but it works.
                            context.ActualMediaType = context.DesiredMediaType;

                            return true;
                        }
                    }
                    else
                    {
                        // For supported media types that are not wildcard patterns, confirm that this formatter
                        // supports a more specific media type than requested e.g. OK if "text/*" requested and
                        // formatter supports "text/plain".
                        // contentType is typically what we got in an Accept header.
                        if (MediaTypeHeaderValueSupport.IsMatch(SupportedMediaTypes[i], context.DesiredMediaType))
                        {
                            // Set the ActualMediaType here in the CanWrite check.
                            // This is needed in this phase so that ANC has the correct media type when it writes
                            // headers, which happens before calling WriteAsync() to write the body.
                            // This feels strange, but it works.
                            context.ActualMediaType = SupportedMediaTypes[i];

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <inheritdoc />
        public virtual Task WriteAsync(EncoderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // See comments in the CanWrite() method regarding the ActualMediaType.
            // It can't be set here because the headers will have been written already.

            return WriteResponseBodyAsync(context);
        }

        /// <summary>
        /// Writes the response body.
        /// </summary>
        /// <param name="context">The formatter context associated with the call.</param>
        /// <returns>A task which can write the response body.</returns>
        public abstract Task WriteResponseBodyAsync(EncoderContext context);


    }

}
