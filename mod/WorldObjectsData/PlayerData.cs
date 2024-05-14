namespace VWL_WorldObjectsData;

[Serializable]
internal struct PlayerData
{
    public SimpleVector3? position;
    public bool? publicPosition;
    public bool? inDebugFly;
    public float? eitr;
    public float? stamina;
    public bool? pvp;
    public bool? inBed;
    public float? health;
    public float? maxHealth;
    public float? noise;
    public InventoryData? inventory;
}