// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

// Based on https://github.com/dotnet/aspnetcore/blob/d1c349f29752ac0718bda7b809ab7fd4f82ca414/src/Mvc/Mvc.Core/src/Formatters/SystemTextJsonOutputFormatter.cs
// From .NET Foundation, which is licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContentNegotiationExample.Formatters.Serialized
{
    /// <summary>
    /// A <see cref="TextOutputFormatter"/> for JSON content that uses <see cref="JsonSerializer"/>.
    /// </summary>
    public abstract class BaseJsonSerializedOutputFormatter : TextOutputFormatter
    {
        /// <summary>
        /// Initializes a new <see cref="BaseJsonSerializedOutputFormatter"/> instance.
        /// </summary>
        /// <param name="jsonSerializerOptions">The <see cref="JsonSerializerOptions"/>.</param>
        public BaseJsonSerializedOutputFormatter(JsonSerializerOptions jsonSerializerOptions, IEnumerable<MediaTypeHeaderValue> supportedMediaTypes)
        {
            SerializerOptions = jsonSerializerOptions;

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);

            foreach (var mediaType in supportedMediaTypes)
            {
                SupportedMediaTypes.Add(mediaType);
            }
        }

        /// <summary>
        /// Gets the <see cref="JsonSerializerOptions"/> used to configure the <see cref="JsonSerializer"/>.
        /// </summary>
        /// <remarks>
        /// A single instance of <see cref="BaseJsonSerializedOutputFormatter"/> is used for all JSON formatting. Any
        /// changes to the options will affect all output formatting.
        /// </remarks>
        public JsonSerializerOptions SerializerOptions { get; }

        /// <inheritdoc />
        public sealed override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (selectedEncoding == null)
            {
                throw new ArgumentNullException(nameof(selectedEncoding));
            }

            var httpContext = context.HttpContext;

            // context.ObjectType reflects the declared model type when specified.
            // For polymorphic scenarios where the user declares a return type, but returns a derived type,
            // we want to serialize all the properties on the derived type. This keeps parity with
            // the behavior you get when the user does not declare the return type and with Json.Net at least at the top level.
            var objectType = context.Object?.GetType() ?? context.ObjectType ?? typeof(object);

            var responseStream = httpContext.Response.Body;
            if (selectedEncoding.CodePage == Encoding.UTF8.CodePage)
            {
                await JsonSerializer.SerializeAsync(responseStream, context.Object, objectType, SerializerOptions);
                await responseStream.FlushAsync();
            }
            else
            {
                // JsonSerializer only emits UTF8 encoded output, but we need to write the response in the encoding specified by
                // selectedEncoding
                var transcodingStream = Encoding.CreateTranscodingStream(httpContext.Response.Body, selectedEncoding, Encoding.UTF8, leaveOpen: true);

                ExceptionDispatchInfo exceptionDispatchInfo = null;
                try
                {
                    await JsonSerializer.SerializeAsync(transcodingStream, context.Object, objectType, SerializerOptions);
                    await transcodingStream.FlushAsync();
                }
                catch (Exception ex)
                {
                    // TranscodingStream may write to the inner stream as part of it's disposal.
                    // We do not want this exception "ex" to be eclipsed by any exception encountered during the write. We will stash it and
                    // explicitly rethrow it during the finally block.
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                }
                finally
                {
                    try
                    {
                        await transcodingStream.DisposeAsync();
                    }
                    catch when (exceptionDispatchInfo != null)
                    {
                    }

                    exceptionDispatchInfo?.Throw();
                }
            }
        }
    }
}