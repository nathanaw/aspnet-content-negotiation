// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

// Based on https://github.com/dotnet/aspnetcore/blob/d1c349f29752ac0718bda7b809ab7fd4f82ca414/src/Mvc/Mvc.Core/src/Formatters/SystemTextJsonInputFormatter.cs
// from .NET Foundation, which is licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Net.Http.Headers;
using CodecExample.Common;

namespace CodecExample.Common.Formatters
{
    /// <summary>
    /// A <see cref="TextInputFormatter"/> for decoding content with the Transcoder and IDecoders.
    /// </summary>
    public class TranscoderInputFormatter : TextInputFormatter, IInputFormatterExceptionPolicy
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="TranscoderInputFormatter"/>.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public TranscoderInputFormatter(Transcoder transcoder)
        {
            _logger = NullLogger.Instance; // TODO

            SupportedEncodings.Add(UTF8EncodingWithoutBOM);
            SupportedEncodings.Add(UTF16EncodingLittleEndian);

            Transcoder = transcoder;

            SetSupportedMediaTypesFromTranscoder(transcoder);
        }

        /// <summary>
        /// The transcoder used by this output formatter.
        /// </summary>
        public Transcoder Transcoder { get; }

        /// <inheritdoc />
        InputFormatterExceptionPolicy IInputFormatterExceptionPolicy.ExceptionPolicy => InputFormatterExceptionPolicy.MalformedInputExceptions;

        public override bool CanRead(InputFormatterContext context)
        {
            var decoderContext = new DecoderContext()
            {
                InputStream = null, // Not passed for CanRead operations.
                MediaType = MediaTypeHeaderValue.Parse(context.HttpContext.Request.ContentType),
                ModelType = context.ModelType,
                ReaderFactory = context.ReaderFactory,
            };

            return base.CanRead(context)
                   && 
                   Transcoder.CanRead(decoderContext);
        }

        /// <inheritdoc />
        public sealed override async Task<InputFormatterResult> ReadRequestBodyAsync(
            InputFormatterContext context,
            Encoding encoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            var httpContext = context.HttpContext;
            var (inputStream, usesTranscodingStream) = GetInputStream(httpContext, encoding);

            object model;
            try
            {
                model = await DecodeFromStream(inputStream, Encoding.UTF8, context);
            }
            catch (JsonException jsonException)
            {
                var path = string.Empty;

                var modelStateException = WrapExceptionForModelState(jsonException);

                context.ModelState.TryAddModelError(path, modelStateException, context.Metadata);

                Log.JsonInputException(_logger, jsonException);

                return InputFormatterResult.Failure();
            }
            catch (Exception exception) when (exception is FormatException || exception is OverflowException)
            {
                // The code in System.Text.Json never throws these exceptions. However a custom converter could produce these errors for instance when
                // parsing a value. These error messages are considered safe to report to users using ModelState.

                context.ModelState.TryAddModelError(string.Empty, exception, context.Metadata);
                Log.JsonInputException(_logger, exception);

                return InputFormatterResult.Failure();
            }
            finally
            {
                if (usesTranscodingStream)
                {
                    await inputStream.DisposeAsync();
                }
            }

            if (model == null && !context.TreatEmptyInputAsDefaultValue)
            {
                // Some nonempty inputs might deserialize as null, for example whitespace,
                // or the JSON-encoded value "null". The upstream BodyModelBinder needs to
                // be notified that we don't regard this as a real input so it can register
                // a model binding error.
                return InputFormatterResult.NoValue();
            }
            else
            {
                Log.JsonInputSuccess(_logger, context.ModelType);
                return InputFormatterResult.Success(model);
            }
        }

        protected async Task<object> DecodeFromStream(Stream inputStream, Encoding encoding, InputFormatterContext context)
        {
            var contentType = context.HttpContext.Request.ContentType;

            var decoderContext = new DecoderContext()
            {
                InputStream = inputStream,
                MediaType = MediaTypeHeaderValue.Parse(contentType),
                ModelType = context.ModelType,
                ReaderFactory = context.ReaderFactory,
            };

            return await Transcoder.ReadAsync(decoderContext);

        }

        private Exception WrapExceptionForModelState(JsonException jsonException)
        {
            // InputFormatterException specifies that the message is safe to return to a client, it will
            // be added to model state.
            return new InputFormatterException(jsonException.Message, jsonException);
        }

        private (Stream inputStream, bool usesTranscodingStream) GetInputStream(HttpContext httpContext, Encoding encoding)
        {
            if (encoding.CodePage == Encoding.UTF8.CodePage)
            {
                return (httpContext.Request.Body, false);
            }

            var inputStream = Encoding.CreateTranscodingStream(httpContext.Request.Body, encoding, Encoding.UTF8, leaveOpen: true);
            return (inputStream, true);
        }


        private void SetSupportedMediaTypesFromTranscoder(Transcoder transcoder)
        {
            // TODO - Not thread-safe.

            foreach (var encoder in transcoder.Encoders)
            {
                foreach (var mediaType in encoder.SupportedMediaTypes)
                {
                    SupportedMediaTypes.Add(mediaType);
                }
            }
        }




        private static class Log
        {
            private static readonly Action<ILogger, string, Exception> _jsonInputFormatterException;
            private static readonly Action<ILogger, string, Exception> _jsonInputSuccess;

            static Log()
            {
                _jsonInputFormatterException = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    new EventId(1, "CustomJsonInputException"),
                    "Custom JSON input formatter threw an exception: {Message}");
                _jsonInputSuccess = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    new EventId(2, "CustomJsonInputSuccess"),
                    "Custom JSON input formatter succeeded, deserializing to type '{TypeName}'");
            }

            public static void JsonInputException(ILogger logger, Exception exception)
                => _jsonInputFormatterException(logger, exception.Message, exception);

            public static void JsonInputSuccess(ILogger logger, Type modelType)
                => _jsonInputSuccess(logger, modelType.FullName, null);
        }


    }
}