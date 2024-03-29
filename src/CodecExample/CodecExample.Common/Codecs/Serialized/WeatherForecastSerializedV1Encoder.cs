﻿// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using CodecExample.Common;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodecExample.Common.Codecs.Serialized
{
    public class WeatherForecastSerializedV1Encoder : BaseJsonSerializationEncoder
    {
        public const string WeatherForecastJsonV1MediaType = "application/json; Domain=Example.WeatherForecast.Serialized; Version=1";

        public WeatherForecastSerializedV1Encoder()
        {
            AddSupportedMediaType(WeatherForecastJsonV1MediaType);
        }


        protected override bool CanWriteType(Type type)
        {
            return typeof(WeatherForecast).IsAssignableFrom(type);
        }


    }
}
