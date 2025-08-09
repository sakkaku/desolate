# Message Spec

The socket will send messages consisting of a type, size, then the contents.

- message type (32bit int)
- message size (32bit int) max size is 8MiB
- message (bytes equal to message size)