// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using CodecExample.Common;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodecExample.Codecs.Custom
{
    public class WeatherForecastCustomV2Encoder : BaseNewtonsoftJsonEncoder
    {
        public const string WeatherForecastJsonV2MediaType = "application/json; Domain=Example.WeatherForecast.Custom; Version=2";

        public WeatherForecastCustomV2Encoder()
        {
            AddSupportedMediaType(WeatherForecastJsonV2MediaType);
        }
        protected override bool CanWriteType(Type type)
        {
            return typeof(WeatherForecast).IsAssignableFrom(type);
        }

        public override JToken EncodeToJToken(EncoderContext context)
        {
            WeatherForecast forecast = (WeatherForecast)context.Object;
            JToken jobject = new JObject();

            // UTC date format. Full control. No ambiguity.
            jobject["date"] = forecast.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            // Structure the json as works best for json. Doesn't have to match object exactly.
            var temperatureNode = jobject["temp"] = new JObject();
            temperatureNode["c"] = forecast.TemperatureC;
            temperatureNode["f"] = forecast.TemperatureF;

            if (forecast.Summary is not null)
                jobject["summary"] = forecast.Summary;

            return jobject;
        }
    }
}
