var url = @"ws://127.0.0.1:5759?pID=88861&sdkVersion=4.0.2&sdkLanguage=CSharp";

/*
var socket = new WebSocketSharp.WebSocket(url);
socket.Log.Level = LogLevel.Debug;
socket.Compression = CompressionMethod.None;
socket.Connect();
socket.Ping("PING");
*/

var ws = new System.Net.WebSockets.ClientWebSocket();
ws.ConnectAsync(new Uri(url), CancellationToken.None).GetAwaiter().GetResult();
//ws.ReceiveAsync()
Console.WriteLine(ws.State);
Console.WriteLine(ws.CloseStatus);

Console.WriteLine("The end");
