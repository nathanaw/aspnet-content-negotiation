// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodecExample.Common;
using CodecExample.Common.Codecs.Custom;
using CodecExample.Common.Codecs.Serialized;
using CodecExample.Data.Sqlite;
using CodecExample.Common.Protobuf.Codecs;

namespace CodecExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherForecastsRepository _repository;


        public WeatherForecastController(
                    ILogger<WeatherForecastController> logger
                    , WeatherForecastsRepository weatherForecastsRepository)
        {
            _logger = logger;
            _repository = weatherForecastsRepository;
        }


        /// <summary>
        /// Get a multi-day forecast
        /// Generates multiple output formats
        /// 
        /// Demonstrates 
        /// 1. Content-negotiation for response
        /// 2. Usage of produces for 
        ///     a. Constraining the candidate media types during output formatting
        ///     b. Informing the Swagger (OAS) schema 
        /// 3. Declaration of the System.Type that the endpoint produces (for Swagger)
        /// 4. Usage of IActionResult in method signature
        ///     a. Allows the method to return multiple responses,
        /// 
        /// </summary>
        [HttpGet]
        [Produces(
            WeatherForecastCollectionCustomV1Encoder.WeatherForecastCollectionJsonV1MediaType,
            WeatherForecastCollectionCustomV2Encoder.WeatherForecastCollectionJsonV2MediaType,
            WeatherForecastCollectionSerializedV1Encoder.WeatherForecastCollectionJsonV1MediaType,
            WeatherForecastCollectionV1Encoder.MediaType,
            Type = typeof(IEnumerable<WeatherForecast>))]
        public async Task<IActionResult> Get([FromQuery] int? minTempC, [FromQuery] int? maxTempC)
        {
            return Ok(await _repository.FindAll(minTempC, maxTempC));
        }


        /// <summary>
        /// Get a single-day forecast
        /// Generates multiple output formats
        /// 
        /// Demonstrates 
        /// 1. Content-negotiation for response
        /// 2. Usage of produces for 
        ///     a. Constraining the candidate media types during output formatting
        ///     b. Informing the Swagger (OAS) schema 
        /// 3. Declaration of the System.Type that the endpoint produces (for Swagger)
        /// 4. Usage of IActionResult in method signature
        ///     a. Allows the method to return multiple responses,
        /// 
        /// </summary>
        [HttpGet("{day}")]
        [Produces(
            WeatherForecastCustomV1Encoder.WeatherForecastJsonV1MediaType,
            WeatherForecastCustomV2Encoder.WeatherForecastJsonV2MediaType,
            WeatherForecastSerializedV1Encoder.WeatherForecastJsonV1MediaType,
            WeatherForecastV1Encoder.MediaType,
            Type = typeof(WeatherForecast))]
        public async Task<IActionResult> Get([FromRoute] int day)
        {
            var forecast = await _repository.Find(day);

            if (forecast is null)
            {
                return NotFound();
            }

            return Ok(forecast);
        }


        /// <summary>
        /// Get a single-day forecast
        /// Generates multiple output formats
        /// 
        /// Demonstrates 
        /// 1. Content-negotiation for response
        /// 2. Absence of Produces attribute
        ///     a. All formatters are candidates if they handle a WeatherForecast type
        ///     b. Swagger (OAS) schema lists all media types, including some that are invalid at runtime
        /// 3. Absence of any System.Type hints that the endpoint produces (for Swagger)
        /// 4. Usage of IActionResult in method signature
        ///     a. Allows the method to return multiple responses,
        /// 
        /// </summary>
        [HttpGet("/[controller]/NoProduces/{day}")]
        public async Task<IActionResult> GetNoProduces([FromRoute] int day)
        {
            var forecast = await _repository.Find(day);

            if (forecast is null)
            {
                return NotFound();
            }

            return Ok(forecast);
        }


        /// <summary>
        /// Get a single-day forecast
        /// Generates multiple output formats
        /// 
        /// Demonstrates 
        /// 1. Content-negotiation for response
        /// 2. Usage of Produces attribute, but only with Type info
        ///     a. All formatters are candidates if they handle a WeatherForecast type
        ///     b. Swagger (OAS) schema lists all media types, including some that are invalid at runtime
        /// 3. Declaration of the System.Type that the endpoint produces (for Swagger)
        /// 4. Usage of IActionResult in method signature
        ///     a. Allows the method to return multiple responses,
        /// 
        /// </summary>
        [HttpGet("/[controller]/ProducesType/{day}")]
        [Produces(typeof(WeatherForecast))]
        public async Task<IActionResult> GetProducesType([FromRoute] int day)
        {
            var forecast = await _repository.Find(day);

            if (forecast is null)
            {
                return NotFound();
            }

            return Ok(forecast);
        }


        /// <summary>
        /// Process a posted forecast
        /// Accepts multiple input formats
        /// Generates multiple output formats
        /// 
        /// Demonstrates 
        /// 1. Content-negotiation for response
        /// 2. Usage of both consumes and produces for 
        ///     a. Constraining the candidate media types during input and output formatting
        ///     b. Informing the Swagger (OAS) schema 
        /// 3. Declaration of the System.Type that the endpoint produces (for Swagger)
        /// 4. Usage of IActionResult in method signature
        ///     a. Allows the method to return multiple responses,
        /// 
        /// </summary>
        [HttpPost("/[controller]")]
        [Consumes(
            WeatherForecastCustomV1Encoder.WeatherForecastJsonV1MediaType,
            WeatherForecastCustomV2Encoder.WeatherForecastJsonV2MediaType,
            WeatherForecastSerializedV1Encoder.WeatherForecastJsonV1MediaType,
            WeatherForecastV1Encoder.MediaType)]
        [Produces(
            WeatherForecastCustomV1Encoder.WeatherForecastJsonV1MediaType,
            WeatherForecastCustomV2Encoder.WeatherForecastJsonV2MediaType,
            WeatherForecastSerializedV1Encoder.WeatherForecastJsonV1MediaType,
            WeatherForecastV1Encoder.MediaType,
            Type = typeof(WeatherForecast))]
        public async Task<IActionResult> Post([FromBody] WeatherForecast forecast)
        {
            return Ok(await _repository.Save(forecast));
        }


        /// <summary>
        /// Process a posted forecast
        /// Accepts single input format (serialized formatter)
        /// Generates multiple output formats
        /// 
        /// Demonstrates 
        /// 1. Content-negotiation for response
        /// 2. Usage of both consumes and produces for 
        ///     a. Constraining the candidate media types during input and output formatting
        ///     b. Informing the Swagger (OAS) schema 
        /// 3. Declaration of the System.Type that the endpoint produces (for Swagger)
        /// 4. Usage of IActionResult in method signature
        ///     a. Allows the method to return multiple responses,
        /// 
        /// </summary>
        [HttpPost("/[controller]/ConsumesSerialized")]
        [Consumes(
            WeatherForecastSerializedV1Encoder.WeatherForecastJsonV1MediaType)]
        [Produces(
            WeatherForecastCustomV1Encoder.WeatherForecastJsonV1MediaType,
            WeatherForecastCustomV2Encoder.WeatherForecastJsonV2MediaType,
            WeatherForecastSerializedV1Encoder.WeatherForecastJsonV1MediaType,
            WeatherForecastV1Encoder.MediaType,
            Type = typeof(WeatherForecast))]
        public async Task<IActionResult> PostConsumesSerialized([FromBody] WeatherForecast forecast)
        {
            return Ok(await _repository.Save(forecast));
        }


        /// <summary>
        /// Process a posted forecast
        /// Accepts single input format (custom formatter)
        /// Generates multiple output formats
        /// 
        /// Demonstrates 
        /// 1. Content-negotiation for response
        /// 2. Usage of both consumes and produces for 
        ///     a. Constraining the candidate media types during input and output formatting
        ///     b. Informing the Swagger (OAS) schema 
        /// 3. Declaration of the System.Type that the endpoint produces (for Swagger)
        /// 4. Usage of IActionResult in method signature
        ///     a. Allows the method to return multiple responses,
        /// 
        /// </summary>
        [HttpPost("/[controller]/ConsumesCustom")]
        [Consumes(
            WeatherForecastCustomV1Encoder.WeatherForecastJsonV1MediaType)]
        [Produces(
            WeatherForecastCustomV1Encoder.WeatherForecastJsonV1MediaType,
            WeatherForecastCustomV2Encoder.WeatherForecastJsonV2MediaType,
            WeatherForecastSerializedV1Encoder.WeatherForecastJsonV1MediaType,
            WeatherForecastV1Encoder.MediaType,
            Type = typeof(WeatherForecast))]
        public async Task<IActionResult> PostConsumesCustom([FromBody] WeatherForecast forecast)
        {
            return Ok(await _repository.Save(forecast));
        }


        /// <summary>
        /// Process a posted forecast
        /// Accepts multiple input formats
        /// Generates multiple output formats
        /// 
        /// Demonstrates 
        /// 1. Content-negotiation for response
        /// 2. Usage of produces for 
        ///     a. Constraining the candidate media types during output formatting
        ///     b. Informing the Swagger (OAS) schema 
        /// 3. Declaration of the System.Type that the endpoint produces (for Swagger)
        /// 4. Absence of Consumes attribute
        ///     a. All formatters are candidates if they handle a WeatherForecast type
        ///     b. Swagger (OAS) schema allows all media types, including some that are invalid at runtime
        /// 5. Usage of IActionResult in method signature
        ///     a. Allows the method to return multiple responses,
        /// 
        /// </summary>
        [HttpPost("/[controller]/NoConsumes")]
        [Produces(
            WeatherForecastCustomV1Encoder.WeatherForecastJsonV1MediaType,
            WeatherForecastCustomV2Encoder.WeatherForecastJsonV2MediaType,
            WeatherForecastSerializedV1Encoder.WeatherForecastJsonV1MediaType,
            WeatherForecastV1Encoder.MediaType,
            Type = typeof(WeatherForecast))]
        public async Task<IActionResult> PostNoConsumes(WeatherForecast forecast)
        {
            return Ok(await _repository.Save(forecast));
        }

        /// <summary>
        /// Process a collection of posted forecasts
        /// Accepts multiple input formats
        /// Generates multiple output formats
        /// 
        /// Demonstrates 
        /// 1. Content-negotiation for response
        /// 2. Usage of both consumes and produces for 
        ///     a. Constraining the candidate media types during input and output formatting
        ///     b. Informing the Swagger (OAS) schema 
        /// 3. Declaration of the System.Type that the endpoint produces (for Swagger)
        /// 4. Usage of IActionResult in method signature
        ///     a. Allows the method to return multiple responses,
        /// 
        /// </summary>
        [HttpPost("/[controller]/ConsumesCustomCollection")]
        [Consumes(
            WeatherForecastCollectionCustomV1Encoder.WeatherForecastCollectionJsonV1MediaType)]
        [Produces(
            WeatherForecastCollectionCustomV1Encoder.WeatherForecastCollectionJsonV1MediaType,
            WeatherForecastCollectionCustomV2Encoder.WeatherForecastCollectionJsonV2MediaType,
            WeatherForecastCollectionSerializedV1Encoder.WeatherForecastCollectionJsonV1MediaType,
            WeatherForecastCollectionV1Encoder.MediaType,
            Type = typeof(IEnumerable<WeatherForecast>))]
        public async Task<IActionResult> PostConsumesCustomCollection([FromBody] IEnumerable<WeatherForecast> forecasts)
        {
            return Ok(await _repository.SaveAll(forecasts));
        }




        private WeatherForecast ProcessForecast(WeatherForecast forecast)
        {
            forecast.Summary = "Yep, it is " + forecast.Summary;

            return forecast;
        }

        private IEnumerable<WeatherForecast> ProcessMultipleForecasts(IEnumerable<WeatherForecast> forecasts)
        {
            foreach (var forecast in forecasts)
            {
                forecast.Summary = "Yep, it is " + forecast.Summary;
            }

            return forecasts;
        }
    }
}
