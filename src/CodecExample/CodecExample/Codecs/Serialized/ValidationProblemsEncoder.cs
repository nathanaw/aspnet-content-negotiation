// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using CodecExample.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodecExample.Codecs.Serialized
{

    /// <summary>
    /// An encoder for the asp.net core ProblemDetails type, which is used for errors in the 
    /// formatter pipeline.
    /// </summary>
    public class ValidationProblemsEncoder : BaseJsonSerializationEncoder
    {
        public const string ProblemDetailsMediaType = "application/problem+json";

        public ValidationProblemsEncoder()
        {
            AddSupportedMediaType(ProblemDetailsMediaType);
        }

        public override bool CanWrite(EncoderContext context)
        {
            // Bypass some checks. Return true if the object type is a ProblemDetails.
            // Specifically skpping the check to see if the request's Accept header included this media type.
            //   We'll just return this media type, regardless of whether it's in the header.
            // This would ideally be a bit more selective to support json vs xml, but this is sufficient for now.
            if (base.CanWriteType(context.ObjectType))
            {
                // Set the ActualMediaType here in the CanWrite check.
                // This is needed in this phase so that ANC has the correct media type when it writes
                // headers, which happens before calling WriteAsync() to write the body.
                // This feels strange, but it works.
                context.ActualMediaType = MediaTypeHeaderValue.Parse(ProblemDetailsMediaType);

                return true;
            }

            return false;
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(ProblemDetails).IsAssignableFrom(type);
        }

        public async override Task WriteAsync(EncoderContext context)
        {
            await base.WriteAsync(context);
        }

    }
}
