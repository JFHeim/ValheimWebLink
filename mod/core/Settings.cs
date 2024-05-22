using BepInEx;
using UnityEngine.Serialization;
using ValheimWebLink.Web;

namespace ValheimWebLink;

public static class SettingsManager
{
    public static Settings instance;

    public static void Init()
    {
        instance = new Settings();
        try
        {
            if (!File.Exists(Path.Combine(Paths.GameRootPath, "settings.json")))
            {
                File.WriteAllText(Path.Combine(Paths.GameRootPath, "settings.json"), JSON.ToNiceJSON(instance));
                Debug($"Your settings file was created in {Paths.GameRootPath}", ConsoleColor.DarkGreen);
            }

            var fromFile = JSON.ToObject<Settings>(File.ReadAllText(Path.Combine(Paths.GameRootPath, "settings.json")));
            if (fromFile == null) File.WriteAllText("settings.json", JSON.ToNiceJSON(instance));
            else instance = fromFile;
        }
        catch (Exception e)
        {
            File.WriteAllText(Path.Combine(Paths.GameRootPath, "settings.json"), JSON.ToNiceJSON(instance));
            Debug("Your settings file is corrupted.\n"
                  + $"Exeption: {e.GetType().Name} {e.Message}", ConsoleColor.Red);
        }

        SetupWatcher();
    }

    private static void SetupWatcher()
    {
        if (!File.Exists(Path.Combine(Paths.GameRootPath, "settings.json")))
        {
            Debug("This can not happen");
            return;
        }

        FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Paths.GameRootPath, "settings.json");
        fileSystemWatcher.Changed += (_, _) =>
        {
            try
            {
                var fromFile = JSON.ToObject<Settings>(File.ReadAllText("settings.json"));
                if (fromFile == null)
                {
                    Debug("Your settings file is corrupted or deleted", ConsoleColor.Red);
                    return;
                }

                // if (instance.port != fromFile.port)
                // {
                //     Debug($"Port changing requires full restart. Old: {instance.port}, new: {fromFile.port}",
                //         ConsoleColor.Red);
                // }
                //TODO: reload port
                WebApiManager.ReloadPort();

                instance = fromFile;
            }
            catch (Exception exception)
            {
                Debug("Your settings file is corrupted.\n"
                      + $"Exeption: {exception.GetType().Name} {exception.Message}",
                    ConsoleColor.Red);
            }
        };
        fileSystemWatcher.IncludeSubdirectories = true;
        fileSystemWatcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        fileSystemWatcher.EnableRaisingEvents = true;
    }
}

[Serializable]
public class Settings
{
    public int httpPort = 8080;
    public int wsPort => httpPort + 1;
}