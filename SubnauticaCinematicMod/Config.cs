using SMLHelper.V2.Json;
using SMLHelper.V2.Options;
using SMLHelper.V2.Options.Attributes;

namespace SubnauticaCinematicMod;

[Menu("Cinematic Mod")]
public class Config: ConfigFile
{
    [Slider("Path preview smoothness", Min = 0.05f, Max = 1f, Step = 0.05f, DefaultValue = 0.05f, Format = "{0:F2}"), OnChange(nameof(OnInterpolatedLineSmoothnessChanged))]
    public float PathPreviewInterpolatedLineSmoothness;

    private void OnInterpolatedLineSmoothnessChanged(ToggleChangedEventArgs e)
    {
        if (PathManager.IsAlive)
        {
            PathManager.Instance.ShowPath();
        }
    }
}