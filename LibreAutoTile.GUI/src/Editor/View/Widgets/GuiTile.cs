using Godot;
using LibreAutoTile.GUI.Editor.Model;
using LibreAutoTile.GUI.Settings;

namespace LibreAutoTile.GUI.Editor.View.Widgets;

public partial class GuiTile : PanelContainer
{
  public readonly Button MoveUpButton;
  public readonly Button MoveDownButton;
  public readonly Button RemoveButton;
  public readonly Button SelectButton;
  public readonly LineEdit TileNameEdit;
  public readonly LineEdit TileIdEdit;
  public readonly LineEdit ConnectionGroupEdit;
  public readonly ColorPickerButton ColorPickerButton;

  public TileModel Model { get; private set; }

  public event Action? RemoveRequested;
  public event Action<string>? NameSubmitted;
  public event Action<string>? IdSubmitted;
  public event Action<string>? ConnectionGroupSubmitted;
  public event Action? SelectRequested;
  public event Action? MoveUpRequested;
  public event Action? MoveDownRequested;
  public event Action<Color>? ColorChanged;

  public GuiTile(TileModel model, AppSettings settings)
  {
    Model = model;

    var panel = this.AppendMargin().ExpandHorizontal().WithMargins(settings.UiSettings.MarginMedium);
    var hbox = panel.AppendHBox();

    RemoveButton = hbox.AppendButton("x").ExpandVertical();
    RemoveButton.Modulate = new Color(2, 0, 0, 1);

    var move = hbox.AppendVBox().ExpandHorizontal().StretchRatio(0.2f);
    MoveUpButton = move.AppendButton("↑").ExpandVertical();
    MoveDownButton = move.AppendButton("↓").ExpandVertical();

    SelectButton = hbox.AppendButton("o").ExpandVertical();

    var fields = hbox.AppendVBox().ExpandHorizontal();

    var colorRow = fields.AppendHBox().ExpandFill();
    colorRow.AppendLabel("Color").ExpandFill().FitContent();
    ColorPickerButton = colorRow.AppendColorPicker(settings.UiSettings).ExpandFill();

    var nameRow = fields.AppendHBox().ExpandFill();
    nameRow.AppendLabel("Name").ExpandFill();
    TileNameEdit = nameRow.AppendLineEdit().ExpandFill();

    var idRow = fields.AppendHBox().ExpandFill();
    idRow.AppendLabel("Id").ExpandFill();
    TileIdEdit = idRow.AppendLineEdit().ExpandFill();

    var groupRow = fields.AppendHBox().ExpandFill();
    groupRow.AppendLabel("Group").ExpandFill();
    ConnectionGroupEdit = groupRow.AppendLineEdit().ExpandFill();

    RemoveButton.Pressed += () => RemoveRequested?.Invoke();
    SelectButton.Pressed += () => SelectRequested?.Invoke();
    MoveUpButton.Pressed += () => MoveUpRequested?.Invoke();
    MoveDownButton.Pressed += () => MoveDownRequested?.Invoke();
    TileNameEdit.TextSubmitted += name => NameSubmitted?.Invoke(name);
    TileNameEdit.FocusExited += () => NameSubmitted?.Invoke(TileNameEdit.Text);
    TileIdEdit.TextSubmitted += id => IdSubmitted?.Invoke(id);
    TileIdEdit.FocusExited += () => IdSubmitted?.Invoke(TileIdEdit.Text);
    ConnectionGroupEdit.TextSubmitted += group => ConnectionGroupSubmitted?.Invoke(group);
    ConnectionGroupEdit.FocusExited += () => ConnectionGroupSubmitted?.Invoke(ConnectionGroupEdit.Text);
    ColorPickerButton.ColorChanged += color => ColorChanged?.Invoke(color);

    ColorPickerButton.Color = Model.Color;
    Refresh(model);
  }

  public void Refresh(TileModel model)
  {
    Model = model;
    TileNameEdit.Text = model.TileName;
    TileIdEdit.Text = model.TileId.ToString();
    ConnectionGroupEdit.Text = model.ConnectionGroup?.ToString() ?? "null";
  }

  public void SetActive(bool active)
    => SelectButton.Modulate = active ? new Color(0, 2, 0) : Colors.White;
}
