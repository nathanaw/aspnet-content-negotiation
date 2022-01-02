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
    public class WeatherForecastCustomV2Decoder : BaseNewtonsoftJsonDecoder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WeatherForecastCustomInputFormatter"/>.
        /// </summary>
        public WeatherForecastCustomV2Decoder()
        {
            AddSupportedMediaType(WeatherForecastCustomV2Encoder.WeatherForecastJsonV2);
        }

        public override bool CanReadType(Type type)
        {
            return type.IsAssignableFrom(typeof(WeatherForecast));
        }

        public override object DecodeFromJToken(JToken jObject, DecoderContext context)
        {
            WeatherForecast forecast = new WeatherForecast();

            // --------------------------------------------------------------------------------
            // This decoder shows how to perform validation that required fields are present.
            // Where possible, this should be handled in the controller or business logic.
            // These checks should be for fields that are crucial to parsing the structure.
            // --------------------------------------------------------------------------------

            var dateNode = jObject["date"];
            if (dateNode is null) throw new FormatException("WeatherForcast is missing the 'date' node.");

            forecast.Date = DateTime.ParseExact(
                                        dateNode.Value<string>(),
                                        new[]
                                        {
                                            "yyyy-MM-ddTHH:mm:ss.fffZ",
                                            "yyyy-MM-ddTHH:mm:ss.ffZ",
                                            "yyyy-MM-ddTHH:mm:ss.fZ",
                                            "yyyy-MM-ddTHH:mm:ssZ",
                                        },
                                        CultureInfo.InvariantCulture);

            var tempNode = jObject["temp"];
            if (tempNode is null) throw new FormatException("WeatherForcast is missing the 'temp' node.");

            var celciusNode = tempNode["c"];
            if (celciusNode is null) throw new FormatException("WeatherForcast is missing the 'temp.c' node.");
            forecast.TemperatureC = celciusNode.Value<int>();

            forecast.Summary = jObject.Value<string>("summary");

            return forecast;
        }
    }
}