using BepInEx;
using ValheimWebLink.Web.Controllers;

namespace ValheimWebLink.Web;

public static class WebApiManager
{
    public static HashSet<IController> Controllers { get; private set; } = [];
    private static HttpListener _listener;
    public static CancellationTokenSource _cancellationTokenSource { get; private set; }
    public static HttpListener Listener => _listener;

    public static void Init()
    {
        var port = SettingsManager.instance.httpPort;
        Debug($"Starting Web API server on port {port}...", ConsoleColor.Green);
        //TODO: way to get this machine ip address
        // if (IP != null) Debug($"Ip address: {IP}", ConsoleColor.Green);
        // else Debug("Failed to get ip address", ConsoleColor.Magenta);
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://*:{port}/");

        _cancellationTokenSource = new CancellationTokenSource();

        FindControllers();

        var listenerThread = new Thread(() => Listen(_cancellationTokenSource.Token));
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    private static void FindControllers()
    {
        List<Type> types = [];
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) types.AddRange(assembly.GetTypes());

        foreach (var type in types)
        {
            if (Attribute.IsDefined(type, typeof(ControllerAttribute)))
            {
                var instance = Activator.CreateInstance(type);
                var controller = (IController)instance;
                if (Controllers.ToList()
                    .Exists(x => x.HttpMethod == controller.HttpMethod && x.Route == controller.Route))
                {
                    Debug($"Controller {type.Name} already exists. Skipping...", ConsoleColor.Yellow);
                    continue;
                }

                Controllers.Add(instance as IController);
            }
        }

        Controllers = Controllers
            .OrderBy(x => x.Route.Length)
            .ThenBy(x => x.HttpMethod)
            .ThenBy(x => x.Route.Equals("/"))
            .ToHashSet();
    }

    public static void Stop()
    {
        Debug("Stopping server...");
        _cancellationTokenSource?.Cancel();
        _listener?.Stop();
    }

    private static async void Listen(CancellationToken cancellationToken)
    {
        try
        {
            _listener.Start();
            Debug($"\n"
                  + $"Some requests required authentication\n"
                  + $"Your login and password are stored in the file 'auth.json'\n"
                  + $"You MUST edit it before running the server\n"
                  + $"\n\n");

            while (!cancellationToken.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync();

                _ = Task.Run(() => HandleRequest(context, cancellationToken), cancellationToken);
            }
        }
        catch (HttpListenerException ex) when (ex.ErrorCode == 995)
        {
            // Игнорировать HttpListenerException с кодом ошибки 995 (Операция отменена)
            DebugWarning($"Exception(995): {ex.Message}");
        }
        catch (OperationCanceledException ex)
        {
            // Обработка отмены операции
            DebugWarning($"Operation was canceled. {ex.Message}");
        }
        catch (Exception ex)
        {
            // Обработка других исключений
            DebugError($"Unhandled exception:\n{ex}");
        }
        finally
        {
            _listener.Close();
            Debug("Stopped listening for requests");
        }
    }

    private static async Task HandleRequest(HttpListenerContext context, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            DebugWarning("HandleRequest: Request was cancelled");
            return;
        }

        var request = context.Request;
        var url = request.Url;
        var httpMethod = request.HttpMethod;
        var absolutePath = url.AbsolutePath;
        var response = context.Response;

        if (request.HttpMethod == "OPTIONS")
        {
            response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
            response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
            response.AddHeader("Access-Control-Max-Age", "1728000");
        }

        response.AppendHeader("Access-Control-Allow-Origin", "*");

        foreach (var controller in Controllers)
            if ((httpMethod == controller.HttpMethod || controller.HttpMethod == "OPTIONS")
                && absolutePath == controller.Route)
            {
                Debug($"Got {absolutePath} request, url: {url}");
                try
                {
                    if (httpMethod == "OPTIONS")
                    {
                        SendResponce(response, OK, "application/json", new RouteInfoJSON(controller));
                        break;
                    }

                    var isAuthed = IsAuthed(request, controller.RequiredPermissions, out var user);
                    if (!isAuthed)
                    {
                        SendResponce(response, Unauthorized);
                        break;
                    }

                    await controller.HandleRequest(request, response, GetQueryParameters(request), user.permissions);
                }
                catch (NotImplementedException)
                {
                    SendResponce(response, NotImplemented);
                }
                catch (Exception e)
                {
                    DebugError($"Request handler threw an exception: {e}");
                    SendResponce(response, 500, "application/json", new WebExceptionJSON(e));
                }

                response.Close();
                return;
            }

        if (Controllers.ToList().Exists(x => x.Route == absolutePath))
        {
            SendResponce(response, MethodNotAllowed);
            return;
        }


        SendResponce(response, BadRequest);
        response.Close();
    }

    public static bool IsAuthed(HttpListenerRequest request, List<Permission> permissions, out User user)
    {
        user = null;
        if (AuthDataManager.instance == null)
        {
            Debug("AuthData is null", ConsoleColor.Red);
            return false;
        }

        var authHeader = request.Headers.Get("Authorization");
        if (!authHeader.IsGood()) return false;
        var authData = ExtractUsernameAndPassword(authHeader);

        user = AuthDataManager.GetUser(authData.username);
        return AuthDataManager.IsAuthed(authData.username, authData.password, permissions);
    }

    private static (string username, string password) ExtractUsernameAndPassword(string authHeader)
    {
        string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
        Encoding encoding = Encoding.GetEncoding("iso-8859-1");
        string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

        int separatorIndex = usernamePassword.IndexOf(':');
        string username = usernamePassword.Substring(0, separatorIndex);
        string password = usernamePassword.Substring(separatorIndex + 1);

        return new(username, password);
    }

    private static Dictionary<string, string> GetQueryParameters(HttpListenerRequest request)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();

        if (request == null)
            return parameters;

        string queryParams = Uri.UnescapeDataString(request.Url.Query);

        if (!string.IsNullOrEmpty(queryParams))
        {
            string[] pairs = queryParams.TrimStart('?').Split('&');
            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0];
                    string value = keyValue[1];
                    parameters[key] = value;
                }
            }
        }

        return parameters;
    }

    public static void SendResponce(HttpListenerResponse response, int status, string contentType, object obj)
    {
        response.StatusCode = status;

        response.ContentType = contentType;
        response.ContentEncoding = Encoding.UTF8;
        string responseString = contentType == "application/json" ? JSON.ToJSON(obj) : (string)obj;
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        using Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
    }

    public static void SendResponce(HttpListenerResponse response, HttpStatusCode status, string contentType,
        object obj) =>
        SendResponce(response, (int)status, contentType, obj);

    public static void SendResponce(HttpListenerResponse response, int status) =>
        SendResponce(response, status, "text/plain", "");

    public static void SendResponce(HttpListenerResponse response, HttpStatusCode status) =>
        SendResponce(response, (int)status, "text/plain", "");

    public static void SendResponce(HttpListenerResponse response, int status, string message) =>
        SendResponce(response, status, "text/plain", message);

    public static void SendResponce(HttpListenerResponse response, HttpStatusCode status, string message) =>
        SendResponce(response, status, "text/plain", message);

    public static void AddRouterInfo(RouteInfoJSON info) => MainRoute.AddRouterInfo(info);
}

[Serializable]
public class RouteInfoJSON()
{
    public string route;
    public string protocol;
    public string httpMethod;
    public string description;
    public List<QueryParamInfo> queryParameters;
    public List<Permission> RequiredPermissions;

    public RouteInfoJSON(IController controller) : this()
    {
        protocol = "http";
        route = controller.Route;
        httpMethod = controller.HttpMethod;
        description = controller.Description;
        queryParameters = controller.QueryParameters;
        RequiredPermissions = controller.RequiredPermissions;
    }
}