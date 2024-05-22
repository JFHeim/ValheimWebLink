using System.Net.Sockets;
using ValheimWebLink;
using ValheimWebLink.Web;
using WebSocketSharp.Server;

namespace VWL_PlayerEye;

public static class ServerLogic
{
    internal static WebSocketServer ws;

    public static void Init()
    {
        WebApiManager.AddRouterInfo(new()
        {
            route = "/playerEye-source",
            protocol = "ws",
            description = "For internal use only. Receives map texture from client",
            queryParameters = [],
            RequiresAuth = false
        });

        // TODO: I don't know if localhost will work on a real server.
        var url = $"ws://{IP}:{SettingsManager.instance.wsPort}";
        Debug($"Starting WebSocket server on '{url}'...");
        ws = new WebSocketServer(url);
        ws.WaitTime = TimeSpan.FromSeconds(1);
        ws.KeepClean = true;
        ws.AddWebSocketService<PlayerEyeSource>("/playerEye-source");
        ws.AddWebSocketService<PlayerEyeSocket>("/playerEye");
        ws.Start();

        Debug("WebSocket server started.");
    }
}

public class PlayerEyeSocket : WebSocketBehavior
{
    public string plName;
    private static readonly List<PlayerEyeSocket> _sockets = [];

    protected override void OnOpen()
    {
        base.OnOpen();
        DebugWarning($"PlayerEyeSocket opened");
        _sockets.Add(this);
        // Debug($"_sockets = {_sockets.Select(x => (x != null ? x.plName : "null")).ToList().GetObjectString()}");
    }

    protected override void OnClose(CloseEventArgs e)
    {
        base.OnClose(e);
        DebugWarning($"PlayerEyeSocket closed");
        _sockets.Remove(this);
    }

    private bool gotPong = false;

    protected override void OnMessage(MessageEventArgs e)
    {
        if (e.Data == "pong")
        {
            gotPong = true;
            return;
        } else gotPong = false;

        var data = JSON.ToObject<EyeData>(e.Data);

        Debug($"Server.PlayerEyeSocket: Received \n'{data}'\n from {plName}");
        this.plName = data.plName;
        if (plName.IsGood())
        {
            Debug($"Calling VWL_RequestEye rpc for player '{plName}'");
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "VWL_RequestEye",
                data.plName, $"{IP}:{SettingsManager.instance.wsPort}");
        }
    }

    public static async void Update(EyeData data)
    {
        var sockets = _sockets.FindAll(x => x.plName == data.plName);
        foreach (var socket in sockets)
        {
            socket.Send(JSON.ToJSON(data));
        }

        foreach (var socket in sockets)
        {
            socket.Send("ping");
            int waited = 0;
            while (true)
            {
                await Task.Delay(100);
                waited += 100;
                if (socket.gotPong) return;
                if (waited > 3000) break;
            }

            DebugWarning($"Found offline socket -> closing");
            _sockets.Remove(socket);
            socket.Close();
        }

        sockets = _sockets.FindAll(x => x.plName == data.plName);
        if (sockets.Count == 0)
        {
            Debug($"No sockets found for player '{data.plName}' -> calling VWL_StopEye rpc");
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "VWL_StopEye", data.plName);
        }
    }
}

public class PlayerEyeSource : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        var data = JSON.ToObject<EyeData>(e.Data);
        Debug($"Server.PlayerEyeSource: Received \n'{data}'\n from {data.plName}");
        PlayerEyeSocket.Update(data);
    }
}