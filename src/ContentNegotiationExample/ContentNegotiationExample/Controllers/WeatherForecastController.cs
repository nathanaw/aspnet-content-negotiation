// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

namespace ContentNegotiationExample.Controllers
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


        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
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
            Formatters.Custom.WeatherForecastCollectionCustomOutputFormatter.WeatherForecastCollectionJsonV1, 
            Formatters.Serialized.WeatherForecastCollectionSerializedOutputFormatter.WeatherForecastCollectionJsonV1, 
            Type = typeof(IEnumerable<WeatherForecast>))]
        public IActionResult Get()
        {
            return Ok(GetMultidayForecast());
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
            Formatters.Custom.WeatherForecastCustomOutputFormatter.WeatherForecastJsonV1,
            Formatters.Serialized.WeatherForecastSerializedOutputFormatter.WeatherForecastJsonV1, 
            Type=typeof(WeatherForecast))]
        public IActionResult Get([FromRoute] int day)
        {
            return Ok(GetSingleDayForecast(day));
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
        public IActionResult GetNoProduces([FromRoute] int day)
        {
            return Ok(GetSingleDayForecast(day));
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
        public IActionResult GetProducesType([FromRoute] int day)
        {
            return Ok(GetSingleDayForecast(day));
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
            Formatters.Custom.WeatherForecastCustomOutputFormatter.WeatherForecastJsonV1,
            Formatters.Serialized.WeatherForecastSerializedOutputFormatter.WeatherForecastJsonV1)]
        [Produces(
            Formatters.Custom.WeatherForecastCustomOutputFormatter.WeatherForecastJsonV1,
            Formatters.Serialized.WeatherForecastSerializedOutputFormatter.WeatherForecastJsonV1,
            Type = typeof(WeatherForecast))]
        public IActionResult Post([FromBody] WeatherForecast forecast)
        {
            return Ok(ProcessForecast(forecast));
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
            Formatters.Serialized.WeatherForecastSerializedOutputFormatter.WeatherForecastJsonV1)]
        [Produces(
            Formatters.Custom.WeatherForecastCustomOutputFormatter.WeatherForecastJsonV1,
            Formatters.Serialized.WeatherForecastSerializedOutputFormatter.WeatherForecastJsonV1,
            Type = typeof(WeatherForecast))]
        public IActionResult PostConsumesSerialized([FromBody] WeatherForecast forecast)
        {
            return Ok(ProcessForecast(forecast));
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
            Formatters.Custom.WeatherForecastCustomOutputFormatter.WeatherForecastJsonV1)]
        [Produces(
            Formatters.Custom.WeatherForecastCustomOutputFormatter.WeatherForecastJsonV1,
            Formatters.Serialized.WeatherForecastSerializedOutputFormatter.WeatherForecastJsonV1,
            Type = typeof(WeatherForecast))]
        public IActionResult PostConsumesCustom([FromBody] WeatherForecast forecast)
        {
            return Ok(ProcessForecast(forecast));
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
            Formatters.Custom.WeatherForecastCustomOutputFormatter.WeatherForecastJsonV1,
            Formatters.Serialized.WeatherForecastSerializedOutputFormatter.WeatherForecastJsonV1,
            Type = typeof(WeatherForecast))]
        public IActionResult PostNoConsumes(WeatherForecast forecast)
        {
            return Ok(ProcessForecast(forecast));
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
            Formatters.Custom.WeatherForecastCollectionCustomOutputFormatter.WeatherForecastCollectionJsonV1)]
        [Produces(
            Formatters.Custom.WeatherForecastCollectionCustomOutputFormatter.WeatherForecastCollectionJsonV1,
            Formatters.Serialized.WeatherForecastCollectionSerializedOutputFormatter.WeatherForecastCollectionJsonV1,
            Type = typeof(IEnumerable<WeatherForecast>))]
        public IActionResult PostConsumesCustomCollection([FromBody] IEnumerable<WeatherForecast> forecasts)
        {
            return Ok(ProcessMultipleForecasts(forecasts));
        }




        private IEnumerable<WeatherForecast> GetMultidayForecast()
        {
            var rng = new Random();
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return forecasts;
        }

        private WeatherForecast GetSingleDayForecast(int day)
        {
            var rng = new Random();
            var forecast = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(day),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            };

            return forecast;
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
