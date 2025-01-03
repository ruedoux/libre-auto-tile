using Godot;
using Qwaitumin.LibreAutoTile.GUI.Core.Signals;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Preview;

public partial class TileMapTile : PanelContainer
{
  public readonly EventNotifier<TileMapTile> TileSelected = new();

  public Label NameLabel { private set; get; } = null!;
  public ColorRect ColorRectangle { private set; get; } = null!;
  public Button SelectButton { private set; get; } = null!;
  public int TileId;

  public override void _Ready()
  {
    NameLabel = GetNode<Label>("Panel/H/Fields/Name/Label");
    ColorRectangle = GetNode<ColorRect>("Panel/H/Fields/Texture/ColorRect");
    SelectButton = GetNode<Button>("Panel/H/Select");

    SelectButton.Pressed += SelectedTile;
  }

  private void SelectedTile()
    => TileSelected.NotifyObservers(this);
}
