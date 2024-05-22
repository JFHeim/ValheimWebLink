using System.Net.Sockets;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ValheimWebLink.Web;

namespace VWL_PlayerEye;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInDependency("com.Frogger.NoUselessWarnings", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.Frogger.ValheimWebLink", BepInDependency.DependencyFlags.SoftDependency)]
internal class PlayerEyePlugin : BaseUnityPlugin
{
    private const string ModName = "VWL_PlayerEye",
        ModAuthor = "Frogger",
        ModVersion = "0.1.0",
        ModGUID = $"com.{ModAuthor}.{ModName}";

    public static string IP;

    public static ConfigEntry<int> updateInterval { get; private set; } = null!;

    internal static bool IsServer { get; private set; } = false;

    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion, ModGUID, false);
        JSON.Parameters = new()
        {
            UseExtensions = false,
            SerializeNullValues = false,
            DateTimeMilliseconds = false,
            UseUTCDateTime = true,
            UseOptimizedDatasetSchema = true,
            UseValuesOfEnums = true
        };

        var ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        if (ip == null)
        {
            DebugError($"Failed to get local ip");
            IP = "127.0.0.1";
        } else IP = ip.ToString();

        harmony.Patch(AccessTools.DeclaredMethod(typeof(ZNet), nameof(ZNet.SetServer)),
            postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(PlayerEyePlugin),
                nameof(PlayerEyePlugin.OnSetServer))));

        updateInterval = config("General", "Update interval", 100,
            new ConfigDescription("Basicly it is fps of video", new AcceptableValueRange<int>(20, 2000)));

        // List<string> windowNames = ScreenApi.GetOpenWindowNames();
        // foreach (string windowName in windowNames) DebugWarning($"Window name: {windowName}");

        var bytes = ScreenApi.GetScreenshot();
        Debug($"bytes = {bytes}");
    }

    private static void OnSetServer()
    {
        IsServer = ZNet.m_isServer;

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (IsServer && Attribute.IsDefined(type, typeof(ServerSidePatchAttribute)))
                harmony.PatchAll(type);

            if ((IsServer || (!IsServer && Application.platform == RuntimePlatform.WindowsPlayer))
                && Attribute.IsDefined(type, typeof(ClientSidePatchAttribute)))
                harmony.PatchAll(type);
        }

        DebugWarning($"Initializing {(IsServer ? "server" : "client")} api");
        if (IsServer) ServerLogic.Init();

        Debug($"Harmony patches applied: {harmony.GetPatchedMethods().Count()}");
    }
}