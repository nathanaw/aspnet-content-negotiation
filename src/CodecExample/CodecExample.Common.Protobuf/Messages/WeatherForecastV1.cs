using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodecExample.Common.Protobuf.Messages
{

    /// <summary>
    /// Extend the generated protobuf partial classes.
    /// </summary>
    internal sealed partial class WeatherForecastV1
    {
        /// <summary>
        /// Create and map from the resource.
        /// </summary>
        /// <param name="weatherForecast">The resource values to map.</param>
        public WeatherForecastV1(Common.WeatherForecast resource)
        {
            Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(resource.Date);
            TempCelcius = resource.TemperatureC;
            TempFarenheight = resource.TemperatureF;
            Summary = resource.Summary;            
        }

        /// <summary>
        /// Convert message into resource.
        /// </summary>
        public WeatherForecast ToResource()
        {
            return new WeatherForecast()
            {
                Date = this.Date.ToDateTime(),
                TemperatureC = this.TempCelcius,
                Summary = this.Summary,
            };
        }
    }
}
