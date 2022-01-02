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
    public abstract class BaseNewtonsoftJsonDecoder : BaseDecoder
    {

        /// <summary>
        /// Reads an object from the request body.
        /// </summary>
        /// <param name="context">The <see cref="DecoderContext"/>.</param>
        /// <returns>A <see cref="Task"/> that on completion deserializes the request body.</returns>
        public override async Task<object> ReadRequestBodyAsync(DecoderContext context) 
        {
            var textReader = context.ReaderFactory(context.InputStream, context.Encoding);

            var jsonReader = new JsonTextReader(textReader);
            jsonReader.DateParseHandling = DateParseHandling.None;
            JToken jobject = await JToken.LoadAsync(jsonReader);

            try
            {
                return DecodeFromJToken(jobject, context);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error decoding JSON from {this.GetType().Name} for '{context.MediaType}' mediatype.", ex);
            }
        }

        /// <summary>
        /// Implement in decoders to populate an object from the supplied JToken.
        /// </summary>
        /// <param name="jObject">The JObject representation of the input stream.</param>
        /// <returns>An object populated from the values in the JObject</returns>
        public abstract object DecodeFromJToken(JToken jObject, DecoderContext context);

    }

}
