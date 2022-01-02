# Content Negotiation with ASP.NET

Examples in this repo show how to fully leverage content negotiation in ASP.NET. The focus
is primarily on usage for HTTP (REST) APIs. 

Examples and approaches assume understanding of HTTP, MediaTypes, and content negotiation 
via the Accept header. See [Content Negotiation - MDN](https://developer.mozilla.org/en-US/docs/Web/HTTP/Content_negotiation) 
for a good overview of the concepts.

The examples (below) demonstrate a progression of usage, starting with what is included
in the ASP.NET Core framework (see ContentNegotiationExample).

# Content Negotiation Example

Demonstrates how to use input and output formatters in ASP.NET for content negotiation.

See [src/ContentNegotiationExample](src/ContentNegotiationExample) for the solution.

See the [ContentNegotiationExample Readme](src/ContentNegotiationExample/ContentNegotiationExample/Readme.md) for details on how to run and test the endpoints.


# Codecs Example

Introduce the concepts of codecs (encoders / decoders) and transcoders. These enable
us to use the formatting and parsing logic anywhere, including in client SDKs and/or
data persistence.

See [src/CodecExample](src/CodecExample) for the solution.

See the [CodecExample Readme](src/CodecExample/CodecExample/Readme.md) for details on how to run and test the endpoints.


