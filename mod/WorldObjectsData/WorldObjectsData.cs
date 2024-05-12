using BepInEx;

namespace VWL_WorldObjectsData;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInDependency("com.Frogger.NoUselessWarnings", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.Frogger.ValheimWebLink", BepInDependency.DependencyFlags.SoftDependency)]
internal class WorldObjectsData : BaseUnityPlugin
{
    private const string ModName = "VWL_WorldObjectsData",
        ModAuthor = "Frogger",
        ModVersion = "0.1.0",
        ModGUID = $"com.{ModAuthor}.{ModName}";

    private void Awake()
    {
        // var coreInstalled = Chainloader.PluginInfos.ContainsKey("com.Frogger.ValheimWebLink");
        // Logger.LogWarning($"SteamManager.APP_ID = {SteamManager.APP_ID}");
        
        // if (SteamManager.APP_ID == 896660)
        // {
        //     if (!coreInstalled)
        //     {
        //         Logger.LogError(
        //             $"To uses any module for ValheimWebLink on server you need to have ValheimWebLink mod installed. "
        //             + $"Please install it. Clients should not have ValheimWebLink installed.\n\n"
        //             + $"Quitting...");
        //
        //         return; 
        //     }
        // } else if (coreInstalled)
        // {
        //     Logger.LogError("Clients should not have ValheimWebLink installed.\n"
        //                     + "Quitting...");
        //     return;
        // }

        CreateMod(this, ModName, ModAuthor, ModVersion, ModGUID);
    }
}