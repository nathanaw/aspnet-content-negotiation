// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

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

namespace ContentNegotiationExample.Formatters.Custom
{
    public class WeatherForecastCustomOutputFormatter : BaseJsonCustomOutputFormatter
    {
        public const string WeatherForecastJsonV1 = "application/json; domain=Example.WeatherForecast.Custom; version=1";

        public WeatherForecastCustomOutputFormatter()
            : base(new MediaTypeHeaderValue[] { MediaTypeHeaderValue.Parse(WeatherForecastJsonV1) })
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

        protected override async Task WriteJsonToStream(OutputFormatterWriteContext context, TextWriter responseStream, object objectToWrite, Type objectType)
        {
            JToken jobject = EncodeToJToken(objectToWrite);

            var jsonWriter = new JsonTextWriter(responseStream);
            jsonWriter.Formatting = Formatting.Indented;
            await jobject.WriteToAsync(jsonWriter);
            await jsonWriter.FlushAsync();
        }

        public static JToken EncodeToJToken(object objectToWrite)
        {
            WeatherForecast forecast = (WeatherForecast)objectToWrite;
            JToken jobject = new JObject();

            // UTC date format. Full control. No ambiguity.
            jobject["date"] = forecast.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            // Structure the json as works best for json. Doesn't have to match object exactly.
            var temperatureNode = jobject["temperature"] = new JObject();
            temperatureNode["celcius"] = forecast.TemperatureC;
            temperatureNode["farenheight"] = forecast.TemperatureF;

            jobject["summary"] = forecast.Summary;
            return jobject;
        }
    }
}
