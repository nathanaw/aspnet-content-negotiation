// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

// Portions of code are based on .NET Source, shared under MIT license
// Primarily based on: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Core/Formatters/InputFormatter.cs,56af25e5fdcc32bf


using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodecExample.Common
{
    public abstract class BaseDecoder : IDecoder
    {
        public IList<MediaTypeHeaderValue> SupportedMediaTypes { get; } = new List<MediaTypeHeaderValue>();

        protected void AddSupportedMediaType(string mediaType)
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(mediaType));
        }

        /// <summary>
        /// Determines whether this decoder can decode an object of the
        /// <paramref name="context"/>'s media type.
        /// </summary>
        /// <param name="context">The DecoderContext.</param>
        /// <returns>
        /// <c>true</c> if this decoder can decode an object of the
        /// <paramref name="context"/>'s media type. <c>false</c> otherwise.
        /// </returns>
        public virtual bool CanRead(DecoderContext context)
        {
            if (SupportedMediaTypes.Count == 0)
            {
                throw new InvalidOperationException($"Decoder '{this.GetType().Name}' does not support any media types. It must support at least one media type.");
            }

            if (context.ModelType is not null && !CanReadType(context.ModelType))
            {
                return false;
            }

            string contentType = context.MediaType?.ToString();
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            // Confirm the request's content type is more specific than a media type this formatter supports e.g. OK if
            // client sent "text/plain" data and this formatter supports "text/*".
            return IsSubsetOfAnySupportedContentType(contentType);
        }

        /// <summary>
        /// Reads an object from the input stream.
        /// </summary>
        /// <param name="context">The DecoderContext.</param>
        /// <returns>A <see cref="Task"/> that on completion deserializes the request body.</returns>
        public virtual Task<object> ReadAsync(DecoderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Stream inputStream = context.InputStream;

            //if (inputStream.Length == 0)
            //{
            //    return null;
            //}

            return ReadRequestBodyAsync(context);
        }


        /// <summary>
        /// Gets the default value for a given type. Used to return a default value when the body contains no content.
        /// </summary>
        /// <param name="modelType">The type of the value.</param>
        /// <returns>The default value for the <paramref name="modelType"/> type.</returns>
        protected virtual object? GetDefaultValueForType(Type modelType)
        {
            if (modelType == null)
            {
                throw new ArgumentNullException(nameof(modelType));
            }

            if (modelType.IsValueType)
            {
                return Activator.CreateInstance(modelType);
            }

            return null;
        }

        private bool IsSubsetOfAnySupportedContentType(string contentType)
        {
            var parsedContentType = MediaTypeHeaderValue.Parse(contentType);

            for (var i = 0; i < SupportedMediaTypes.Count; i++)
            {
                if (MediaTypeHeaderValueSupport.IsMatch(parsedContentType, SupportedMediaTypes[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether this decoder can decode an object of the given type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of object that will be read.</param>
        /// <returns><c>true</c> if the <paramref name="type"/> can be read, otherwise <c>false</c>.</returns>
        public virtual bool CanReadType(Type type)
        {
            return true;
        }


        /// <summary>
        /// Reads an object from the request body.
        /// </summary>
        /// <param name="context">The <see cref="DecoderContext"/>.</param>
        /// <returns>A <see cref="Task"/> that on completion deserializes the request body.</returns>
        public abstract Task<object> ReadRequestBodyAsync(DecoderContext context);


        /// <summary>
        /// Gets a filtered list of content types which are supported by the decoder
        /// for the <paramref name="objectType"/> and <paramref name="contentType"/>.
        /// </summary>
        /// <param name="contentType">
        /// The content type for which the supported content types are desired, or <c>null</c> if any content
        /// type can be used.
        /// </param>
        /// <param name="objectType">
        /// The <see cref="Type"/> for which the supported content types are desired.
        /// </param>
        /// <returns>Content types which are supported by the decoder.</returns>
        public virtual IReadOnlyList<MediaTypeHeaderValue>? GetSupportedContentTypes(string contentType, Type objectType)
        {
            if (SupportedMediaTypes.Count == 0)
            {
                throw new InvalidOperationException($"Decoder '{this.GetType().Name}' does not support any media types. It must support at least one media type.");
            }

            if (!CanReadType(objectType))
            {
                return null;
            }

            if (contentType == null)
            {
                // If contentType is null, then any type we support is valid.
                return SupportedMediaTypes.ToList().AsReadOnly();
            }
            else
            {
                var parsedContentType = MediaTypeHeaderValue.Parse(contentType);
                List<MediaTypeHeaderValue>? mediaTypes = null;

                // Confirm this formatter supports a more specific media type than requested e.g. OK if "text/*"
                // requested and formatter supports "text/plain". Treat contentType like it came from an Content-Type header.
                foreach (var mediaType in SupportedMediaTypes)
                {
                    if (MediaTypeHeaderValueSupport.IsMatch(mediaType, parsedContentType))
                    {
                        if (mediaTypes == null)
                        {
                            mediaTypes = new List<MediaTypeHeaderValue>(SupportedMediaTypes.Count);
                        }

                        mediaTypes.Add(mediaType);
                    }
                }

                return mediaTypes.AsReadOnly();
            }
        }

    }
}
