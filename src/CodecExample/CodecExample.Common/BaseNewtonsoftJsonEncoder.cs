// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CodecExample.Common
{
    public abstract class BaseNewtonsoftJsonEncoder : BaseEncoder
    {

        /// <summary>
        /// Writes the object to the supplied stream as JSON.
        /// </summary>
        /// <param name="context">Encoding context.</param>
        public async override Task WriteResponseBodyAsync(EncoderContext context)
        {
            JToken jobject = EncodeToJToken(context);

            var textWriter = context.WriterFactory(context.OutputStream, context.Encoding);
            var jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.Formatting = Formatting.Indented;
            await jobject.WriteToAsync(jsonWriter);
            await jsonWriter.FlushAsync();
        }

        /// <summary>
        /// Implement the encoding logic to create a JToken from the supplied object.
        /// </summary>
        /// <param name="context">The encoder context with the object to encode.</param>
        /// <returns>The JToken representation of the object.</returns>
        public abstract JToken EncodeToJToken(EncoderContext context);

    }

}
