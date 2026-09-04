using Godot;
using Qwaitumin.LibreAutoTile.GUI.Views.Presentation;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

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

  public TileViewModel Model { get; private set; }

  public event Action? RemoveRequested;
  public event Action<string>? NameSubmitted;
  public event Action<string>? IdSubmitted;
  public event Action<string>? ConnectionGroupSubmitted;
  public event Action? SelectRequested;
  public event Action? MoveUpRequested;
  public event Action? MoveDownRequested;
  public event Action<Color>? ColorChanged;

  public GuiTile(TileViewModel model)
  {
    Model = model;

    var panel = this.AppendMargin().ExpandHorizontal().WithMargins(Settings.MARGIN_MEDIUM);
    var hbox = panel.AppendHBox();

    RemoveButton = hbox.AppendButton("x").ExpandVertical();
    RemoveButton.Modulate = new Color(2, 0, 0, 1);

    var move = hbox.AppendVBox().ExpandHorizontal().StretchRatio(0.2f);
    MoveUpButton = move.AppendButton("↑").ExpandVertical();
    MoveDownButton = move.AppendButton("↓").ExpandVertical();

    SelectButton = hbox.AppendButton("o").ExpandVertical();

    var fields = hbox.AppendVBox().ExpandHorizontal();

    var colorRow = fields.AppendHBox().ExpandHorizontal().ExpandVertical();
    colorRow.AppendLabel("Color").ExpandHorizontal().ExpandVertical().FitContent();
    ColorPickerButton = colorRow.AppendColorPicker().ExpandHorizontal().ExpandVertical();

    var nameRow = fields.AppendHBox().ExpandHorizontal().ExpandVertical();
    nameRow.AppendLabel("Name").ExpandHorizontal().ExpandVertical();
    TileNameEdit = nameRow.AppendLineEdit().ExpandHorizontal().ExpandVertical();

    var idRow = fields.AppendHBox().ExpandHorizontal().ExpandVertical();
    idRow.AppendLabel("Id").ExpandHorizontal().ExpandVertical();
    TileIdEdit = idRow.AppendLineEdit().ExpandHorizontal().ExpandVertical();

    var groupRow = fields.AppendHBox().ExpandHorizontal().ExpandVertical();
    groupRow.AppendLabel("Group").ExpandHorizontal().ExpandVertical();
    ConnectionGroupEdit = groupRow.AppendLineEdit().ExpandHorizontal().ExpandVertical();

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

  public void Refresh(TileViewModel model)
  {
    Model = model;
    TileNameEdit.Text = model.TileName;
    TileIdEdit.Text = model.TileId.ToString();
    ConnectionGroupEdit.Text = model.ConnectionGroup?.ToString() ?? "null";
  }

  public void SetActive(bool active)
    => SelectButton.Modulate = active ? new Color(0, 2, 0) : Colors.White;
}
