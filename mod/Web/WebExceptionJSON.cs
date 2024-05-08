namespace ValheimWebLink.Web;

public struct WebExceptionJSON(Exception e)
{
    public string errorName = e.GetType().ToString();
    public string errorMessage = e.Message;
    public string stackTrace = e.StackTrace;
}