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
    public class WeatherForecastSerializedOutputFormatter : BaseJsonSerializedOutputFormatter
    {
        public const string WeatherForecastJsonV1 = "application/json; domain=Example.WeatherForecast.Serialized; version=1";

        public WeatherForecastSerializedOutputFormatter()
            : this(new JsonSerializerOptions())
        {
        }

        public WeatherForecastSerializedOutputFormatter(JsonSerializerOptions jsonSerializerOptions)
            : base(jsonSerializerOptions, new MediaTypeHeaderValue[] { MediaTypeHeaderValue.Parse(WeatherForecastJsonV1) })
        {
        }

        protected override bool CanWriteType(Type type)
        {
            if (type.IsAssignableTo(typeof(WeatherForecast)))
            {
                Console.WriteLine("WeatherForecastOutputFormatter.CanWriteType() - Returning true because types match. Type={0}", type);
                return true;
            }

            Console.WriteLine("WeatherForecastOutputFormatter.CanWriteType() - Returning false. Type={0}", type);
            return false;
        }


        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            Console.WriteLine("WeatherForecastOutputFormatter.CanWriteResult() - Initial - ContentType={0}, ServerDefined={1}", context.ContentType, context.ContentTypeIsServerDefined);

            var canWriteResult = base.CanWriteResult(context);

            Console.WriteLine("WeatherForecastOutputFormatter.CanWriteResult() - After CanWriteResult - ContentType={0}, ServerDefined={1}", context.ContentType, context.ContentTypeIsServerDefined);
            Console.WriteLine("WeatherForecastOutputFormatter.CanWriteResult() - Returning {0}", canWriteResult);

            return canWriteResult;
        }

        public override Task WriteAsync(OutputFormatterWriteContext context)
        {
            Console.WriteLine("WeatherForecastOutputFormatter.WriteResult()");

            return base.WriteAsync(context);
        }

    }
}
