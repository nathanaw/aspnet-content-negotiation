// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CodecExample.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodecExample.Codecs.Custom
{
    /// <summary>
    /// A <see cref="TextInputFormatter"/> for JSON content that uses <see cref="JsonSerializer"/>.
    /// </summary>
    public class WeatherForecastCustomV1Decoder : BaseNewtonsoftJsonDecoder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WeatherForecastCustomInputFormatter"/>.
        /// </summary>
        public WeatherForecastCustomV1Decoder()
        {
            AddSupportedMediaType(WeatherForecastCustomV1Encoder.WeatherForecastJsonV1MediaType);
        }

        public override bool CanReadType(Type type)
        {
            return type.IsAssignableFrom(typeof(WeatherForecast));
        }


        public override object DecodeFromJToken(JToken jObject, DecoderContext context)
        {
            WeatherForecast forecast = new WeatherForecast();

            forecast.Date = DateTime.ParseExact(
                                        jObject.Value<string>("date"),
                                        new[]
                                        {
                                            "yyyy-MM-ddTHH:mm:ss.fffZ",
                                            "yyyy-MM-ddTHH:mm:ss.ffZ",
                                            "yyyy-MM-ddTHH:mm:ss.fZ",
                                            "yyyy-MM-ddTHH:mm:ssZ",
                                        },
                                        CultureInfo.InvariantCulture);

            var tempNode = jObject["temperature"];
            if (tempNode is not null)
            {
                forecast.TemperatureC = tempNode.Value<int>("celcius");
            }

            forecast.Summary = jObject.Value<string>("summary");

            return forecast;
        }
    }
}