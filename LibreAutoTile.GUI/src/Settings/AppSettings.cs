using LibreAutoTile.GUI.Editor.Model;

namespace LibreAutoTile.GUI.Settings;

public class AppSettings
{
  public readonly UiSettings UiSettings = new();
  public readonly EditorSettings EditorSettings = new();
  public readonly CameraSettings CameraSettings = new();
  public readonly WorldSettings WorldSettings = new();
  public DisplayOptions DisplayOptions = new();

  public string LogFilePath = Path.Combine(AppContext.BaseDirectory, "guilog.txt");
}
