using BepInEx;

namespace ValheimWebLink;

public static class AuthDataManager
{
    public static AuthData instance;

    public static void Init()
    {
        instance = new AuthData();
        try
        {
            if (!File.Exists(Path.Combine(Paths.GameRootPath, "auth.json")))
            {
                instance.root_password = randomPassword();
                instance.users.Add(new()
                {
                    username = "guest",
                    password = "guest",
                    permissions = [Permission.READ_generalinfo]
                });
                File.WriteAllText(Path.Combine(Paths.GameRootPath, "auth.json"), JSON.ToNiceJSON(instance));
                Debug($"Your auth file was created in {Paths.GameRootPath}", ConsoleColor.DarkGreen);
            }

            var fromFile = JSON.ToObject<AuthData>(File.ReadAllText(Path.Combine(Paths.GameRootPath, "auth.json")));
            if (fromFile == null) File.WriteAllText("auth.json", JSON.ToNiceJSON(instance));
            else instance = fromFile;
        }
        catch (Exception e)
        {
            File.WriteAllText(Path.Combine(Paths.GameRootPath, "auth.json"), JSON.ToNiceJSON(instance));
            Debug("Your auth file is corrupted.\n"
                  + $"Exeption: {e.GetType().Name} {e.Message}", ConsoleColor.Red);
        }

        SetupWatcher();
    }

    private static string randomPassword()
    {
        System.Random random = new();
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable
            .Repeat(chars, 10)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
    }

    private static void SetupWatcher()
    {
        if (!File.Exists(Path.Combine(Paths.GameRootPath, "auth.json")))
        {
            Debug("AuthData.SetupWatcher.0: AuthData file not found. This can not happen", ConsoleColor.Red);
            return;
        }

        FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Paths.GameRootPath, "auth.json");
        fileSystemWatcher.Changed += (_, _) =>
        {
            try
            {
                var fromFile = JSON.ToObject<AuthData>(File.ReadAllText("auth.json"));
                if (fromFile == null)
                {
                    Debug("Your auth file is corrupted or deleted", ConsoleColor.Red);
                    return;
                }

                if (fromFile.root_password == "root")
                {
                    Debug("\nYour login and password are default. You MUST edit it before running the server\n",
                        ConsoleColor.Red);
                }

                if (!fromFile.root_password.IsGood())
                {
                    Debug("\nYour rool password is in invalid format. Change it. Aborting...\n", ConsoleColor.Red);
                    return;
                }

                for (var i = 0; i < fromFile.users.Count; i++)
                {
                    var user = fromFile.users[i];
                    if (!user.username.IsGood())
                    {
                        Debug($"\nLogin of user number {i + 1} is in invalid format. Change it. Aborting...\n",
                            ConsoleColor.Red);
                        return;
                    }

                    if (!user.password.IsGood())
                    {
                        Debug($"\nPassword of user number {i + 1} is in invalid format. Change it. Aborting...\n",
                            ConsoleColor.Red);
                        return;
                    }
                    //TODO: check permissions for unknown ones. Use Enum.GetNames(typeof(Permission))
                }

                instance = fromFile;
            }
            catch (Exception exception)
            {
                var rewrite = new AuthData();
                File.WriteAllText("auth.json", JSON.ToNiceJSON(rewrite));
                Debug("Your auth.json file is corrupted and will be rewritten with default values.\n"
                      + $"Exeption: {exception.GetType().Name} {exception.Message}",
                    ConsoleColor.Red);

                instance = rewrite;
            }
        };
        fileSystemWatcher.IncludeSubdirectories = true;
        fileSystemWatcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        fileSystemWatcher.EnableRaisingEvents = true;
    }

    public static bool IsAuthed(string username, string password, List<Permission> requiredPermissions)
    {
        if (username == "root" && password == instance.root_password) return true;

        var user = GetUser(username);
        if (user == null) return false;
        if (user.password != password) return false;

        if (requiredPermissions == null) return true;
        foreach (var perm in user.permissions)
            if (!requiredPermissions.Contains(perm))
                return false;

        return true;
    }

    public static User GetUser(string username)
    {
        if (username == "root")
            return new()
            {
                username = "root",
                password = "root",
                // all permissions
                permissions = Enum.GetValues(typeof(Permission)).Cast<Permission>().ToList()
            };
        return instance.users.FirstOrDefault(x => x.username == username);
    }
}

[Serializable]
public class AuthData()
{
    public string root_password;
    public List<User> users = [];
}

[Serializable]
public class User
{
    public string username;
    public string password;
    public List<Permission> permissions = [];
}

public enum Permission
{
    READ_generalinfo,
    WRITE_generalinfo,
    READ_log,
    READ_adminlist,
    WRITE_adminlist,
    DELETE_adminlist,
    READ_banlist,
    WRITE_banlist,
    DELETE_banlist,
    READ_modlist,
    READ_players,
    READ_objects,
    WRITE_objects,
    CommandExecute,
}