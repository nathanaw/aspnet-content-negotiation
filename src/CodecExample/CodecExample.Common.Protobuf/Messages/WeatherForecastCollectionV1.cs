namespace CodecExample.Common.Protobuf.Messages
{
    /// <summary>
    /// Extend the generated protobuf partial classes.
    /// </summary>
    internal sealed partial class WeatherForecastCollectionV1
    {
        /// <summary>
        /// Create and map from the resources.
        /// </summary>
        /// <param name="weatherForecast">The resource values to map.</param>
        public WeatherForecastCollectionV1(IEnumerable<Common.WeatherForecast> resources)
        {
            var messages = resources.Select(r => new WeatherForecastV1(r));

            this.Forecasts.AddRange(messages);
        }

        /// <summary>
        /// Convert message collection into resource list.
        /// </summary>
        public IEnumerable<WeatherForecast> ToResources()
        {
            return this.Forecasts.Select(r => r.ToResource()).ToList();
        }
    }
}
