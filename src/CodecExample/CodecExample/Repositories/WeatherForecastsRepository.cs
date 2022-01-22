using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodecExample;
using CodecExample.Common;
using CodecExample.Codecs.Custom;
using CodecExample.Codecs.Serialized;
using Microsoft.Net.Http.Headers;

namespace CodecExample.Repositories
{

    /// <summary>
    /// A data repository for reading and writing weather forecasts.
    /// 
    /// Demonstrates 
    ///     1. Use of the Transcoder for encoding and decoding the resource.
    /// </summary>
    public class WeatherForecastsRepository
    {
        private readonly string RelativeDataPath = "Data/WeatherForecasts.json";

        private readonly Transcoder Transcoder;
        private readonly Encoding UTF8EncodingWithoutBOM
            = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private string FullDataPath = null;

        /// <summary>
        /// Creates an instance of the repository.
        /// </summary>
        /// <param name="transcoder">
        /// The transcoder, which should be populated with all relevant codecs that this repository may need. (Required)
        /// </param>
        public WeatherForecastsRepository(Transcoder transcoder)
        {
            Transcoder = transcoder ?? throw new ArgumentNullException("transcoder");

            FullDataPath = Path.Combine(
                                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                                RelativeDataPath);
        }


        /// <summary>
        /// Get a collection of forecasts.
        /// </summary>
        public async Task<IEnumerable<WeatherForecast>> FindAll()
        {
            return await DecodeFile<IEnumerable<WeatherForecast>>(
                            this.FullDataPath, 
                            WeatherForecastCollectionCustomV1Encoder.WeatherForecastCollectionJsonV1MediaType);
        }

        /// <summary>
        /// Get a forecasts.
        /// </summary>
        public async Task<WeatherForecast> Find(int day)
        {
            var allForecasts = (
                    await DecodeFile<IEnumerable<WeatherForecast>>(
                            this.FullDataPath, 
                            WeatherForecastCollectionCustomV1Encoder.WeatherForecastCollectionJsonV1MediaType)
                ).ToList();

            return allForecasts[day - 1];
        }

        /// <summary>
        /// Save a collection of forecasts.
        /// </summary>
        public async Task<IEnumerable<WeatherForecast>> SaveAll(IEnumerable<WeatherForecast> forecasts)
        {
            await EncodeToFile<IEnumerable<WeatherForecast>>(
                            this.FullDataPath, forecasts,
                            WeatherForecastCollectionCustomV1Encoder.WeatherForecastCollectionJsonV1MediaType);

            return forecasts;
        }

        /// <summary>
        /// Save a forecast.
        /// </summary>
        public async Task<IEnumerable<WeatherForecast>> Save(WeatherForecast forecast)
        {
            // Get the full list.
            var forecasts = (await DecodeFile<IEnumerable<WeatherForecast>>(
                                    this.FullDataPath, 
                                    WeatherForecastCollectionCustomV1Encoder.WeatherForecastCollectionJsonV1MediaType)
                            ).ToList();

            // Find the match by date.
            var day = forecasts.SingleOrDefault(f => f.Date.Date == forecast.Date.Date);

            if (day is not null)
            {
                forecasts.Remove(day);
            }

            forecasts.Add(forecast);

            // Keep the list sorted.
            forecasts = forecasts.OrderBy(f => f.Date).ToList();

            await EncodeToFile<IEnumerable<WeatherForecast>>(
                            this.FullDataPath, 
                            forecasts,
                            WeatherForecastCollectionCustomV1Encoder.WeatherForecastCollectionJsonV1MediaType);

            return forecasts;
        }


        /// <summary>
        /// Decodes the response representation into the specified resource type
        /// using the content type header to select the proper codec.
        /// </summary>
        /// <typeparam name="T">The type of resource to return.</typeparam>
        /// <returns>The resource decoded by the transcoder and codecs.</returns>
        private async Task<T> DecodeFile<T>(string filePath, string mediaType)
        {
            var mediaTypeValue = MediaTypeHeaderValue.Parse(mediaType);  // Use MS class. NET6 only.

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var decoderContext = new DecoderContext()
                {
                    InputStream = stream,
                    MediaType = mediaTypeValue,
                    ModelType = typeof(T)
                };

                var responseObject = await Transcoder.ReadAsync(decoderContext);

                return (T)responseObject;
            }
        }

        /// <summary>
        /// Use the transcoder to encode the resource into the desired media type.
        /// Then create and populate an HttpContent to send to the server.
        /// </summary>
        /// <typeparam name="T">The type to use on the EncoderContext.ObjectType.</typeparam>
        /// <param name="resource">The resource instance to encode into the HTTP request body.</param>
        /// <param name="desiredMediaType">The mediatype that the transcoder should use.</param>
        /// <returns>
        /// An HttpContent that can be used in the HTTP request body.
        /// The ContentType header will be set to the actual mediatype that the transcoder used.
        /// </returns>
        private async Task EncodeToFile<T>(string filePath, object resource, string desiredMediaType)
        {

            using (var stream = new FileStream(filePath, FileMode.Truncate, FileAccess.Write))
            {
                // Create the content to post.
                var encoderContext = new EncoderContext()
                {
                    DesiredMediaType = MediaTypeHeaderValue.Parse(desiredMediaType),
                    Object = resource,
                    ObjectType = typeof(T),
                    OutputStream = stream,

                    // This must not send a BOM (byte order mark) or else the server will fail to parse the JSON.
                    Encoding = UTF8EncodingWithoutBOM
                };

                await Transcoder.WriteAsync(encoderContext);

                await stream.FlushAsync();
                stream.Close();
            }
        }



    }
}
