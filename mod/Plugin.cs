using BepInEx;
using BepInEx.Logging;
using ValheimWebLink.ConsoleCommands;
using ValheimWebLink.Web;

namespace ValheimWebLink;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInDependency("com.Frogger.NoUselessWarnings", BepInDependency.DependencyFlags.SoftDependency)]
internal class Plugin : BaseUnityPlugin
{
    private const string ModName = "ValheimWebLink",
        ModAuthor = "Frogger",
        ModVersion = "0.1.0",
        ModGUID = $"com.{ModAuthor}.{ModName}";

    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion, ModGUID);

        JSON.Parameters = new JSONParameters
        {
            UseExtensions = false,
            SerializeNullValues = false,
            DateTimeMilliseconds = false,
            UseUTCDateTime = true,
            UseOptimizedDatasetSchema = true,
            UseValuesOfEnums = true
        };

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