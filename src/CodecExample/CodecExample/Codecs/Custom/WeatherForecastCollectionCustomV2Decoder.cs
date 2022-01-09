﻿// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
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
    public class WeatherForecastCollectionCustomV2Decoder : BaseNewtonsoftJsonDecoder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WeatherForecastCollectionCustomInputFormatter"/>.
        /// </summary>
        public WeatherForecastCollectionCustomV2Decoder()
        {
            AddSupportedMediaType(WeatherForecastCollectionCustomV2Encoder.WeatherForecastCollectionJsonV2MediaType);
        }

        public override bool CanReadType(Type type)
        {
            return type.IsAssignableFrom(typeof(List<WeatherForecast>));
        }

        public override object DecodeFromJToken(JToken jObject, DecoderContext context)
        {
            var forecastCollection = new List<WeatherForecast>();

            // --------------------------------------------------------------------------------
            // Use the decoding logic of the singular item decoder.
            // --------------------------------------------------------------------------------
            var itemDecoder = new WeatherForecastCustomV2Decoder();

            JArray jarray = (JArray)jObject;

            if (jarray is object) // not null
            {
                foreach (var forecast in jarray)
                {
                    var decodedForecast = (WeatherForecast)itemDecoder.DecodeFromJToken(forecast, context);
                    forecastCollection.Add(decodedForecast);
                }
            }

            return forecastCollection;
        }

    }
}