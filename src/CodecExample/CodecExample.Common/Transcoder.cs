// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodecExample.Common
{
    public class Transcoder
    {

        public IList<IDecoder> Decoders { get; } = new List<IDecoder>();

        public IList<IEncoder> Encoders { get; } = new List<IEncoder>();


        public bool CanRead(DecoderContext context)
        {
            foreach (var decoder in Decoders)
            {
                if (decoder.CanRead(context))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<object> ReadAsync(DecoderContext context)
        {
            foreach (var decoder in Decoders)
            {
                if (decoder.CanRead(context))
                {
                    return await decoder.ReadAsync(context);
                }
            }

            throw new FormatException($"Unable to find a decoder for {context.MediaType}.");
        }


        public bool CanWrite(EncoderContext context) 
        {
            foreach (var encoder in Encoders)
            {
                if (encoder.CanWrite(context))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task WriteAsync(EncoderContext context) 
        {
            foreach (var encoder in Encoders)
            {
                if (encoder.CanWrite(context))
                {
                    await encoder.WriteAsync(context);
                    return;
                }
            }

            throw new FormatException($"Unable to find an encoder for {context.DesiredMediaType} that can encode an object of type {context.Object.GetType().Name}.");
        }

        public IDecoder GetDecoder(DecoderContext context)
        {
            foreach (var decoder in Decoders)
            {
                if (decoder.CanRead(context))
                {
                    return decoder;
                }
            }

            return null;
        }

        public IEncoder GetEncoder(EncoderContext context)
        {
            foreach (var encoder in Encoders)
            {
                if (encoder.CanWrite(context))
                {
                    return encoder;
                }
            }

            return null;
        }

    }
}
