# Codec Client Example

Demonstrates:
1. Use of the Transcoder for decoding the HTTP Response representation into the resource instances.
2. Use of Accept header media types for content negotiation.
3. Use of the Flurl.Http library as a simple, clean way to make HTTP requests.



## Exmaple Output

```

--------------------------------------------------------------------------------
Initializing Transcoder and client

--------------------------------------------------------------------------------
Fetching weather forecasts - CUSTOM V1
Handling Response
        Request URI:           GET https://localhost:5001/WeatherForecast
        Request Accept:        application/json; Domain=Example.WeatherForecastCollection.Custom; Version=1
        Response Status:       200
        Response Content-Type: application/json; Domain=Example.WeatherForecastCollection.Custom; Version=1; charset=utf-8
Received Forecasts:
        1/9/2022 4:46:14 AM     Temp: 10 C      Summary: Hot.
        1/10/2022 4:46:14 AM    Temp: 36 C      Summary: Cool.
        1/11/2022 4:46:14 AM    Temp: 23 C      Summary: Balmy.
        1/12/2022 4:46:14 AM    Temp: -20 C     Summary: Cool.
        1/13/2022 4:46:14 AM    Temp: 31 C      Summary: Mild.

--------------------------------------------------------------------------------
Fetching weather forecasts - CUSTOM V2
Handling Response
        Request URI:           GET https://localhost:5001/WeatherForecast
        Request Accept:        application/json; Domain=Example.WeatherForecastCollection.Custom; Version=2
        Response Status:       200
        Response Content-Type: application/json; Domain=Example.WeatherForecastCollection.Custom; Version=2; charset=utf-8
Received Forecasts:
        1/9/2022 4:46:15 AM     Temp: 48 C      Summary: Scorching.
        1/10/2022 4:46:15 AM    Temp: 15 C      Summary: Cool.
        1/11/2022 4:46:15 AM    Temp: 2 C       Summary: Bracing.
        1/12/2022 4:46:15 AM    Temp: 35 C      Summary: Hot.
        1/13/2022 4:46:15 AM    Temp: 42 C      Summary: Warm.

--------------------------------------------------------------------------------
Fetching weather forecasts - SERIALIZED V1
Handling Response
        Request URI:           GET https://localhost:5001/WeatherForecast
        Request Accept:        application/json; Domain=Example.WeatherForecastCollection.Serialized; Version=1
        Response Status:       200
        Response Content-Type: application/json; Domain=Example.WeatherForecastCollection.Serialized; Version=1; charset=utf-8
Received Forecasts:
        1/9/2022 9:46:15 AM     Temp: 40 C      Summary: Cool.
        1/10/2022 9:46:15 AM    Temp: 45 C      Summary: Freezing.
        1/11/2022 9:46:15 AM    Temp: 6 C       Summary: Scorching.
        1/12/2022 9:46:15 AM    Temp: -17 C     Summary: Mild.
        1/13/2022 9:46:15 AM    Temp: 47 C      Summary: Chilly.

--------------------------------------------------------------------------------
Fetching weather forecasts - CONTENT NEGOTIATION
Handling Response
        Request URI:           GET https://localhost:5001/WeatherForecast
        Request Accept:        application/json; Domain=Example.WeatherForecastCollection.Custom; Version=2; q=0.900, application/json; Domain=Example.WeatherForecastCollection.Custom; Version=1; q=0.500, application/json; Domain=Example.WeatherForecastCollection.Serialized; Version=1; q=0.100
        Response Status:       200
        Response Content-Type: application/json; Domain=Example.WeatherForecastCollection.Custom; Version=2; charset=utf-8
Received Forecasts:
        1/9/2022 4:46:15 AM     Temp: 33 C      Summary: Freezing.
        1/10/2022 4:46:15 AM    Temp: 4 C       Summary: Warm.
        1/11/2022 4:46:15 AM    Temp: 27 C      Summary: Sweltering.
        1/12/2022 4:46:15 AM    Temp: 50 C      Summary: Mild.
        1/13/2022 4:46:15 AM    Temp: 10 C      Summary: Chilly.

--------------------------------------------------------------------------------
Posting weather forecast - Send CUSTOM V1, Accept CUSTOM V2
Sending Forecast:
        1/8/2022 9:46:15 AM     Temp: 25 C      Summary: Raining cats!.
Handling Response
        Request URI:           POST https://localhost:5001/WeatherForecast
        Request Accept:        application/json; Domain=Example.WeatherForecast.Custom; Version=2
        Response Status:       200
        Response Content-Type: application/json; Domain=Example.WeatherForecast.Custom; Version=2; charset=utf-8
Received Forecast:
        1/7/2022 11:46:15 PM    Temp: 25 C      Summary: Yep, it is Raining cats!.

--------------------------------------------------------------------------------
Posting weather forecasts - Send CUSTOM V1, Accept CUSTOM V2
Sending Forecasts:
        1/8/2022 9:46:16 AM     Temp: 25 C      Summary: Raining cats!.
        1/9/2022 9:46:16 AM     Temp: 30 C      Summary: Raining dogs!.
        1/10/2022 9:46:16 AM    Temp: 35 C      Summary: Raining pennies from heaven!.
Handling Response
        Request URI:           POST https://localhost:5001/WeatherForecast/ConsumesCustomCollection
        Request Accept:        application/json; Domain=Example.WeatherForecastCollection.Custom; Version=2
        Response Status:       200
        Response Content-Type: application/json; Domain=Example.WeatherForecastCollection.Custom; Version=2; charset=utf-8
Received Forecasts:
        1/7/2022 11:46:16 PM    Temp: 25 C      Summary: Yep, it is Raining cats!.
        1/8/2022 11:46:16 PM    Temp: 30 C      Summary: Yep, it is Raining dogs!.
        1/9/2022 11:46:16 PM    Temp: 35 C      Summary: Yep, it is Raining pennies from heaven!.

--------------------------------------------------------------------------------
Fetching with bad accept header
Returned exception: Flurl.Http.FlurlHttpException: Call failed with status code 406 (Not Acceptable): GET https://localhost:5001/WeatherForecast
   at Flurl.Http.FlurlRequest.HandleExceptionAsync(FlurlCall call, Exception ex, CancellationToken token)
   at Flurl.Http.FlurlRequest.SendAsync(HttpMethod verb, HttpContent content, CancellationToken cancellationToken, HttpCompletionOption completionOption)
   at Flurl.Http.FlurlRequest.SendAsync(HttpMethod verb, HttpContent content, CancellationToken cancellationToken, HttpCompletionOption completionOption)
   at CodecExample.Client.WeatherForecastClient.GetUnacceptableMediaType() in C:\src\github\nathanaw\examples\aspnet-content-negotiation\src\CodecExample\CodecExample.Client\WeatherForecastClient.cs:line 222
   at Program.Main(String[] args) in C:\src\github\nathanaw\examples\aspnet-content-negotiation\src\CodecExample\CodecExample.Client\Program.cs:line 77

```