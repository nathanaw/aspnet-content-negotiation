// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContentNegotiationExample.Formatters.Serialized
{
    public class WeatherForecastCollectionSerializedOutputFormatter : BaseJsonSerializedOutputFormatter
    {
        public const string WeatherForecastCollectionJsonV1 = "application/json; domain=Example.WeatherForecastCollection.Serialized; version=1";

        public WeatherForecastCollectionSerializedOutputFormatter()
            : this(new JsonSerializerOptions())
        {
        }

        public WeatherForecastCollectionSerializedOutputFormatter(JsonSerializerOptions jsonSerializerOptions)
            : base(jsonSerializerOptions, new MediaTypeHeaderValue[] { MediaTypeHeaderValue.Parse(WeatherForecastCollectionJsonV1) })
        {
        }

        protected override bool CanWriteType(Type type)
        {
            if (type.IsAssignableTo(typeof(IEnumerable<WeatherForecast>))){
                return true;
            }

            return false;
        }

        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return base.CanWriteResult(context);
        }

        public override Task WriteAsync(OutputFormatterWriteContext context)
        {
            return base.WriteAsync(context);
        }

    }
}
