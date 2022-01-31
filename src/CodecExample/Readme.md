# Codecs Example

Introduces the concept of Encoders and Decoders (aka Codecs) so that we can reuse them in multiple 
places, including the input and output formatters in ASP.NET for content negotiation.

## Running the sample

### IDE

To run the sample:
1. Start the server by running the CodecExample project.
2. Start the client by running the CodecExample.Client project.
3. Run the below curl commands in a command prompt to observe output directly.


### Command Line

From the root of the CodecExample solution directory:

```bash
dotnet build

# Start the server
dotnet run --project CodecExample/CodecExample.csproj

# Start the client (separate command prompt)
# This will create/update records in the database
dotnet run --project CodecExample.Client/CodecExample.Client.csproj

# Get multi-day forecast using Version 1 CUSTOM media type and formatter.
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.WeatherForecastCollection.Custom;version=1"

```




## Formats

Below are examples of the json formats for a single weather forecast.
Note that they are different structures, but contain the same data.

### application/json; domain=Example.WeatherForecast.Custom; version=1

The "Custom" format and formatters use "custom" json parsing and json creation.
This is mostly accomplished via the Newtonsoft library's JObject.

```json
{
	"date": "2021-07-21T12:48:13.570Z",
	"temperature":
	{
		"celcius": 20,
		"farenheight": 67
	},
	"summary": "Lovely"
}
```

### application/json; domain=Example.WeatherForecast.Custom; version=2

The version 2 "Custom" format and formatters show how we can evolve the APIs
structures over time and support both flavors on the same endpoint.

```json
{
	"date": "2021-07-21T12:48:13.570Z",
	"temp":
	{
		"c": 20,
		"f": 67
	},
	"summary": "Lovely"
}
```

### application/json; domain=Example.WeatherForecast.Serialized; version=1

The "Serialized" format and formatters use the more automatic "binding" capabilities
of the json libraries. This infers the schema based on the class structure and matches
the values by naming conventions.

```json
{
	"Date": "2021-07-21T12:48:13.57Z",
	"TemperatureC": 20,
	"TemperatureF": 67,
	"Summary": "Lovely"
}
```


## Curl Commands

The following show some example curl commands for invoking the HTTP API. This is not 
an exhaustive list.

### Curl GET requests

```bash
# Get multi-day forecast using Version 1 CUSTOM media type and formatter.
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.WeatherForecastCollection.Custom;version=1"

# Get multi-day forecast using Version 2 CUSTOM media type and formatter.
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.WeatherForecastCollection.Custom;version=1"

# Get multi-day forecast using SERIALIZED media type and formatter.
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.WeatherForecastCollection.Serialized;version=1"

# Get multi-day forecast without an accept header. Server chooses and will return CUSTOM format because it is the first registered formatter
curl -i -X GET "https://localhost:5001/WeatherForecast"

# Trigger an HTTP 406 - Not Acceptable due to an unsupported media type.
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/xml"
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.Foo"
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.WeatherForecastCollection.Serialized;version=99"

# Get multi-day forecast using quality (q) parameters where multiple formatters are supported.
# The preferred format is the SERIALIZED format.
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.WeatherForecastCollection.Custom;version=1;q=0.1, application/json;domain=Example.WeatherForecastCollection.Serialized;version=1;q=0.9"

# Get multi-day forecast using quality (q) parameters where multiple formatters are supported.
# The preferred format is the CUSTOM format.
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.WeatherForecastCollection.Custom;version=1;q=0.9, application/json;domain=Example.WeatherForecastCollection.Serialized;version=1;q=0.1"

# Get requests for other endpoints
curl -i -X GET "https://localhost:5001/WeatherForecast/3" -H "Accept: application/json;domain=Example.WeatherForecast.Custom;version=1"
curl -i -X GET "https://localhost:5001/WeatherForecast/3" -H "Accept: application/protobuf;domain=Example.WeatherForecast;version=1"
curl -i -X GET "https://localhost:5001/WeatherForecast/NoProduces/3" -H "Accept: application/json; domain=Example.WeatherForecast.Custom; version=1, */*;q=0.1"
curl -i -X GET "https://localhost:5001/WeatherForecast/ProducesType/3" -H "Accept: application/json; domain=Example.WeatherForecast.Custom; version=1"

```

### Curl POST requests

```bash
# POST using the SERIALIZED format, but request the CUSTOM format in the response.
curl -i -X POST "https://localhost:5001/WeatherForecast/" \
  -H  "Accept: application/json; domain=Example.WeatherForecast.Custom; version=1" \
  -H  "Content-Type: application/json; domain=Example.WeatherForecast.Serialized; version=1" \
  -d "{\"Date\":\"2021-07-21T12:48:13.570Z\", \"TemperatureC\":20, \"Summary\":\"Lovely\" }"
```

```bash
# POST a collection of forecasts.
curl -i -X POST "https://localhost:5001/WeatherForecast/ConsumesCustomCollection/" \
  -H  "Accept: application/json; domain=Example.WeatherForecastCollection.Custom; version=1" \
  -H  "Content-Type: application/json; domain=Example.WeatherForecastCollection.Serialized; version=1" \
  -d "[{\"Date\":\"2021-07-21T12:48:13.570Z\", \"TemperatureC\":20, \"Summary\":\"Lovely\" }, {\"Date\":\"2021-07-22T12:48:13.570Z\", \"TemperatureC\":30, \"Summary\":\"Hot\" }]"
```
