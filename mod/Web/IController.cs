namespace ValheimWebLink.Web.Controllers;

public interface IController
{
    public string Route { get; }
    public string HttpMethod { get; }
    public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response, bool isAuthed,
        Dictionary<string, string> queryParameters);
    public string Description { get; }
}