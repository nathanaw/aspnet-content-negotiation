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
    public abstract class BaseJsonSerializationEncoder : BaseEncoder
    {

        /// <summary>
        /// Writes the object to the supplied stream as JSON.
        /// </summary>
        /// <param name="context">Encoding context.</param>
        public async override Task WriteResponseBodyAsync(EncoderContext context)
        {
            var serializerOptions = new JsonSerializerOptions();
            serializerOptions.WriteIndented = true;

            await JsonSerializer.SerializeAsync(context.OutputStream, context.Object, context.ObjectType, serializerOptions);
            await context.OutputStream.FlushAsync();
        }

    }

}
