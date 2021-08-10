// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ContentNegotiationExample.Formatters.Custom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContentNegotiationExample.Formatters.Custom
{
    /// <summary>
    /// A <see cref="TextInputFormatter"/> for JSON content that uses <see cref="JsonSerializer"/>.
    /// </summary>
    public class WeatherForecastCustomInputFormatter : BaseJsonCustomInputFormatter
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WeatherForecastCustomInputFormatter"/>.
        /// </summary>
        public WeatherForecastCustomInputFormatter()
            : base(new MediaTypeHeaderValue[] { 
                        MediaTypeHeaderValue.Parse(WeatherForecastCustomOutputFormatter.WeatherForecastJsonV1) 
                    })
        {
        }
        protected override async Task<object> DecodeJsonFromStream(TextReader inputStream, InputFormatterContext context)
        {
            var jsonReader = new JsonTextReader(inputStream);
            jsonReader.DateParseHandling = DateParseHandling.None;
            JToken jobject = await JToken.LoadAsync(jsonReader);

            return DecodeFromJToken(jobject);
        }

        public static object DecodeFromJToken(JToken jobject)
        {
            WeatherForecast forecast = new WeatherForecast();

            forecast.Date = DateTime.ParseExact(
                                        jobject.Value<string>("date"), 
                                        new[]
                                        {
                                            "yyyy-MM-ddTHH:mm:ss.fffZ",
                                            "yyyy-MM-ddTHH:mm:ss.ffZ",
                                            "yyyy-MM-ddTHH:mm:ss.fZ",
                                            "yyyy-MM-ddTHH:mm:ssZ",
                                        }, 
                                        CultureInfo.InvariantCulture);

            forecast.TemperatureC = jobject["temperature"].Value<int>("celcius");

            forecast.Summary = jobject.Value<string>("summary");

            return forecast;
        }
    }
}