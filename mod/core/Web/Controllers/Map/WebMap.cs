using System.ComponentModel;

namespace ValheimWebLink.Web.Controllers.Map;

[Description("Huge thanks to 'https://github.com/h0tw1r3/valheim-webmap/'")]
public static class WebMap
{
    public static MapDataServer mapDataServer;
    public static string worldDataPath;
    public static string mapDataPath = @"D:\Steam\steamapps\common\Valheim\_map";

    public static int sayMethodHash = 0;
    public static int chatMessageMethodHash = 0;

    public static bool fogTextureNeedsSaving;

    public static string currentWorldName;
    public static Dictionary<string, object> serverInfo;

    public static void NewWorld()
    {
        WebMapConfig.ReadConfigFile(GetPlugin().Config);
        string worldName = WebMapConfig.GetWorldName();
        bool forceReload = (currentWorldName != worldName);

        worldDataPath = Path.Combine(mapDataPath, WebMapConfig.GetWorldName());
        Directory.CreateDirectory(worldDataPath);

        if (mapDataServer == null)
        {
            Debug($"WebMap: loading existing world: #{worldName}");
            mapDataServer = new MapDataServer();
        } else if (forceReload)
        {
            Debug($"WebMap: loading a new world! old: #{currentWorldName} new: #{worldName}");
        }

        currentWorldName = worldName;

        string mapImagePath = Path.Combine(worldDataPath, "map.png");
        try
        {
            mapDataServer.mapImageData = File.ReadAllBytes(mapImagePath);
        }
        catch (Exception e)
        {
            DebugError("WebMap: Failed to read map image data from disk. " + e.Message);
        }

        string fogImagePath = Path.Combine(worldDataPath, "fog.png");
        try
        {
            Texture2D fogTexture = new Texture2D(WebMapConfig.TEXTURE_SIZE, WebMapConfig.TEXTURE_SIZE);
            byte[] fogBytes = File.ReadAllBytes(fogImagePath);
            fogTexture.LoadImage(fogBytes);
            mapDataServer.fogTexture = fogTexture;
        }
        catch (Exception e)
        {
            DebugWarning("WebMap: Failed to read fog image data from disk... Making new fog image..." + e.Message);
            Texture2D fogTexture = new Texture2D(WebMapConfig.TEXTURE_SIZE, WebMapConfig.TEXTURE_SIZE,
                TextureFormat.R8, false);
            Color32[] fogColors = new Color32[WebMapConfig.TEXTURE_SIZE * WebMapConfig.TEXTURE_SIZE];
            for (int t = 0; t < fogColors.Length; t++) fogColors[t] = Color.black;

            fogTexture.SetPixels32(fogColors);
            byte[] fogPngBytes = fogTexture.EncodeToPNG();

            mapDataServer.fogTexture = fogTexture;
            try
            {
                File.WriteAllBytes(fogImagePath, fogPngBytes);
            }
            catch (Exception ex)
            {
                DebugError("WebMap: FAILED TO WRITE FOG FILE! " + ex.Message);
            }
        }

        string mapPinsFile = Path.Combine(worldDataPath, "pins.csv");
        try
        {
            string[] pinsLines = File.ReadAllLines(mapPinsFile);
            mapDataServer.pins = new List<string>(pinsLines);
        }
        catch (Exception e)
        {
            DebugError("WebMap: Failed to read pins.csv from disk. " + e.Message);
        }

        if (forceReload)
        {
            mapDataServer.Reload();
        }
    }

    public static Color GetMaskColor(float wx, float wy, float height, Heightmap.Biome biome)
    {
        Color noForest = new Color(0f, 0f, 0f, 0f);
        Color forest = new Color(1f, 0f, 0f, 0f);

        if (height < ZoneSystem.instance.m_waterLevel) return noForest;

        if (biome == Heightmap.Biome.Meadows)
        {
            if (!WorldGenerator.InForest(new Vector3(wx, 0f, wy))) return noForest;

            return forest;
        }

        if (biome == Heightmap.Biome.Plains)
        {
            if (WorldGenerator.GetForestFactor(new Vector3(wx, 0f, wy)) >= 0.8f) return noForest;

            return forest;
        }

        if (biome == Heightmap.Biome.BlackForest || biome == Heightmap.Biome.Mistlands) return forest;

        return noForest;
    }

    public static Color GetPixelColor(Heightmap.Biome biome)
    {
        Color m_meadowsColor = new Color(0.573f, 0.655f, 0.361f);
        Color m_swampColor = new Color(0.639f, 0.447f, 0.345f);
        Color m_mountainColor = new Color(1f, 1f, 1f);
        Color m_blackforestColor = new Color(0.420f, 0.455f, 0.247f);
        Color m_heathColor = new Color(0.906f, 0.671f, 0.470f);
        Color m_ashlandsColor = new Color(0.690f, 0.192f, 0.192f);
        Color m_deepnorthColor = new Color(1f, 1f, 1f);
        Color m_mistlandsColor = new Color(0.36f, 0.22f, 0.4f);

        switch (biome)
        {
            case Heightmap.Biome.Meadows:
                return m_meadowsColor;
            case Heightmap.Biome.Swamp:
                return m_swampColor;
            case Heightmap.Biome.Mountain:
                return m_mountainColor;
            case Heightmap.Biome.BlackForest:
                return m_blackforestColor;
            case Heightmap.Biome.Plains:
                return m_heathColor;
            case Heightmap.Biome.AshLands:
                return m_ashlandsColor;
            case Heightmap.Biome.DeepNorth:
                return m_deepnorthColor;
            case Heightmap.Biome.Ocean:
                return Color.white;
            case Heightmap.Biome.Mistlands:
                return m_mistlandsColor;
            default:
                return Color.white;
        }
    }
}