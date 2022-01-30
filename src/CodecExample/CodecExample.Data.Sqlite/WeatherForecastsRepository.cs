using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodecExample.Common;
using Microsoft.Net.Http.Headers;
using Microsoft.Data.Sqlite;
using Dapper;
using CodecExample.Common.Codecs.Custom;
using SqlKata;
using SqlKata.Execution;
using SqlKata.Extensions;
using CodecExample.Common.Protobuf.Codecs;
using CodecExample.Common.Codecs.Serialized;

namespace CodecExample.Data.Sqlite
{

    /// <summary>
    /// A data repository for reading and writing weather forecasts.
    /// 
    /// Demonstrates 
    ///     1. Use of the Transcoder for encoding and decoding the resource.
    ///     
    ///     2. Usage of SqlKata and Dapper for saving encoded resources to a
    ///        Sqlite database.
    /// </summary>
    public class WeatherForecastsRepository
    {
        private readonly string ConnectionString;

        private readonly Transcoder Transcoder;
        private readonly Encoding UTF8EncodingWithoutBOM
            = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);


        /// <summary>
        /// Creates an instance of the repository.
        /// </summary>
        /// <param name="transcoder">
        /// The transcoder, which should be populated with all relevant codecs that this repository may need. (Required)
        /// </param>
        public WeatherForecastsRepository(Transcoder transcoder, string connectionString)
        {
            Transcoder = transcoder ?? throw new ArgumentNullException("transcoder");
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }


        /// <summary>
        /// Get a collection of forecasts.
        /// </summary>
        public async Task<IEnumerable<WeatherForecast>> FindAll(int? minTempCelcius = null, int? maxTempCelcius = null)
        {
            using var connection = new SqliteConnection(ConnectionString);

            // Use SqlKata to build up the SQL dynamically,
            // and safely with parameterized SQL parameters.
            var db = new SqlKata.Execution.QueryFactory(connection, new SqlKata.Compilers.SqliteCompiler());

            var query = db.Query("WeatherForecasts")
                            .Select("ID", "MediaType", "Content");

            // Add this where clause, but only if the value was supplied.
            if (minTempCelcius.HasValue)
            {
                query = query.Where("TempCelcius", ">=", minTempCelcius.Value);
            }

            // Add this where clause, but only if the value was supplied.
            if (maxTempCelcius.HasValue)
            {
                query = query.Where("TempCelcius", "<=", maxTempCelcius.Value);
            }

            // Execute the query to fetch the records.
            // This uses Dapper to map the columns into the specified type.
            var rows = await query.GetAsync<EncodedContentRow>();


            // Decode the content on each row into the resource.
            var resources = rows.Select(async r => await DecodeRow<WeatherForecast>(r));

            return await Task.WhenAll(resources);
        }


        /// <summary>
        /// Get a forecasts.
        /// </summary>
        public async Task<WeatherForecast> Find(int day)
        {
            var id = DateTime.Now.AddDays(day - 1).ToString("yyyy-MM-dd");

            using var connection = new SqliteConnection(ConnectionString);

            var row = (await connection.QueryAsync<EncodedContentRow>(@"
                                    SELECT ID, MediaType, Content
                                    FROM WeatherForecasts
                                    WHERE ID = @ID;
                                ", new { ID = id })).SingleOrDefault();

            if (row is not null)
            {
                return await DecodeRow<WeatherForecast>(row);
            }

            return null;
        }



        /// <summary>
        /// Save a collection of forecasts.
        /// </summary>
        public async Task<IEnumerable<WeatherForecast>> SaveAll(IEnumerable<WeatherForecast> forecasts)
        {
            using var connection = new SqliteConnection(ConnectionString);

            var rows = await Task.WhenAll(
                            forecasts.Select(async forecast =>
                                        await EncodeWeatherForecastToRow(forecast))
                        );

            foreach (var row in rows)
            {
                // Remove existing row so that we can replace with new entry.
                // There are better ways to do this, but this keeps it simple
                // for the example.
                var rowsAffected = await connection.ExecuteAsync(@"
                                    DELETE FROM WeatherForecasts 
                                    WHERE ID = @ID
                                ", row);

                await connection.ExecuteAsync(@"
                                    INSERT INTO WeatherForecasts 
                                    (ID, MediaType, Content, TempCelcius)
                                    VALUES
                                    (@ID, @MediaType, @Content, @TempCelcius)
                                ", row);
            }

            return forecasts;
        }

        /// <summary>
        /// Save a forecast.
        /// </summary>
        public async Task<WeatherForecast> Save(WeatherForecast forecast)
        {
            using var connection = new SqliteConnection(ConnectionString);

            var row = await EncodeWeatherForecastToRow(forecast);

            // Remove existing row so that we can replace with new entry.
            // There are better ways to do this, but this keeps it simple
            // for the example.
            var rowsAffected = await connection.ExecuteAsync(@"
                            DELETE FROM WeatherForecasts 
                            WHERE ID = @ID
                        ", row);

            await connection.ExecuteAsync(@"
                            INSERT INTO WeatherForecasts 
                            (ID, MediaType, Content, TempCelcius)
                            VALUES
                            (@ID, @MediaType, @Content, @TempCelcius)
                        ", row);

            return forecast;
        }

        private async Task<WeatherForecastContentRow> EncodeWeatherForecastToRow(WeatherForecast forecast)
        {
            // For DEMO purposes, use different codecs depending on the temperature.
            // This lets the caller simulate different types in the DB rows.
            // In real usage, the encoder would likely come from config or be fixed.

            var mediatype = WeatherForecastCustomV1Encoder.WeatherForecastJsonV1MediaType;

            if (forecast.TemperatureC <= 10)
                mediatype = WeatherForecastV1Encoder.MediaType;
            else if (forecast.TemperatureC <= 20)
                mediatype = WeatherForecastCustomV1Encoder.WeatherForecastJsonV1MediaType;
            else if (forecast.TemperatureC <= 30)
                mediatype = WeatherForecastCustomV2Encoder.WeatherForecastJsonV2MediaType;
            else
                mediatype = WeatherForecastSerializedV1Encoder.WeatherForecastJsonV1MediaType;


            var row = await EncodeToRow<WeatherForecast, WeatherForecastContentRow>(
                            forecast,
                            mediatype,
                            f => f.Date.Date.ToString("yyyy-MM-dd")
                        );

            // Promote a copy of this value so that SQL queries can see it.
            row.TempCelcius = forecast.TemperatureC;

            return row;
        }


        /// <summary>
        /// Decodes the response representation into the specified resource type
        /// using the specified mediatype to select the proper codec.
        /// </summary>
        /// <typeparam name="T">The type of resource to return.</typeparam>
        /// <returns>The resource decoded by the transcoder and codecs.</returns>
        private async Task<T> DecodeRow<T>(EncodedContentRow row)
        {
            var mediaTypeValue = MediaTypeHeaderValue.Parse(row.MediaType);  // Use MS class. NET6 only.

            using (var stream = new MemoryStream(row.Content))
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
        /// Then create and populate an EncodedContentRow to save to DB.
        /// </summary>
        /// <typeparam name="T">The type to use on the EncoderContext.ObjectType.</typeparam>
        /// <param name="resource">The resource instance to encode into the HTTP request body.</param>
        /// <param name="desiredMediaType">The mediatype that the transcoder should use.</param>
        /// <param name="idFunc">A delegate that returns the ID of the resource.</param>
        /// <returns>
        /// An EncodedContentRow that can be saved to the DB.
        /// </returns>
        private async Task<TRow> EncodeToRow<TResource, TRow>(TResource resource, string desiredMediaType, Func<TResource, string> idFunc)
            where TRow : EncodedContentRow, new()
        {
            var row = new TRow();


            using (var stream = new MemoryStream())
            {
                // Create the content to post.
                var encoderContext = new EncoderContext()
                {
                    DesiredMediaType = MediaTypeHeaderValue.Parse(desiredMediaType),
                    Object = resource,
                    ObjectType = typeof(TResource),
                    OutputStream = stream,

                    // This must not send a BOM (byte order mark) or else the server will fail to parse the JSON.
                    Encoding = UTF8EncodingWithoutBOM
                };

                await Transcoder.WriteAsync(encoderContext);

                stream.Position = 0;

                row.Content = stream.ToArray();
                row.MediaType = encoderContext.ActualMediaType.ToString();
                row.ID = idFunc(resource);
            }

            return row;
        }

    }
}
