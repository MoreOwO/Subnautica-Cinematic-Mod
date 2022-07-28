using SMLHelper.V2.Commands;
using UnityEngine;

namespace SubnauticaCinematicMod;

public static class Commands
{
    [ConsoleCommand("runpath")]
    public static string RunPath(int duration)
    {
        float? pathDuration = PathManager.Instance.RunPath(duration);
        return pathDuration == null ? "" : $"Running path for {pathDuration}sec";
    }

    [ConsoleCommand("addpoint")]
    public static string AddPoint()
    {
        Camera camera = Camera.main!;
        PathManager.Instance.AddPoint(camera);
        return "Added point";
    }

    [ConsoleCommand("clearpoints")]
    public static string ClearPoints()
    {
        PathManager.Instance.ClearPoints();
        return "Cleared points successfully";
    }

    [ConsoleCommand("stoppath")]
    public static string StopPath()
    {
        PathManager.Instance.StopPath();
        return "Stopping path";
    }
}