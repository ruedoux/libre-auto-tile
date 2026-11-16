using Godot;
using LibreAutoTile.GUI.Editor.View.Tools;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.View;

public partial class ToolsPanel : TabContainer
{
  public readonly SettingsPanel SettingsPanel;
  public readonly TilesPanel TilesPanel;
  public readonly ProbabilityPanel ProbabilityPanel;
  public readonly PreviewPanel PreviewPanel;

  public ToolsPanel(AppSettings settings, GuiLogger logger)
  {
    SettingsPanel = this.AddTab(new SettingsPanel(settings), "Settings");
    TilesPanel = this.AddTab(new TilesPanel(settings), "Tiles");
    ProbabilityPanel = this.AddTab(new ProbabilityPanel(), "Chance");
    PreviewPanel = this.AddTab(new PreviewPanel(settings, logger), "Preview");
  }
}
