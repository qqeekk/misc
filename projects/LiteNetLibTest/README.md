## LiteNetLib Test

This is the project to try [LiteNetLib](https://github.com/RevenantX/LiteNetLib) project. The Avalonia desktop app shows two buttons and syncs its coordinates between client and server. To test run the project. There are two options available:

- `--server`. Run as server. Two buttons are available to move.
- `--client <host>`. Run as client. The client connects to host to get latest buttons positions. If host is not specified - the localhost is used.
