using System;
using HarmonyLib;
using QModManager.API.ModLoading;
using QModManager.Utility;
using SMLHelper.V2.Handlers;

namespace SubnauticaCinematicMod
{
    [QModCore]
    public class Initializer
    {
        // private static CameraMenu _cameraMenu;
        public static PathManager PathManager;
        [QModPatch]
        public static void InitializerMethod()
        {
            // _cameraMenu = CameraMenu.Instance;
            ConsoleCommandsHandler.Main.RegisterConsoleCommands(typeof(Commands));
            Harmony harmony = new Harmony("CinematicMod");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    public static class PlayerAwakePatcher
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            Logger.Log(Logger.Level.Debug, "Postfix Awake");
            Initializer.PathManager = PathManager.Instance;
        }
    }
}