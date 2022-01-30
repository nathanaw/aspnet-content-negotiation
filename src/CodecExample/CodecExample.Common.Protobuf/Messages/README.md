# Protobuf Serialization

The files in this folder are used by the Google Protobuf library
to serialize and deserialize protobuf payloads.

The *.proto files are processed at build time, and generate
a set of C# classes for the messages.

The generated classes are partial classes, allowing customization
as needed.