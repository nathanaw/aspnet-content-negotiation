// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using System;
using System.Collections.Generic;
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

namespace CodecExample.Common.Codecs.Serialized
{
    /// <summary>
    /// A <see cref="TextInputFormatter"/> for JSON content that uses <see cref="JsonSerializer"/>.
    /// </summary>
    public class WeatherForecastSerializedV1Decoder : BaseJsonSerializationDecoder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WeatherForecastSerializedInputFormatter"/>.
        /// </summary>
        public WeatherForecastSerializedV1Decoder()
        {
            AddSupportedMediaType(WeatherForecastSerializedV1Encoder.WeatherForecastJsonV1MediaType);
        }

        public override bool CanReadType(Type type)
        {
            return type.IsAssignableFrom(typeof(WeatherForecast));
        }

    }
}