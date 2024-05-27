using BepInEx;

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
            Debug("Settings.SetupWatcher.0: Settings file not found. This can not happen", ConsoleColor.Red);
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

                if (instance.httpPort != fromFile.httpPort)
                {
                    Debug($"Port changing requires full restart. Old: {instance.httpPort}, new: {fromFile.httpPort}",
                        ConsoleColor.Red);
                }

                if (instance.wsPort != fromFile.wsPort)
                {
                    Debug($"Port changing requires full restart. Old: {instance.wsPort}, new: {fromFile.wsPort}",
                        ConsoleColor.Red);
                }


                //TODO: reload port

                instance = fromFile;
            }
            catch (Exception exception)
            {
                var rewrite = new Settings();
                File.WriteAllText("settings.json", JSON.ToNiceJSON(rewrite));
                Debug("Your settings.json file is corrupted and will be rewritten with default values.\n"
                      + $"Exeption: {exception.GetType().Name} {exception.Message}",
                    ConsoleColor.Red);

                instance = rewrite;
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
    public int wsPort = 8081;
}