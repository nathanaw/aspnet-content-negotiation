// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CodecExample.Common
{
    public abstract class BaseJsonSerializationDecoder : BaseDecoder
    {

        /// <summary>
        /// Reads an object from the request body.
        /// </summary>
        /// <param name="context">The <see cref="DecoderContext"/>.</param>
        /// <returns>A <see cref="Task"/> that on completion deserializes the request body.</returns>
        public override async Task<object> ReadRequestBodyAsync(DecoderContext context) 
        {
            var serializerOptions = new JsonSerializerOptions();
            serializerOptions.WriteIndented = true;

            return await JsonSerializer.DeserializeAsync(context.InputStream, context.ModelType, serializerOptions);
        }

    }

}
