// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

// Portions of code are based on .NET Source, shared under MIT license
// Primarily from: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Abstractions/Formatters/InputFormatterContext.cs,641b503b73e15a46

using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodecExample.Common
{
    public class DecoderContext
    {
        public DecoderContext()
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="context">Context to copy from</param>
        public DecoderContext(DecoderContext context)
        {
            this.ModelType = context.ModelType;
            this.ReaderFactory = context.ReaderFactory; 
            this.Encoding = context.Encoding;
            this.MediaType = context.MediaType;
            this.InputStream = context.InputStream;
        }

        /// <summary>
        /// Gets the System.Type of the decoded object to return.
        /// This is optional.
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        /// Gets a delegate which can create a System.IO.TextReader for the input stream body.
        /// </summary>
        public Func<Stream, Encoding, TextReader> ReaderFactory { get; set;  }

        /// <summary>
        /// The encoding to use for the input stream.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// The media type (aka Content Type) of the data in the InputStream
        /// </summary>
        public MediaTypeHeaderValue MediaType { get; set; }

        /// <summary>
        /// The stream of data to decode.
        /// </summary>
        public Stream InputStream { get; set; }

    }
}
