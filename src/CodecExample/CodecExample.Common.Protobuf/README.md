# Protobuf Codecs

These codecs demonstrate use of a binary codec, which can be much more
efficient (smaller) on the wire.

## Note About Async

The Google Protobuf libraries do not support async operations. This will
generate errors for Kestrel unless we turn on "AllowAsynchronousIO". However, 
this setting is meant to detect sync code for a reason.

The implementations in this example side-step this issue by using a second
stream. Clearly this is not ideal, but keeps the code simple for the sake 
of this example.

Ideally, protobuf should use async operations that read and write directly
to the input and output streams provided by Kestrel.