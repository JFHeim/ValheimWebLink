namespace ValheimWebLink.Web;

[Serializable]
public struct QueryParamInfo
{
    public string name;
    public string type;
    public string description;

    public QueryParamInfo(string name, string type, string description)
    {
        this.name = name;
        this.type = type;
        this.description = description;
    }
}