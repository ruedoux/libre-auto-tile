using Godot;
using Qwaitumin.LibreAutoTile.GUI.Signals;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.Preview;

public partial class TileMapTile : PanelContainer
{
  public readonly EventNotifier<TileMapTile> TileSelected = new();

  public Label NameLabel { private set; get; } = null!;
  public TextureRect TextureRectangle { private set; get; } = null!;
  public Button SelectButton { private set; get; } = null!;
  public int TileId;

  public override void _Ready()
  {
    NameLabel = GetNode<Label>("Panel/H/Fields/Name/Label");
    TextureRectangle = GetNode<TextureRect>("Panel/H/Fields/Texture/TextureRect");
    SelectButton = GetNode<Button>("Panel/H/Select");

    SelectButton.Pressed += SelectedTile;
  }

  private void SelectedTile()
    => TileSelected.NotifyObservers(this);
}
