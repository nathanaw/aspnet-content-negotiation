// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using CodecExample.Common;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodecExample.Codecs.Serialized
{
    public class WeatherForecastCollectionSerializedV1Encoder : BaseJsonSerializationEncoder
    {
        public const string WeatherForecastCollectionJsonV1MediaType = "application/json; Domain=Example.WeatherForecastCollection.Serialized; Version=1";

        public WeatherForecastCollectionSerializedV1Encoder()
        {
            AddSupportedMediaType(WeatherForecastCollectionJsonV1MediaType);
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(IEnumerable<WeatherForecast>).IsAssignableFrom(type);
        }

    }
}
