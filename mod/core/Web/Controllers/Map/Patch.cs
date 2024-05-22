using HarmonyLib;
using static ValheimWebLink.Web.Controllers.Map.WebMap;

namespace ValheimWebLink.Web.Controllers.Map;

[UsedImplicitly, HarmonyPatch, HarmonyWrapSafe]
file static class Patch
{
    private static readonly Color DeepWaterColor = new Color(0.36105883f, 0.36105883f, 0.43137255f);
    private static readonly Color ShallowWaterColor = new Color(0.574f, 0.50709206f, 0.47892025f);
    private static readonly Color ShoreColor = new Color(0.1981132f, 0.12241901f, 0.1503943f);

    [HarmonyPatch(typeof(ZoneSystem), nameof(ZoneSystem.Start))]
    [HarmonyPostfix]
    public static void ZoneSystemStart()
    {
        NewWorld();

        if (mapDataServer.mapImageData != null)
        {
            Debug("WebMap: MAP ALREADY BUILT!");
            return;
        }

        Debug("WebMap: BUILD MAP!");

        int num = WebMapConfig.TEXTURE_SIZE / 2;
        float num2 = WebMapConfig.PIXEL_SIZE / 2f;
        Color mask;
        Color32[] colorArray = new Color32[WebMapConfig.TEXTURE_SIZE * WebMapConfig.TEXTURE_SIZE];
        Color32[] treeMaskArray = new Color32[WebMapConfig.TEXTURE_SIZE * WebMapConfig.TEXTURE_SIZE];
        float[] heightArray = new float[WebMapConfig.TEXTURE_SIZE * WebMapConfig.TEXTURE_SIZE];
        for (int i = 0; i < WebMapConfig.TEXTURE_SIZE; i++)
        {
            for (int j = 0; j < WebMapConfig.TEXTURE_SIZE; j++)
            {
                float wx = (float)(j - num) * WebMapConfig.PIXEL_SIZE + num2;
                float wy = (float)(i - num) * WebMapConfig.PIXEL_SIZE + num2;
                Heightmap.Biome biome = WorldGenerator.instance.GetBiome(wx, wy);
                float biomeHeight = WorldGenerator.instance.GetBiomeHeight(biome, wx, wy, out mask);
                colorArray[i * WebMapConfig.TEXTURE_SIZE + j] = GetPixelColor(biome);
                treeMaskArray[i * WebMapConfig.TEXTURE_SIZE + j] = GetMaskColor(wx, wy, biomeHeight, biome);
                heightArray[i * WebMapConfig.TEXTURE_SIZE + j] = biomeHeight;
            }
        }

        float waterLevel = ZoneSystem.instance.m_waterLevel;
        Vector3 sunDir = new Vector3(-0.57735f, 0.57735f, 0.57735f);
        Color[] newColors = new Color[colorArray.Length];

        for (int t = 0; t < colorArray.Length; t++)
        {
            float h = heightArray[t];

            int tUp = t - WebMapConfig.TEXTURE_SIZE;
            if (tUp < 0) tUp = t;

            int tDown = t + WebMapConfig.TEXTURE_SIZE;
            if (tDown > colorArray.Length - 1) tDown = t;

            int tRight = t + 1;
            if (tRight > colorArray.Length - 1) tRight = t;

            int tLeft = t - 1;
            if (tLeft < 0) tLeft = t;

            float hUp = heightArray[tUp];
            float hRight = heightArray[tRight];
            float hLeft = heightArray[tLeft];
            float hDown = heightArray[tDown];

            Vector3 va = new Vector3(2f, 0f, hRight - hLeft).normalized;
            Vector3 vb = new Vector3(0f, 2f, hUp - hDown).normalized;
            Vector3 normal = Vector3.Cross(va, vb);

            float surfaceLight = Vector3.Dot(normal, sunDir) * 0.25f + 0.75f;

            float shoreMask = Mathf.Clamp(h - waterLevel, 0, 1);
            float shallowRamp = Mathf.Clamp((h - waterLevel + 0.2f * 12.5f) * 0.5f, 0, 1);
            float deepRamp = Mathf.Clamp((h - waterLevel + 1f * 12.5f) * 0.1f, 0, 1);

            Color32 mapColor = colorArray[t];
            Color ans = Color.Lerp(ShoreColor, mapColor, shoreMask);
            ans = Color.Lerp(ShallowWaterColor, ans, shallowRamp);
            ans = Color.Lerp(DeepWaterColor, ans, deepRamp);

            newColors[t] = new Color(ans.r * surfaceLight, ans.g * surfaceLight, ans.b * surfaceLight, ans.a);
        }

        Texture2D newTexture = new Texture2D(WebMapConfig.TEXTURE_SIZE, WebMapConfig.TEXTURE_SIZE,
            TextureFormat.RGBA32, false);
        newTexture.SetPixels(newColors);
        byte[] pngBytes = newTexture.EncodeToPNG();

        mapDataServer.mapImageData = pngBytes;
        try
        {
            File.WriteAllBytes(Path.Combine(worldDataPath, "map.png"), pngBytes);
            Debug("WebMap: BUILDING MAP DONE!");
        }
        catch (Exception e)
        {
            DebugError("WebMap: FAILED TO WRITE MAP FILE! " + e.Message);
        }
    }
}