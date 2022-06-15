using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LiteNetLibTest;

public partial class MainWindow : Window
{
    private const int Port = 12045;
    private const string ConnectionKey = "Test";

    private readonly EventBasedNetListener listener = new();
    private readonly NetPacketProcessor netPacketProcessor = new();
    private NetManager? server;
    private NetManager? client;
    private DispatcherTimer? timer;

    private Control? controlUnderMoving;
    private Point controlStartMousePosition;
    private readonly TimeSpan timerInterval = TimeSpan.FromMilliseconds(50);

    public MainWindow()
    {
        InitializeComponent();
        NetDebug.Logger = new ConsoleLogger();
        netPacketProcessor.RegisterNestedType(() => new GameObject());

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
        var objects = this.FindControl<Canvas>("Scene").Children
            .Cast<Control>().Where(o => o != null).ToArray();

        // Start timer.
        bool inProcess = false;
        timer = new DispatcherTimer(timerInterval, DispatcherPriority.Normal, (sender, args) =>
        {
            if (inProcess)
            {
                return;
            }
            inProcess = true;
            writer.Reset();

            for (int i = 0; i < objects.Length; i++)
            {
                var go = new GameObject
                {
                    Id = objects[i].Name,
                    Left = objects[i].GetValue(Canvas.LeftProperty),
                    Top = objects[i].GetValue(Canvas.TopProperty),
                };
                netPacketProcessor.WriteNetSerializable(writer, go);
            }
            server.SendToAll(writer, DeliveryMethod.Unreliable);
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

        // Events.
        bool inProcess = false;
        netPacketProcessor.SubscribeReusable<GameObject, NetPeer>((go, netPeer) =>
        {
            var obj = objects.FirstOrDefault(o => o.Name == go.Id);
            if (obj == null)
            {
                return;
            }
            obj.SetValue(Canvas.LeftProperty, go.Left);
            obj.SetValue(Canvas.TopProperty, go.Top);
        });
        listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
        {
            netPacketProcessor.ReadAllPackets(dataReader, fromPeer);
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
