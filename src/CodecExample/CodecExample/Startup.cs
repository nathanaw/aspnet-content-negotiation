// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodecExample.Codecs.Custom;
using CodecExample.Codecs.Serialized;
using CodecExample.Common;
using CodecExample.Common.Formatters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace CodecExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // -----------------------
            // Create and hydrate the transcoder.
            // -----------------------

            var transcoder = new Transcoder();

            // V1 Custom Codecs
            transcoder.Encoders.Add(new WeatherForecastCustomV1Encoder());
            transcoder.Encoders.Add(new WeatherForecastCollectionCustomV1Encoder());
            transcoder.Decoders.Add(new WeatherForecastCustomV1Decoder());
            transcoder.Decoders.Add(new WeatherForecastCollectionCustomV1Decoder());

            // V2 Custom Codecs
            transcoder.Encoders.Add(new WeatherForecastCustomV2Encoder());
            transcoder.Encoders.Add(new WeatherForecastCollectionCustomV2Encoder());
            transcoder.Decoders.Add(new WeatherForecastCustomV2Decoder());
            transcoder.Decoders.Add(new WeatherForecastCollectionCustomV2Decoder());

            // V1 Serilization-based Codecs
            transcoder.Encoders.Add(new WeatherForecastSerializedV1Encoder());
            transcoder.Encoders.Add(new WeatherForecastCollectionSerializedV1Encoder());
            transcoder.Decoders.Add(new WeatherForecastSerializedV1Decoder());
            transcoder.Decoders.Add(new WeatherForecastCollectionSerializedV1Decoder());

            // Other
            transcoder.Encoders.Add(new ValidationProblemsEncoder());

            services.AddSingleton<Transcoder>(transcoder);


            services.AddControllers(options =>
            {
                // -----------------------
                // Output formatters / Encoders
                // -----------------------

                // For the example, remove the built-in formatter. 
                options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();

                options.OutputFormatters.Add(new TranscoderOutputFormatter(transcoder));

                // -----------------------
                // Input formatters / Decoders
                // -----------------------

                // For the example, remove the built-in formatter. 
                options.InputFormatters.RemoveType<SystemTextJsonInputFormatter>();

                options.InputFormatters.Add(new TranscoderInputFormatter(transcoder));

                // -----------------------
                // Relevant settings
                // -----------------------

                // Gets or sets the flag which causes content negotiation to ignore Accept header
                // when it contains the media type */*. 
                // false by default.
                // 
                // Set to true if the server wants to comprehend the valid case of a */* media type.
                // Recommended value: true
                options.RespectBrowserAcceptHeader = true;

                // Gets or sets the flag which decides whether an HTTP 406 Not Acceptable response
                // will be returned if no formatter has been selected to format the response. 
                // false by default.
                // 
                // Set to true if the server wants to be strict about supported media types.
                // Recommended value: true
                options.ReturnHttpNotAcceptable = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CodecExample", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodecExample v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
