using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodecExample;
using CodecExample.Common;
using CodecExample.Codecs.Custom;
using CodecExample.Codecs.Serialized;
using Flurl;
using Flurl.Http;
using Microsoft.Net.Http.Headers;

namespace CodecExample.Client
{

    /// <summary>
    /// A client for accessing the WeatherForecast HTTP API endpoints on the CodecExample service.
    /// 
    /// Demonstrates 
    ///     1. Use of the Transcoder for decoding the HTTP Response representation 
    ///     into the resource instances.
    ///     
    ///     2. Use of Accept header media types for content negotiation.
    ///     
    ///     3. Use of the Flurl.Http library as a simple, clean way to make HTTP requests.
    /// 
    /// </summary>
    public class WeatherForecastClient
    {

        private readonly string CodecExampleServiceUri = "https://localhost:5001";
        private readonly Transcoder Transcoder;
        private readonly Encoding UTF8EncodingWithoutBOM
            = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);



        /// <summary>
        /// Creates an instance of the client.
        /// </summary>
        /// <param name="transcoder">
        /// The transcoder, which should be populated with all relevant codecs that this client may need. (Required)
        /// </param>
        public WeatherForecastClient(Transcoder transcoder)
        {
            Transcoder = transcoder ?? throw new ArgumentNullException("transcoder");
        }


        /// <summary>
        /// Get a collection of forecasts.
        /// 
        /// Accepts: 
        ///     application/json; Domain=Example.WeatherForecastCollection.Custom; Version=1
        /// </summary>
        public async Task<IEnumerable<WeatherForecast>> GetForecastsV1Custom()
        {
            // Use Flurl.Http to execute the request.
            // Include the Accept header.
            var response = await CodecExampleServiceUri
                                    .AppendPathSegment("WeatherForecast")
                                    .WithHeader("Accept", WeatherForecastCollectionCustomV1Encoder.WeatherForecastCollectionJsonV1MediaType)
                                    .GetAsync();

            return await DecodeResponse<IEnumerable<WeatherForecast>>(response);
        }

        /// <summary>
        /// Get a collection of forecasts.
        /// 
        /// Accepts: 
        ///     application/json; Domain=Example.WeatherForecastCollection.Custom; Version=2
        /// </summary>
        public async Task<IEnumerable<WeatherForecast>> GetForecastsV2Custom()
        {
            var response = await CodecExampleServiceUri
                                    .AppendPathSegment("WeatherForecast")
                                    .WithHeader("Accept", WeatherForecastCollectionCustomV2Encoder.WeatherForecastCollectionJsonV2MediaType)
                                    .GetAsync();

            return await DecodeResponse<IEnumerable<WeatherForecast>>(response);
        }

        /// <summary>
        /// Get a collection of forecasts.
        /// 
        /// Accepts: 
        ///     application/json; Domain=Example.WeatherForecastCollection.Serialized; Version=1
        /// </summary>
        public async Task<IEnumerable<WeatherForecast>> GetForecastsV1Serialized()
        {
            var response = await CodecExampleServiceUri
                                    .AppendPathSegment("WeatherForecast")
                                    .WithHeader("Accept", WeatherForecastCollectionSerializedV1Encoder.WeatherForecastCollectionJsonV1MediaType)
                                    .GetAsync();

            return await DecodeResponse<IEnumerable<WeatherForecast>>(response);
        }




        /// <summary>
        /// Get a collection of forecasts.
        /// Uses server content negotiation to select the preferred media type.
        /// 
        /// Accepts: 
        ///     application/json; Domain=Example.WeatherForecastCollection.Custom; Version=1      q=0.900
        ///     application/json; Domain=Example.WeatherForecastCollection.Custom; Version=2      q=0.500
        ///     application/json; Domain=Example.WeatherForecastCollection.Serialized; Version=1; q=0.100 
        /// </summary>
        public async Task<IEnumerable<WeatherForecast>> GetForecasts()
        {
            // Accept multiple.
            // Includes quality paramters to influence preference for which mediatype to select.
            var acceptedMediaTypes = string.Join(", ",
                                            WeatherForecastCollectionCustomV2Encoder.WeatherForecastCollectionJsonV2MediaType     + "; q=0.900",
                                            WeatherForecastCollectionCustomV1Encoder.WeatherForecastCollectionJsonV1MediaType     + "; q=0.500",
                                            WeatherForecastCollectionSerializedV1Encoder.WeatherForecastCollectionJsonV1MediaType + "; q=0.100"
                                        );

            var response = await CodecExampleServiceUri
                                    .AppendPathSegment("WeatherForecast")
                                    .WithHeader("Accept", acceptedMediaTypes)
                                    .GetAsync();

            return await DecodeResponse<IEnumerable<WeatherForecast>>(response);
        }



        /// <summary>
        /// Post a forecast to the server and receive a response with a forecast.
        /// 
        /// Sends (Content-Type):
        ///     application/json; Domain=Example.WeatherForecast.Custom; Version=1
        /// 
        /// Accepts: 
        ///     application/json; Domain=Example.WeatherForecast.Custom; Version=2;
        /// </summary>
        public async Task<WeatherForecast> PostForecastV1Custom(WeatherForecast forecast)
        {
            // Create the HttpContent that will be posted to the server.
            // This uses the transcoder to encode the resource into a MemoryStream for sending to the server.
            var httpPostContent = await CreateHttpRequestContent<WeatherForecast>(forecast,
                                            WeatherForecastCustomV1Encoder.WeatherForecastJsonV1MediaType);

            // POST the request to the server.
            var response = await CodecExampleServiceUri
                                    .AppendPathSegments("WeatherForecast")
                                    .WithHeader("Accept", WeatherForecastCustomV2Encoder.WeatherForecastJsonV2MediaType)
                                    .PostAsync(httpPostContent);

            // Decode the response.
            return await DecodeResponse<WeatherForecast>(response);
        }




        /// <summary>
        /// Post a collection of forecasts to the server and receive a response with a collection of forecasts.
        /// 
        /// Sends (Content-Type):
        ///     application/json; Domain=Example.WeatherForecastCollection.Custom; Version=1
        /// 
        /// Accepts: 
        ///     application/json; Domain=Example.WeatherForecastCollection.Custom; Version=2;
        /// </summary>
        public async Task<IEnumerable<WeatherForecast>> PostForecastV1Custom(IEnumerable<WeatherForecast> forecasts)
        {
            // Create the HttpContent that will be posted to the server.
            // This uses the transcoder to encode the resource into a MemoryStream for sending to the server.
            var httpPostContent = await CreateHttpRequestContent<IEnumerable<WeatherForecast>>(forecasts, 
                                            WeatherForecastCollectionCustomV1Encoder.WeatherForecastCollectionJsonV1MediaType);

            // POST the request to the server.
            var response = await CodecExampleServiceUri
                                    .AppendPathSegments("WeatherForecast", "ConsumesCustomCollection")
                                    .WithHeader("Accept", WeatherForecastCollectionCustomV2Encoder.WeatherForecastCollectionJsonV2MediaType)
                                    .PostAsync(httpPostContent);

            // Decode the response.
            return await DecodeResponse<IEnumerable<WeatherForecast>>(response);
        }


        /// <summary>
        /// Demonstrate an HTTP 406 unacceptable response when the client and server
        /// cannot find a common mediatype.
        /// 
        /// Throws an exception.
        /// 
        /// </summary>
        public async Task<IEnumerable<WeatherForecast>> GetUnacceptableMediaType()
        {
            // Force an HTTP 406 by using a non-supported mediatype
            // This will throw an exception.
            var response = await CodecExampleServiceUri
                                    .AppendPathSegment("WeatherForecast")
                                    .WithHeader("Accept", "application/json; Domain=foo; Version=9999")
                                    .GetAsync();

            return await DecodeResponse<IEnumerable<WeatherForecast>>(response);
        }


        /// <summary>
        /// Decodes the response representation into the specified resource type
        /// using the content type header to select the proper codec.
        /// </summary>
        /// <typeparam name="T">The type of resource to return.</typeparam>
        /// <param name="response">The IFlurlResponse to be parsed.</param>
        /// <returns>The resource decoded by the transcoder and codecs.</returns>
        private async Task<T> DecodeResponse<T>(IFlurlResponse response)
        {
            // Throw exception if not successfull
            response.ResponseMessage.EnsureSuccessStatusCode();

            // The transcoder needs to know what the server actually returned.
            // This is in the Content-Type header of the response.
            var contentType = response.ResponseMessage.Content.Headers.ContentType;
            var mediaType = MediaTypeHeaderValue.Parse(contentType.ToString());  // Use MS class. NET6 only.

            // Log a few details about the request / response to the console.
            var request = response.ResponseMessage.RequestMessage;

            Console.WriteLine($"Handling Response");
            Console.WriteLine($"\tRequest URI:           {request.Method} {request.RequestUri}");
            Console.WriteLine($"\tRequest Accept:        {request.Headers.Accept}");

            Console.WriteLine($"\tResponse Status:       {response.StatusCode}");
            Console.WriteLine($"\tResponse Content-Type: {mediaType}");


            var stream = await response.GetStreamAsync();

            var decoderContext = new DecoderContext()
            {
                InputStream = stream,
                MediaType = mediaType,
                ModelType = typeof(T)
            };

            var responseObject = await Transcoder.ReadAsync(decoderContext);

            return (T)responseObject;
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
        private async Task<HttpContent> CreateHttpRequestContent<T>(object resource, string desiredMediaType)
        {
            // Create the content to post.
            var encoderContext = new EncoderContext()
            {
                DesiredMediaType = MediaTypeHeaderValue.Parse(desiredMediaType),
                Object = resource,
                ObjectType = typeof(T),
                OutputStream = new MemoryStream(),

                // This must not send a BOM (byte order mark) or else the server will fail to parse the JSON.
                Encoding = UTF8EncodingWithoutBOM
            };

            await Transcoder.WriteAsync(encoderContext);

            // Move back to start of stream.
            encoderContext.OutputStream.Position = 0; 

            // Create HttpContent and set the Content-Type header to the media type that was encoded.
            var httpPostContent = new StreamContent(encoderContext.OutputStream);
            httpPostContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(encoderContext.ActualMediaType.ToString());

            return httpPostContent;
        }



    }
}
