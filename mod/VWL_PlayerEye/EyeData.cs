namespace VWL_PlayerEye;

[Serializable]
public struct EyeData
{
    public string plName;
    public string message;
    public byte[] textureData;

    //TODO: implement this
    
    public override string ToString() => $"plName:{plName}, message:{message}";
}