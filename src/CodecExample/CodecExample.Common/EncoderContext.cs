// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

// Portions of code are based on .NET Source, shared under MIT license
// Primarily from: https://source.dot.net/#Microsoft.AspNetCore.Mvc.Abstractions/Formatters/OutputFormatterCanWriteContext.cs,551499fba54fa867

using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodecExample.Common
{
    public class EncoderContext
    {

        public EncoderContext()
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="context">Context to copy</param>
        public EncoderContext(EncoderContext context)
        {
            this.DesiredMediaType = context.DesiredMediaType;
            this.ActualMediaType = context.ActualMediaType;
            this.Encoding = context.Encoding;
            this.WriterFactory = context.WriterFactory;
            this.ObjectType = context.ObjectType;
            this.Object = context.Object;
            this.OutputStream = context.OutputStream;
        }

        /// <summary>
        /// Gets or sets the media type that should be encoded.
        /// Optional. If null, the ActualMediaType will be set to the SupportedMediaType of the encoder.
        /// </summary>
        public virtual MediaTypeHeaderValue DesiredMediaType { get; set; }

        /// <summary>
        /// Gets or sets the media type that the encoder wrote to the stream.
        /// </summary>
        public virtual MediaTypeHeaderValue ActualMediaType { get; set; }

        /// <summary>
        /// Gets or sets the object to encode to the stream.
        /// </summary>
        public virtual object Object { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the object to encode to the stream.
        /// </summary>
        public virtual Type ObjectType { get; set; }

        /// <summary>
        /// Gets a delegate which can create a System.IO.TextWriter for the output stream body.
        /// </summary>
        public Func<Stream, Encoding, TextWriter> WriterFactory { get; set; }

        /// <summary>
        /// The encoding to use in the output stream.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// The stream for writing encoded data.
        /// </summary>
        public Stream OutputStream { get; set; }

    }
}
