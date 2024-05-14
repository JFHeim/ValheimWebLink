namespace ValheimWebLink.Web.Controllers;

[Serializable]
public struct MessageResult(string message)
{
    public string message = message;
}