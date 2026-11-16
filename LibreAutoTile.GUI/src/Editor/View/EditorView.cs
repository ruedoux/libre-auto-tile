using Godot;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.View;

public partial class EditorView : Control
{
  public readonly Workspace Workspace;
  public readonly Toolbar Toolbar;
  public readonly ToolsPanel ToolsPanel;
  public readonly LayerControl LayerControl;
  public readonly MessageDisplay MessageDisplay;
  public readonly Control[] UiElements;

  public event Action<InputEventMouseMotion>? MouseMotion;
  public event Action<InputEventMouseButton>? MouseButton;
  public event Action<InputEventMouse>? MouseClick;
  public event Action<InputEventKey>? KeyButton;

  public EditorView(AppSettings appSettings, GuiLogger logger)
  {
    Workspace = this.AppendChild(new Workspace(appSettings));

    var splitContainer = this.AppendCanvasLayer().AppendSplitContainer()
      .FullRect().WithDefaultTheme(appSettings.UiSettings);

    ToolsPanel = splitContainer.AppendChild(new ToolsPanel(appSettings, logger))
      .ExpandFill().StretchRatio(appSettings.EditorSettings.PanelStretchRatio);

    var workspaceBox = splitContainer.AppendVBox().ExpandFill();

    Toolbar = workspaceBox.AppendChild(new Toolbar(appSettings));
    MessageDisplay = workspaceBox.AppendChild(new MessageDisplay()).ExpandHorizontal();

    workspaceBox.AppendVBox().ExpandFill();

    var bottomBar = workspaceBox.AppendMargin().ExpandHorizontal()
      .WithMargins(0, appSettings.UiSettings.MarginBig, appSettings.UiSettings.MarginMedium, appSettings.UiSettings.MarginMedium);
    var bottomBarHbox = bottomBar.AppendHBox().ExpandHorizontal();
    bottomBarHbox.Alignment = BoxContainer.AlignmentMode.End;
    LayerControl = bottomBarHbox.AppendChild(new LayerControl());

    UiElements =
    [
      ToolsPanel, Toolbar.SelectImageButton, Toolbar.SaveButton,
      Toolbar.LoadButton, Toolbar.ClearButton, LayerControl
    ];
  }

  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventMouse mouse)
      MouseClick?.Invoke(mouse);

    if (@event is InputEventMouseButton mouseButton)
      MouseButton?.Invoke(mouseButton);

    if (@event is InputEventMouseMotion mouseMotion)
      MouseMotion?.Invoke(mouseMotion);

    if (@event is InputEventKey keyButton)
      KeyButton?.Invoke(keyButton);
  }

  public void HideWorkspace()
  {
    Workspace.SetCanvasVisible(false);
    LayerControl.Hide();
  }

  public void ShowWorkspace()
  {
    Workspace.SetCanvasVisible(true);
    LayerControl.Show();
  }
}
