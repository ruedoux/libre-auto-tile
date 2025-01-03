using System;
using Godot;
using Qwaitumin.AutoTile.GUI.Core.Signals;
namespace Qwaitumin.AutoTile.GUI.Scenes.Editor.Tiles;

public partial class GuiTile : PanelContainer
{
  public Button MoveUpButton { private set; get; } = null!;
  public Button MoveDownButton { private set; get; } = null!;
  public Button RemoveButton { private set; get; } = null!;
  public Button SelectButton { private set; get; } = null!;
  public LineEdit TileNameEdit { private set; get; } = null!;
  public ColorPickerButton ColorPickerButton { private set; get; } = null!;

  public readonly ObserverNotifier<string> TryDeleteNotifier = new();
  public readonly ObserverNotifier<Tuple<GuiTile, string>> TryChangeTileNameNotifier = new();
  public readonly ObserverNotifier<GuiTile> SelectActiveTileNotifier = new();
  public string TileName = "<DEFAULT>";
  public int TileId = -1;

  public override void _Ready()
  {
    MoveUpButton = GetNode<Button>("Panel/H/Move/UpButton");
    MoveDownButton = GetNode<Button>("Panel/H/Move/DownButton");
    RemoveButton = GetNode<Button>("Panel/H/Remove");
    SelectButton = GetNode<Button>("Panel/H/Select");
    TileNameEdit = GetNode<LineEdit>("Panel/H/Fields/Name/LineEdit");
    ColorPickerButton = GetNode<ColorPickerButton>("Panel/H/Fields/Color/ColorPicker");

    MoveUpButton.Pressed += MoveUp;
    MoveDownButton.Pressed += MoveDown;
    RemoveButton.Pressed += Remove;
    SelectButton.Pressed += TrySelectActiveTile;
    TileNameEdit.TextSubmitted += TrySetNewTileName;
    TileNameEdit.FocusExited += TileNameEditExitedFocus;
  }

  private void TrySelectActiveTile()
    => SelectActiveTileNotifier.NotifyObservers(this);

  private void TileNameEditExitedFocus()
    => TrySetNewTileName(TileNameEdit.Text);

  private void TrySetNewTileName(string name)
    => TryChangeTileNameNotifier.NotifyObservers(new(this, name));

  private void Remove()
   => TryDeleteNotifier.NotifyObservers(TileName);

  private void MoveUp()
  {
    var nodeIndex = GetIndex();
    if (nodeIndex == 0)
      return;

    GetParent().MoveChild(this, nodeIndex - 1);
  }

  private void MoveDown()
  {
    var nodeIndex = GetIndex();
    var maxIndex = GetParent().GetChildCount();
    if (nodeIndex == maxIndex - 1)
      return;

    GetParent().MoveChild(this, nodeIndex + 1);
  }
}
