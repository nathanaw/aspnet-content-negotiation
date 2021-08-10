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
    public class WeatherForecastCollectionCustomOutputFormatter : BaseJsonCustomOutputFormatter
    {
        public const string WeatherForecastCollectionJsonV1 = "application/json; domain=Example.WeatherForecastCollection.Custom; version=1";

        public WeatherForecastCollectionCustomOutputFormatter()
            : base(new MediaTypeHeaderValue[] { MediaTypeHeaderValue.Parse(WeatherForecastCollectionJsonV1) })
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

        protected async override Task WriteJsonToStream(OutputFormatterWriteContext context, TextWriter responseStream, object objectToWrite, Type objectType)
        {
            JToken jobject = EncodeToJToken(objectToWrite);

            var jsonWriter = new JsonTextWriter(responseStream);
            jsonWriter.Formatting = Formatting.Indented;
            await jobject.WriteToAsync(jsonWriter);
            await jsonWriter.FlushAsync();
        }

        public static JToken EncodeToJToken(object objectToWrite)
        {
            var forecastCollection = (IEnumerable<WeatherForecast>)objectToWrite;

            JArray jarray = new JArray();

            if (forecastCollection is object) // not null
            {
                foreach (var forecast in forecastCollection)
                {
                    var encodedForecast = WeatherForecastCustomOutputFormatter.EncodeToJToken(forecast);
                    jarray.Add(encodedForecast);
                }
            }

            return jarray;
        }
    }
}
