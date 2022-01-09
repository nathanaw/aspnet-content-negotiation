using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Json;
using CodecExample.Common;
using CodecExample.Codecs.Custom;
using CodecExample.Codecs.Serialized;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;

namespace CodecExample.Tests
{
    [TestClass]
    public class CodecTests
    {
        /// <summary>
        /// The transcoder used by most tests. Fully populated with relevant codecs.
        /// </summary>
        private Transcoder _Transcoder;


        public CodecTests()
        {
            _Transcoder = GetTranscoder();
        }



        /// <summary>
        /// Test showing how to round trip test the codecs.
        ///   1. Starts with a fully populated representation. 
        ///   2. Decodes it to a resouce. 
        ///   3. Encodes back to a representation.
        ///   4. Compares the original representation to the encoded representation.
        /// </summary>
        /// <param name="inputFilePath">Relative path to the test data file.</param>
        /// <param name="mediaType">The mediatype to use for decoding / enoding.</param>
        /// <param name="resourceType">The Type that the encoder / decoder are working with.</param>

        [DataRow("TestData/Codecs/Custom/WeatherForecastV1.json", "application/json; Domain=Example.WeatherForecast.Custom; Version=1", typeof(WeatherForecast))]
        [DataRow("TestData/Codecs/Custom/WeatherForecastV1 - No Summary.json", "application/json; Domain=Example.WeatherForecast.Custom; Version=1", typeof(WeatherForecast))]
        [DataRow("TestData/Codecs/Custom/WeatherForecastCollectionV1.json", "application/json; Domain=Example.WeatherForecastCollection.Custom; Version=1", typeof(IEnumerable<WeatherForecast>))]

        [DataRow("TestData/Codecs/Custom/WeatherForecastV2.json", "application/json; Domain=Example.WeatherForecast.Custom; Version=2", typeof(WeatherForecast))]
        [DataRow("TestData/Codecs/Custom/WeatherForecastV2 - No Summary.json", "application/json; Domain=Example.WeatherForecast.Custom; Version=2", typeof(WeatherForecast))]
        [DataRow("TestData/Codecs/Custom/WeatherForecastCollectionV2.json", "application/json; Domain=Example.WeatherForecastCollection.Custom; Version=2", typeof(IEnumerable<WeatherForecast>))]

        [DataRow("TestData/Codecs/Serialized/WeatherForecastV1 - No Summary.json", "application/json; Domain=Example.WeatherForecast.Serialized; Version=1", typeof(WeatherForecast))]
        [DataRow("TestData/Codecs/Serialized/WeatherForecastV1 - No Summary.json", "application/json; Domain=Example.WeatherForecast.Serialized; Version=1", typeof(WeatherForecast))]
        [DataRow("TestData/Codecs/Serialized/WeatherForecastCollectionV1.json", "application/json; Domain=Example.WeatherForecastCollection.Serialized; Version=1", typeof(IEnumerable<WeatherForecast>))]

        [TestMethod]
        public async Task RoundTripRepresentations(string inputFilePath, string mediaType, Type resourceType)
        {
            // -------------------------
            // Load json from disk
            // -------------------------
            var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var testFile = Path.Combine(testDirectory, inputFilePath);

            var inputJsonString = File.ReadAllText(testFile);
            // Console.WriteLine(inputJsonString);
            var inputJObject = JToken.Parse(inputJsonString);


            object decodedResource = null;
            var mediaTypeValue = MediaTypeHeaderValue.Parse(mediaType);

            // -------------------------
            // Decode to resource
            // -------------------------
            using (var stream = new FileStream(testFile, FileMode.Open))
            {
                var decoderContext = new DecoderContext()
                {
                    InputStream = stream,
                    MediaType = mediaTypeValue,
                    ModelType = resourceType
                };

                decodedResource = await _Transcoder.ReadAsync(decoderContext);
            }

            decodedResource.Should().NotBeNull();

            // -------------------------
            // Encode the resource
            // -------------------------
            JToken encodedJObject = null;
            using (var encodedResource = new MemoryStream())
            {
                var encoderContext = new EncoderContext()
                {
                    DesiredMediaType = mediaTypeValue,
                    Object = decodedResource,
                    ObjectType = resourceType,
                    OutputStream = encodedResource
                };

                await _Transcoder.WriteAsync(encoderContext);

                // Parse to JObject
                encodedResource.Position = 0;
                using (var sr = new StreamReader(encodedResource))
                {
                    var encodedJson = sr.ReadToEnd();
                    encodedJObject = JToken.Parse(encodedJson);
                }
            }

            // -------------------------
            // Compare JObjects
            // This uses FluentAssertions.Json
            // -------------------------
            encodedJObject.Should().BeEquivalentTo(inputJObject);
        }

        /// <summary>
        /// Test showing how to round trip test the codecs.
        ///   1. Starts with a fully populated resource instance. 
        ///   3. Encodes it to a representation.
        ///   2. Decodes it back to a resouce. 
        ///   4. Compares the original resource to the decoded resource.
        /// 
        /// Uses DynamicData to feed the test scenarios for this test.
        /// </summary>
        /// <param name="description">A description of the test scenario.</param>
        /// <param name="resource">An instance of a resource to encode / decode.</param>
        /// <param name="mediaType">The mediatype to use for decoding / enoding.</param>

        [TestMethod]
        [DynamicData(nameof(GetRoundTripResourcesData), DynamicDataSourceType.Property)]
        public async Task RoundTripResources(string description, object resource, string mediaType)
        {
            var mediaTypeValue = MediaTypeHeaderValue.Parse(mediaType);
            object decodedResource = null;

            // -------------------------
            // Encode the resource
            // -------------------------
            using (var encodedResource = new MemoryStream())
            {
                var encoderContext = new EncoderContext()
                {
                    DesiredMediaType = mediaTypeValue,
                    Object = resource,
                    ObjectType = resource.GetType(),
                    OutputStream = encodedResource
                };

                await _Transcoder.WriteAsync(encoderContext);

                // Reset the stream position so that we can use it for the decoder.
                encodedResource.Position = 0;

                // -------------------------
                // Decode to resource
                // -------------------------
                var decoderContext = new DecoderContext()
                {
                    InputStream = encodedResource,
                    MediaType = mediaTypeValue,
                    ModelType = encoderContext.ObjectType
                };

                decodedResource = await _Transcoder.ReadAsync(decoderContext);
            }

            decodedResource.Should().NotBeNull();

            // -------------------------
            // Compare resources
            // This uses FluentAssertions
            // -------------------------
            decodedResource.Should().BeEquivalentTo(resource,
                    config => config
                        // -------------------------------------------------------------------
                        // Instruct fluent assertions to allow the date field to be off by
                        // up to 1 millisecond. This is needed because the codecs format
                        // the value to 3 decimal places, which strips some of the original
                        // precision.
                        // -------------------------------------------------------------------
                        .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
                            .WhenTypeIs<DateTime>()
                    );
        }



        /// <summary>
        /// Creates a transcoder and populates it with the codecs needed by the tests.
        /// </summary>
        private static Transcoder GetTranscoder()
        {
            var transcoder = new Transcoder();

            // V1 Custom Codecs
            transcoder.Encoders.Add(new WeatherForecastCustomV1Encoder());
            transcoder.Encoders.Add(new WeatherForecastCollectionCustomV1Encoder());
            transcoder.Decoders.Add(new WeatherForecastCustomV1Decoder());
            transcoder.Decoders.Add(new WeatherForecastCollectionCustomV1Decoder());

            // V2 Custom Codecs
            transcoder.Encoders.Add(new WeatherForecastCustomV2Encoder());
            transcoder.Encoders.Add(new WeatherForecastCollectionCustomV2Encoder());
            transcoder.Decoders.Add(new WeatherForecastCustomV2Decoder());
            transcoder.Decoders.Add(new WeatherForecastCollectionCustomV2Decoder());

            // V1 Serilization-based Codecs
            transcoder.Encoders.Add(new WeatherForecastSerializedV1Encoder());
            transcoder.Encoders.Add(new WeatherForecastCollectionSerializedV1Encoder());
            transcoder.Decoders.Add(new WeatherForecastSerializedV1Decoder());
            transcoder.Decoders.Add(new WeatherForecastCollectionSerializedV1Decoder());

            // Other
            transcoder.Encoders.Add(new ValidationProblemsEncoder());

            return transcoder;
        }


        /// <summary>
        /// A DynamicData property for the RoundTripResources test.
        /// </summary>
        public static IEnumerable<object[]> GetRoundTripResourcesData
        {
            get
            {
                var forecast = new WeatherForecast()
                {
                    Date = DateTime.Now,
                    TemperatureC = 20,
                    Summary = "Sunny"
                };

                var forecastCollection = new List<WeatherForecast>()
                {
                    new WeatherForecast()
                    {
                        Date = DateTime.Now,
                        TemperatureC = 20,
                        Summary = "Sunny"
                    },
                    new WeatherForecast()
                    {
                        Date = DateTime.Now,
                        TemperatureC = 25,
                        Summary = "Thunderstoms and lightning"
                    },
                    new WeatherForecast()
                    {
                        Date = DateTime.Now,
                        TemperatureC = 30,
                        Summary = "Cloudy, but hot"
                    }
                };

                yield return new object[] { 
                    /* Description   */ "WeatherForecast Custom V1",
                    /* Object        */ forecast,
                    /* MediaType     */ "application/json; Domain=Example.WeatherForecast.Custom; Version=1"
                };

                yield return new object[] { 
                    /* Description   */ "WeatherForecastCollection Custom V1",
                    /* Object        */ forecastCollection,
                    /* MediaType     */ "application/json; Domain=Example.WeatherForecastCollection.Custom; Version=1"
                };

                yield return new object[] { 
                    /* Description   */ "WeatherForecast Custom V2",
                    /* Object        */ forecast,
                    /* MediaType     */ "application/json; Domain=Example.WeatherForecast.Custom; Version=2"
                };

                yield return new object[] { 
                    /* Description   */ "WeatherForecastCollection Custom V2",
                    /* Object        */ forecastCollection,
                    /* MediaType     */ "application/json; Domain=Example.WeatherForecastCollection.Custom; Version=2"
                };

                yield return new object[] { 
                    /* Description   */ "WeatherForecast Serialized V1",
                    /* Object        */ forecast,
                    /* MediaType     */ "application/json; Domain=Example.WeatherForecast.Serialized; Version=1"
                };

                yield return new object[] { 
                    /* Description   */ "WeatherForecastCollection Serialized V1",
                    /* Object        */ forecastCollection,
                    /* MediaType     */ "application/json; Domain=Example.WeatherForecastCollection.Serialized; Version=1"
                };


            }
        }
    }

}