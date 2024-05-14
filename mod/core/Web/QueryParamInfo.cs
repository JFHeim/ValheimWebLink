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

// #nullable enable
// namespace ValheimWebLink.Web;
//
// [Serializable]
// public struct QueryParamInfo<T>
// {
//     public string name;
//     public Type type;
//     public string description;
//
//     public QueryParamInfo(string name, string description)
//     {
//         this.name = name;
//         this.type = typeof(T);
//         this.description = description;
//     }
// }
// [Serializable]
// public class QueryParam<T>(QueryParamInfo<T> info, T? value)
// {
//     public readonly QueryParamInfo<T> info = info;
//     public readonly T? Value = value;
// }