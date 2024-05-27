// #nullable enable
//
// namespace ValheimWebLink.Web.Controllers;
//
// [Controller]
// public class CheckLogin : IController
// {
//     public string Route => "/checklogin";
//     public string HttpMethod => "GET";
//     public string Description => "Used on web dashboard to check if user login is valid";
//     public List<QueryParamInfo> QueryParameters => [];
//     public List<Permission> RequiredPermissions => [];
//
//     public Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response,
//         Dictionary<string, string> queryParameters)
//     {
//         var result = Response.Create(request);
//         WebApiManager.SendResponce(response, 200, "application/json", result);
//         return Task.CompletedTask;
//     }
// }
//
// [Serializable]
// file struct Response()
// {
//     public bool success;
//     public string? error;
//     public Response(bool success) : this() => this.success = success;
//
//
//     public static Response Create(HttpListenerRequest request)
//     {
//         if (WebApiManager.IsAuthed(request)) return new Response(true);
//         return new Response(false);
//     }
// }