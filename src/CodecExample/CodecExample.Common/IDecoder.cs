// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

// Portions of code are based on .NET Source, shared under MIT license
// Primarily based on: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Core/Formatters/InputFormatter.cs,56af25e5fdcc32bf


using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodecExample.Common
{
    public interface IDecoder
    {
        /// <summary>
        /// Gets the mutable collection of media type elements supported by this decoder.
        /// </summary>
        IList<MediaTypeHeaderValue> SupportedMediaTypes { get; }

        bool CanRead(DecoderContext context);

        Task<object> ReadAsync(DecoderContext context);

    }
}
