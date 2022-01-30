using CodecExample.Common.Protobuf.Messages;

namespace CodecExample.Common.Protobuf.Codecs
{
    public class WeatherForecastCollectionV1Decoder : BaseDecoder
    {

        // Some debate on what the type/subtype should be here.
        public readonly string MediaType = "application/protobuf; Domain=Example.WeatherForecastCollection; Version=1";


        public WeatherForecastCollectionV1Decoder()
        {
            AddSupportedMediaType(MediaType);
        }

        public override bool CanReadType(Type type)
        {
            return type.IsAssignableFrom(typeof(List<WeatherForecast>));
        }

        public async override Task<object> ReadRequestBodyAsync(DecoderContext context)
        {
            // ------------------------------------------------------------------
            // Google's Protobuf library doesn't support async operations.
            // Avoiding sync IO here by writing to a memory stream first, then async to the HTTP stream.
            // Not ideal, but works for the sake of an example.
            // ------------------------------------------------------------------
            using (var ms = new MemoryStream())
            {
                await context.InputStream.CopyToAsync(ms);
                ms.Position = 0;

                var message = WeatherForecastCollectionV1.Parser.ParseFrom(ms);

                var resources = message.ToResources();

                return resources;
            }
        }
    }
}