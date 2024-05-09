namespace ValheimWebLink.Web.Controllers.PlayerData;

[Controller]
public class GetPlayerData : IController
{
    public string Route => "/playerdata/get";
    public string HttpMethod => "GET";
    public string Description => "Returns detail information about the player. Requires authentication. To get more data about the player, install PlayerData module.";
    public List<QueryParamInfo> QueryParameters => [new("name", "string", "Name of the player")];

    public async Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response, bool isAuthed,
        Dictionary<string, string> queryParameters)
    {
        if (!isAuthed)
        {
            WebApiManager.SendResponce(response, Unauthorized);
            return;
        }

        if (!queryParameters.TryGetValue("name", out var name))
        {
            WebApiManager.SendResponce(response, BadRequest, "Missing name parameter");
            return;
        }

        (HttpStatusCode code, string msg, PlayerData playerData) pD = await PlayerData.Create(name);

        if (pD.code == OK)
        {
            WebApiManager.SendResponce(response, pD.code, "application/json", pD.playerData);
            return;
        }

        WebApiManager.SendResponce(response, pD.code, pD.msg);
    }
}