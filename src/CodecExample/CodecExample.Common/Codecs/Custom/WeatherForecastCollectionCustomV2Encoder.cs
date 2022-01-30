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

namespace CodecExample.Common.Codecs.Custom
{
    public class WeatherForecastCollectionCustomV2Encoder : BaseNewtonsoftJsonEncoder
    {
        public const string WeatherForecastCollectionJsonV2MediaType = "application/json; Domain=Example.WeatherForecastCollection.Custom; Version=2";

        public WeatherForecastCollectionCustomV2Encoder()
        {
            AddSupportedMediaType(WeatherForecastCollectionJsonV2MediaType);
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(IEnumerable<WeatherForecast>).IsAssignableFrom(type);
        }

        public override JToken EncodeToJToken(EncoderContext encoderContext)
        {
            var forecastCollection = (IEnumerable<WeatherForecast>)encoderContext.Object;

            // --------------------------------------------------------------------------------
            // Use the encoding logic of the singular item encoder.
            // TODO - Fetch the item encoder from transcoder.
            // --------------------------------------------------------------------------------
            var itemEncoder = new WeatherForecastCustomV2Encoder();

            JArray jarray = new JArray();

            if (forecastCollection is object) // not null
            {
                foreach (var forecast in forecastCollection)
                {
                    // --------------------------------------------------------------------------------
                    // Create a new context specific to this item.
                    // Use the copy constructor of the EncoderContext to do a deep clone.
                    // Then overwrite the Object.
                    // --------------------------------------------------------------------------------
                    var itemContext = new EncoderContext(encoderContext)
                    {
                        Object = forecast,
                        ObjectType = typeof(WeatherForecast),
                    };

                    var encodedForecast = itemEncoder.EncodeToJToken(itemContext);
                    jarray.Add(encodedForecast);
                }
            }

            return jarray;
        }

    }
}
