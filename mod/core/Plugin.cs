using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using ValheimWebLink.ConsoleCommands;
using ValheimWebLink.Web;

namespace ValheimWebLink;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInDependency("com.Frogger.NoUselessWarnings", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    public static JSONParameters JSONParameters { get; private set; }

    private const string ModName = "ValheimWebLink",
        ModAuthor = "Frogger",
        ModVersion = "0.1.0",
        ModGUID = $"com.{ModAuthor}.{ModName}";

    public static List<string> MobulesInstalled = [];

    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion, ModGUID);

        JSONParameters = JSON.Parameters = new JSONParameters
        {
            UseExtensions = false,
            SerializeNullValues = false,
            DateTimeMilliseconds = false,
            UseUTCDateTime = true,
            UseOptimizedDatasetSchema = true,
            UseValuesOfEnums = true
        };

        StartCoroutine(WaiteForFullLoad());
    }

    private IEnumerator WaiteForFullLoad()
    {
        yield return new WaitUntil(() => Chainloader._loaded);

        foreach (var pluginInfos in Chainloader.PluginInfos.Values)
        {
            if (!pluginInfos.Metadata.Name.StartsWith("VWL_")) continue;
            MobulesInstalled.Add(pluginInfos.Metadata.Name.Replace("VWL_", ""));
        }

        if (MobulesInstalled.Count > 0)
            Debug($"Mobules installed: {MobulesInstalled.GetString()}");

        WebApiManager.Init(8080);
        ConsoleCommandsManager.Init();
    }

    public static void Debug(string message, ConsoleColor color = ConsoleColor.Cyan)
    {
        ConsoleManager.SetConsoleColor(color);
        ConsoleManager.StandardOutStream.WriteLine(message);
        ConsoleManager.SetConsoleColor(ConsoleColor.White);

        foreach (var listener in BepInEx.Logging.Logger.Listeners)
        {
            if (listener is not DiskLogListener diskLogListener) continue;
            diskLogListener.LogWriter.WriteLine(message);
        }
    }
}