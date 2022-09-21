using System;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using LiteNetLibTest.Media.Input;
using LiteNetLibTest.Media.Output;

namespace LiteNetLibTest;

public partial class MainWindow : Window
{
    private const int Port = 12045;
    private const string ConnectionKey = "Test";
    private readonly static object lo = new();

    private readonly EventBasedNetListener listener = new();
    private NetManager? server;
    private NetManager? client;
    private DispatcherTimer? timer;

    private Control? controlUnderMoving;
    private Point controlStartMousePosition;
    private MemoryStream stream;
    private readonly TimeSpan timerInterval = TimeSpan.FromMilliseconds(60);

    public MainWindow()
    {
        InitializeComponent();
        NetDebug.Logger = new ConsoleLogger();

        this.Closed += OnClosed;
        this.AddHandler(PointerMovedEvent, (sender, args) =>
        {
            if (controlUnderMoving == null)
            {
                return;
            }
            var pos = args.GetPosition(this);
            controlUnderMoving.SetValue(Canvas.LeftProperty, pos.X - controlStartMousePosition.X);
            controlUnderMoving.SetValue(Canvas.TopProperty, pos.Y - controlStartMousePosition.Y);
        }, handledEventsToo: true);
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        timer?.Stop();
        server?.Stop(true);
        client?.Stop(true);
        stream?.Dispose();
    }

    public void InitializeAsServer()
    {
        server = new NetManager(listener);
        var result = server.Start(Port);
        if (!result)
        {
            throw new InvalidOperationException("Cannot start server!");
        }
        Console.WriteLine($"Server started at port {server.LocalPort}.");

        // Events.
        listener.ConnectionRequestEvent += request =>
        {
            Console.WriteLine($"Connection request {request.RemoteEndPoint}.");
            request.AcceptIfKey(ConnectionKey);
        };

        var writer = new NetDataWriter();

        var scence = this.FindControl<Canvas>("Scene");
        var objects = scence.Children.Cast<Control>().Where(o => o != null).ToArray();

        var gameRecorder = new GameRecorder(objects);
        var microphone = new MicrophoneSimulator("unencoded.raw", sampleRate: 44100);
        var queue = new OggDataInput(microphone, gameRecorder);

        // Start recording.
        microphone.Start();

        // Start timer.
        bool inProcess = false;
        timer = new DispatcherTimer(timerInterval, DispatcherPriority.Normal, (sender, args) =>
        {
            if (inProcess)
            {
                return;
            }
            inProcess = true;

            lock (lo)
            {
                gameRecorder.PollState();
                var data = queue.Flush();
                
                writer.Reset();
                writer.PutBytesWithLength(data);
            }

            server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
            server.PollEvents();
            inProcess = false;
        });
        timer.Start();

        // Attach events.
        foreach (var obj in objects)
        {
            obj.AddHandler(PointerPressedEvent, (sender, args) =>
            {
                controlUnderMoving = sender as Control;
                controlStartMousePosition = args.GetPosition(controlUnderMoving);
            }, handledEventsToo: true);
            obj.AddHandler(PointerReleasedEvent, (sender, args) =>
            {
                controlUnderMoving = null;
            }, handledEventsToo: true);
        }

        Title = "Server";
    }

    public void InitializeAsClient(string host)
    {
        client = new NetManager(listener);
        var result = client.Start();
        if (!result)
        {
            throw new InvalidOperationException("Cannot start client!");
        }
        var peer = client.Connect(host, Port, ConnectionKey);

        var objects = this.FindControl<Canvas>("Scene").Children
            .Cast<Control>().Where(o => o != null).ToArray();

        var player = new OggAudioPlayer(MicrophoneSimulator.VorbisStreamSerialNo, sampleRate: 44100);
        var gameWriter = new GameWriter(GameRecorder.MetadataStreamSerialNo, objects);
        var outputs = new OggDataOutput(gameWriter, player);

        // Events.
        bool inProcess = false;
        ThreadPool.QueueUserWorkItem(_ => player.PlayAsync());
        listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
        {
            outputs.ReceiveBytes(dataReader.GetBytesWithLength());
        };
        listener.ConnectionRequestEvent += request =>
        {
            Console.WriteLine($"Connection request {request.RemoteEndPoint}.");
            request.AcceptIfKey(ConnectionKey);
        };
        listener.PeerConnectedEvent += peer =>
        {
            Console.WriteLine($"Connected {peer.EndPoint}.");
        };

        // Start timer.
        timer = new DispatcherTimer(timerInterval, DispatcherPriority.Normal, (sender, args) =>
        {
            if (inProcess)
            {
                return;
            }
            inProcess = true;
            client.PollEvents();
            inProcess = false;
        });
        timer.Start();
        
        Title = "Client";
    }
}
