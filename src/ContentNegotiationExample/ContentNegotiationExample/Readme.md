﻿# Content Negotiation Example

Demonstrate how to use input and output formatters in ASP.NET for content negotiation.


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
	"summary": "Yep, it is Lovely"
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
	"Summary": "Yep, it is Lovely"
}
```


## Curl Commands

The following show some example curl commands for invoking the HTTP API. This is not 
an exhaustive list.

### Curl GET requests

```bash
# Get multi-day forecast using CUSTOM media type and formatter.
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.WeatherForecastCollection.Custom;version=1"

# Get multi-day forecast using SERIALIZED media type and formatter.
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.WeatherForecastCollection.Serialized;version=1"

# Get multi-day forecast without an accept header. Server chooses and will return CUSTOM format because it is the first registered formatter
curl -i -X GET "https://localhost:5001/WeatherForecast"

# Trigger an HTTP 406 - Not Acceptable due to an unsupported media type.
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/xml"
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.Foo;"
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.WeatherForecastCollection.Serialized;version=2"

# Get multi-day forecast using quality (q) parameters where multiple formatters are supported.
# The preferred format is the SERIALIZED format.
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.WeatherForecastCollection.Custom;version=1;q=0.1, application/json;domain=Example.WeatherForecastCollection.Serialized;version=1;q=0.9"

# Get multi-day forecast using quality (q) parameters where multiple formatters are supported.
# The preferred format is the CUSTOM format.
curl -i -X GET "https://localhost:5001/WeatherForecast" -H  "Accept: application/json;domain=Example.WeatherForecastCollection.Custom;version=1;q=0.9, application/json;domain=Example.WeatherForecastCollection.Serialized;version=1;q=0.1"

# Get requests for other endpoints
curl -i -X GET "https://localhost:5001/WeatherForecast/3" -H "Accept: application/json;domain=Example.WeatherForecast;version=1"
curl -i -X GET "https://localhost:5001/WeatherForecast/NoProduces/3" -H "Accept: application/json; domain=Example.WeatherForecast; version=1, */*;q=0.1"
curl -i -X GET "https://localhost:5001/WeatherForecast/ProducesType/3" -H "Accept: application/json; domain=Example.WeatherForecast; version=1"

```

### Curl POST requests

```bash
# POST using the SERIALIZED format, but request the CUSTOM format in the response.
curl -i -X POST "https://localhost:5001/WeatherForecast/" \
  -H  "Accept: application/json; domain=Example.WeatherForecast.Custom; version=1" \
  -H  "Content-Type: application/json; domain=Example.WeatherForecast.Serialized; version=1" \
  -d "{\"date\":\"2021-07-21T12:48:13.570Z\", \"temperatureC\":20, \"summary\":\"Lovely\" }"
```

