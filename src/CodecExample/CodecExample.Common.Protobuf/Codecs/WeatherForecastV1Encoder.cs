using CodecExample.Common.Protobuf.Messages;

namespace CodecExample.Common.Protobuf.Codecs
{
    public class WeatherForecastV1Encoder : BaseEncoder
    {

        // Some debate on what the type/subtype should be here.
        public const string MediaType = "application/protobuf; Domain=Example.WeatherForecast; Version=1";


        public WeatherForecastV1Encoder()
        {
            AddSupportedMediaType(MediaType);
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(WeatherForecast).IsAssignableFrom(type);
        }

        public async override Task WriteResponseBodyAsync(EncoderContext context)
        {
            var resource = (WeatherForecast)context.Object;

            // Map data
            var message = new WeatherForecastV1(resource);


            // ------------------------------------------------------------------
            // Google's Protobuf library doesn't support async operations.
            // Avoiding sync IO here by writing to a memory stream first, then async to the HTTP stream.
            // Not ideal, but works for the sake of an example.
            // ------------------------------------------------------------------
            using (var ms = new MemoryStream())
            using (var codedStream = new Google.Protobuf.CodedOutputStream(ms))
            {
                message.WriteTo(codedStream);
                codedStream.Flush();

                ms.Position = 0;

                await ms.CopyToAsync(context.OutputStream);
            }
        }

    }

}