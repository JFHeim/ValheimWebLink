namespace VWL_PlayerEye;

public static class ClientLogic
{
    public static string plName;

    private static WebSocket ws;
    // public static Camera eyeCamera;
    // public static RenderTexture eyeTexture;

    private static CancellationTokenSource _cancellationTokenSource;

    public static void ConnectToServer(string plName, string serverUrl)
    {
        ClientLogic.plName = plName;

        _cancellationTokenSource = new CancellationTokenSource();
        try
        {
            var url = $"ws://{serverUrl}/playerEye-source";
            Debug($"Connecting to server '{url}' with plName = {plName}");
            ws = new WebSocket(url);
            ws.WaitTime = TimeSpan.FromSeconds(1);
            ws.OnMessage += (_, e) => DebugWarning($"Server says: {e.Data}");
            ws.Connect();

            DebugWarning("Client: Sending hello to server...");
            ws.Send(JSON.ToJSON(new EyeData()
            {
                plName = plName,
                message = "Hello Server!✋"
            }));
        }
        catch (Exception ex)
        {
            DebugError($"Failed to connect to server. Exception: {ex}");
            return;
        }

        Task.Run(StartSending, _cancellationTokenSource.Token);
    }

    private static async Task StartSending()
    {
        try
        {
            while (true)
            {
                if (_cancellationTokenSource.IsCancellationRequested) break;
                //TODO: Custom map update interval
                await Task.Delay(updateInterval.Value);

                DebugWarning("Client: Sending map update to server...");

                var textureData = ScreenApi.GetScreenshot();

                var eyeData = new EyeData()
                {
                    plName = plName,
                    message = "Hey server!👋 I got a map update!",
                    textureData = textureData
                };

                ws.Send(JSON.ToJSON(eyeData));
            }
        }
        catch (Exception e)
        {
            DebugError($"Failed to start sending. Exception: {e}");
        }
    }

    public static void StopEye()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        ws.Close();
    }
}