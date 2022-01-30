using CodecExample.Client;
using CodecExample.Common;
using CodecExample.Common.Codecs.Custom;
using CodecExample.Common.Codecs.Serialized;
using Flurl.Http;

public class Program
{

    /// <summary>
    /// Uses the WeatherForecastClient to make various HTTP requests to the CodecExample server.
    /// </summary>
    public static async Task Main(string[] args)
    {
        // -------------------
        // Setup
        // -------------------

        LogHeading("Initializing Transcoder and client");
        var transcoder = GetTranscoder();
        var client = new WeatherForecastClient(transcoder);


        // -------------------
        // POST requests
        // -------------------

        LogHeading("Posting weather forecast - Send CUSTOM V1, Accept CUSTOM V2");
        var forecastToSend = new WeatherForecast() { Date = DateTime.UtcNow, Summary = "Raining json!", TemperatureC = 25 };
        LogForecasts(forecastToSend, "Sending Forecast:");
        var forecastPostReply = await client.PostForecastV1Custom(forecastToSend);
        LogForecasts(forecastPostReply);


        LogHeading("Posting weather forecasts - Send CUSTOM V1, Accept CUSTOM V2");
        var forecastsToSend = new WeatherForecast[] {
                                    new WeatherForecast() { Date = DateTime.UtcNow.AddDays(1), Summary = "Raining cats!", TemperatureC = 25 },
                                    new WeatherForecast() { Date = DateTime.UtcNow.AddDays(2), Summary = "Raining dogs!", TemperatureC = 30 },
                                    new WeatherForecast() { Date = DateTime.UtcNow.AddDays(3), Summary = "Raining pennies from heaven!", TemperatureC = 35 }
                                };
        LogForecasts(forecastsToSend, "Sending Forecasts:");
        var forecastsPostReply = await client.PostForecastV1Custom(forecastsToSend);
        LogForecasts(forecastsPostReply);


        LogHeading("Posting weather forecast - Send PROTOBUF V1, Accept PROTOBUF V2");
        var forecastToSendPB = new WeatherForecast() { Date = DateTime.UtcNow.AddDays(4), Summary = "Raining buffers!", TemperatureC = 10 };
        LogForecasts(forecastToSendPB, "Sending Forecast:");
        var forecastPostReplyPB = await client.PostForecastV1Protobuf(forecastToSendPB);
        LogForecasts(forecastPostReplyPB);



        // -------------------
        // GET requests
        // -------------------

        LogHeading("Fetching weather forecasts - CUSTOM V1");
        var forecasts1C = await client.GetForecastsV1Custom();
        LogForecasts(forecasts1C);

        LogHeading("Fetching weather forecasts - CUSTOM V1 (Over 30C)");
        var forecasts1CHot = await client.GetForecastsV1Custom(30);
        LogForecasts(forecasts1CHot);


        LogHeading("Fetching weather forecasts - CUSTOM V2");
        var forecasts2C = await client.GetForecastsV2Custom();
        LogForecasts(forecasts2C);


        LogHeading("Fetching weather forecasts - SERIALIZED V1");
        var forecasts1S = await client.GetForecastsV1Serialized();
        LogForecasts(forecasts1S);


        LogHeading("Fetching weather forecasts - CONTENT NEGOTIATION");
        var forecasts = await client.GetForecasts();
        LogForecasts(forecasts);


        LogHeading("Fetching weather forecasts - PROTOBUF V1");
        var forecast1PB = await client.GetForecastV1Protobuf(2);
        LogForecasts(forecast1PB);



        // -------------------
        // Demonstrate Errors
        // -------------------

        LogHeading("Fetching with bad accept header");
        try
        {
            var unacceptable = await client.GetUnacceptableMediaType();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Returned exception: {ex}");
        }


        Console.WriteLine("Press enter to exit.");
        Console.ReadLine();
    }

    private static void LogHeading(string value)
    {
        Console.WriteLine("\r\n" + new String('-', 80));
        Console.WriteLine(value);
    }

    private static void LogForecasts(IEnumerable<WeatherForecast> forecasts, string description = "Received Forecasts:")
    {
        Console.WriteLine(description);
        foreach (var forecast in forecasts)
        {
            Console.WriteLine($"\t{forecast.Date}  \tTemp: {forecast.TemperatureC} C  \tSummary: {forecast.Summary}.");
        }
    }

    private static void LogForecasts(WeatherForecast forecast, string description = "Received Forecast:")
    {
        Console.WriteLine(description);
        Console.WriteLine($"\t{forecast.Date}  \tTemp: {forecast.TemperatureC} C  \tSummary: {forecast.Summary}.");
    }

    /// <summary>
    /// Shows a typical client side setup of the transcoder.
    /// 
    /// Note:
    ///     1. We use a single instance of the transcoder
    ///     2. It has all known/supported codecs.
    ///     
    /// </summary>
    private static Transcoder GetTranscoder()
    {
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

        // V1 Protobuf Codecs
        transcoder.Encoders.Add(new CodecExample.Common.Protobuf.Codecs.WeatherForecastV1Encoder());
        transcoder.Decoders.Add(new CodecExample.Common.Protobuf.Codecs.WeatherForecastV1Decoder());
        transcoder.Encoders.Add(new CodecExample.Common.Protobuf.Codecs.WeatherForecastCollectionV1Encoder());
        transcoder.Decoders.Add(new CodecExample.Common.Protobuf.Codecs.WeatherForecastCollectionV1Decoder());

        // Other
        transcoder.Encoders.Add(new ValidationProblemsEncoder());

        return transcoder;
    }
}