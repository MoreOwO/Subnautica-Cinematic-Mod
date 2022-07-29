using SMLHelper.V2.Json;
using SMLHelper.V2.Options;
using SMLHelper.V2.Options.Attributes;
using UnityEngine;

namespace SubnauticaCinematicMod;

[Menu("Cinematic Mod")]
public class Config: ConfigFile
{
    [Slider("Path preview smoothness", Min = 0.05f, Max = 1f, Step = 0.05f, DefaultValue = 0.05f, Format = "{0:F2}"), OnChange(nameof(OnInterpolatedLineSmoothnessChanged))]
    public float PathPreviewInterpolatedLineSmoothness;

    [Keybind(Label = "Roll Left")] public KeyCode RollLeftKeyBind = KeyCode.LeftArrow;

    [Keybind(Label = "Roll Right")] public KeyCode RollRightKeyBind = KeyCode.RightArrow;

    [Keybind(Label = "Reset Roll")] public KeyCode RollResetKeyBind = KeyCode.DownArrow;

    [Keybind(Label = "Zoom")] public KeyCode FOVDecreaseKeyBind = KeyCode.KeypadPlus;

    [Keybind(Label = "DeZoom")] public KeyCode FOVIncreaseKeyBind = KeyCode.KeypadMinus;

    [Keybind(Label = "Reset Zoom")] public KeyCode FOVResetKeyBind = KeyCode.KeypadEnter;
    

    private void OnInterpolatedLineSmoothnessChanged(ToggleChangedEventArgs e)
    {
        if (PathManager.IsAlive)
        {
            PathManager.Instance.ShowPath();
        }
    }
}