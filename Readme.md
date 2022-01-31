﻿# Content Negotiation with ASP.NET

Examples in this repo show how to fully leverage content negotiation in ASP.NET. The focus
is primarily on usage for HTTP (REST) APIs. 

Examples and approaches assume understanding of HTTP, MediaTypes, and content negotiation 
via the Accept header. See [Content Negotiation - MDN](https://developer.mozilla.org/en-US/docs/Web/HTTP/Content_negotiation) 
for a good overview of the concepts.

The examples (below) demonstrate a progression of usage, starting with what is included
in the ASP.NET Core framework (see ContentNegotiationExample). However the primary
example is the Codec Example.


# Content Negotiation Example

Demonstrates how to use input and output formatters in ASP.NET for content negotiation.

This example is primarly aimed at showing what ASP.NET provides in the framework. However,
the framework's approach is challenging to use.

See [src/ContentNegotiationExample](src/ContentNegotiationExample) for the solution.

See the [ContentNegotiationExample Readme](src/ContentNegotiationExample/ContentNegotiationExample/Readme.md) for details on how to run and test the endpoints.


# Codecs Example

Introduce the concepts of codecs (encoders / decoders) and transcoders. These enable
us to use the formatting and parsing logic anywhere, including in client SDKs and/or
data persistence.

See [src/CodecExample](src/CodecExample) for the solution.

See the [CodecExample Readme](src/CodecExample/Readme.md) for details on how to run and test the server endpoints.

See the [CodecExample Client Readme](src/CodecExample/CodecExample.Client/README.md) for an example of using the transcoder and codecs on the client.

See the [CodecExample Sqlite Data Repository Readme](src/CodecExample/CodecExample.Data.Sqlite/README.md) for an example of using the transcoder and codecs when saving to the database.

See the [CodecExample Protobuf Readme](src/CodecExample/CodecExample.Common.Protobuf/README.md) for protobuf codec examples, which use a binary encoding for compact representations.



