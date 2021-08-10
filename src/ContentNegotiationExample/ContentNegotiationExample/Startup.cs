// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentNegotiationExample.Formatters;
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

namespace ContentNegotiationExample
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

            services.AddControllers(options =>
            {
                // -----------------------
                // Output formatters
                // -----------------------

                // For the example, remove the built-in formatter. 
                options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();

                // Prefer "custom" formatters, by adding them first.
                options.OutputFormatters.Add(new Formatters.Custom.WeatherForecastCustomOutputFormatter());
                options.OutputFormatters.Add(new Formatters.Custom.WeatherForecastCollectionCustomOutputFormatter());

                // Also support "serialized" formatters.
                options.OutputFormatters.Add(new Formatters.Serialized.WeatherForecastSerializedOutputFormatter());
                options.OutputFormatters.Add(new Formatters.Serialized.WeatherForecastCollectionSerializedOutputFormatter());


                // -----------------------
                // Input formatters
                // -----------------------

                // For the example, remove the built-in formatter. 
                options.InputFormatters.RemoveType<SystemTextJsonInputFormatter>();

                // Prefer "custom" formatters, by adding them first.
                options.InputFormatters.Add(new Formatters.Custom.WeatherForecastCustomInputFormatter());
                options.InputFormatters.Add(new Formatters.Custom.WeatherForecastCollectionCustomInputFormatter());

                // Also support "serialized" formatters.
                options.InputFormatters.Add(new Formatters.Serialized.WeatherForecastSerializedInputFormatter());
                options.InputFormatters.Add(new Formatters.Serialized.WeatherForecastCollectionSerializedInputFormatter());

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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ContentNegotiationExample", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContentNegotiationExample v1"));
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
