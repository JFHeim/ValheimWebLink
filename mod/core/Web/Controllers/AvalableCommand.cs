namespace ValheimWebLink.Web.Controllers;

[Controller]
public class AvalibleCommand : IController
{
    public string Route => "/avaliblecommands";
    public string HttpMethod => "GET";
    public string Description => "Returns all known ingame console commands";
    public List<QueryParamInfo> QueryParameters => [];
    public bool RequiresAuth => false;

    public Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response,
        Dictionary<string, string> queryParameters)
    {
        if (!ZNetScene.instance)
        {
            WebApiManager.SendResponce(response, ServiceUnavailable, "Game is not fully loaded");
            return Task.CompletedTask;
        }
        WebApiManager.SendResponce(response, OK, "application/json", Result.Create());
        return Task.CompletedTask;
    }
}

[Serializable]
file struct CommandInfo
{
    public string command;
    public string description;
    public List<string> tabOptions;
}

[Serializable]
file struct Result()
{
    public List<CommandInfo> commands = [];

    public static Result Create()
    {
        var ret = new Result();
        foreach (var cm in Terminal.commands.Values)
            ret.commands.Add(new()
            {
                command = cm.Command,
                description = cm.Description,
                tabOptions = cm.m_tabOptions
            });

        return ret;
    }
}