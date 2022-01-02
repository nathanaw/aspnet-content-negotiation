// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

// Portions of code are based on .NET Source, shared under MIT license
// Primarily based on: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Core/Formatters/OutputFormatter.cs,8f3819df9b12f4fc

using Microsoft.AspNetCore.Mvc.Formatters;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;

namespace CodecExample.Common
{
    public interface IEncoder
    {
        /// <summary>
        /// Gets the mutable collection of media type elements supported by this encoder.
        /// </summary>
        IList<MediaTypeHeaderValue> SupportedMediaTypes { get; }

        bool CanWrite(EncoderContext context);

        Task WriteAsync(EncoderContext context);

    }
}
