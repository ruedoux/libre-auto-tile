using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.GodotBindings;
using Qwaitumin.LibreAutoTile.GUI.Models;

namespace Qwaitumin.LibreAutoTile.GUI.Views;

public partial class PreviewPanel : MarginContainer
{
  public readonly VBoxContainer TileList;

  public AutoTileMap? AutoTileMap { get; private set; } = null;
  public PreviewTile? ActiveTile { get; private set; } = null;

  public PreviewPanel()
  {
    this.ExpandFill();
    TileList = this.AppendVBox().ExpandFill()
      .AppendScroll().ExpandFill()
      .AppendVBox().ExpandFill();
  }

  public void AddCreatedTiles(
    IEnumerable<(int TileId, string TileName)> createdTiles, AutoTileConfiguration autoTileConfiguration)
  {
    if (AutoTileMap is null)
      GodotLogger.LogErrorAndThrow("AutoTileMap is null");

    ActiveTile = null;
    foreach (var tile in TileList.GetChildren())
    {
      TileList.RemoveChild(tile);
      tile.QueueFree();
    }

    var tileIdToImageLocation = AutoTileConfigurationConverter.GetTileIdToImageLocation(
      autoTileConfiguration);
    foreach (var (tileId, tileName) in createdTiles)
    {
      Texture2D texture = ImageTexture.CreateFromImage(
        Image.CreateEmpty(1, 1, false, Image.Format.Rgba8));
      if (!tileIdToImageLocation.TryGetValue((uint)tileId, out var imageAtlasToImageName))
      {
        GodotLogger.LOGGER.LogWarning($"Could not get texture for tile id: {tileId}");
      }
      else
      {
        var atlasPosition = imageAtlasToImageName.Item1;
        var imageFileName = imageAtlasToImageName.Item2;
        var sourceId = AutoTileMap.GetSourceId(imageFileName);
        var source = AutoTileMap.GetTileMapLayer(0).TileSet.GetSource(sourceId);
        var tileSize = (int)autoTileConfiguration.TileSize;
        texture = TileTextureHelper.GetTileTexture(
          (TileSetAtlasSource)source,
          new(atlasPosition.X, atlasPosition.Y),
          new(tileSize, tileSize));
      }

      var previewTile = new PreviewTile();
      TileList.AddChild(previewTile);

      previewTile.TileSelected += ChangeActiveTile;
      previewTile.NameLabel.Text = tileName;
      previewTile.TileId = tileId;
      previewTile.TextureRectangle.Texture = texture;
    }

    if (TileList.GetChildren().FirstOrDefault() is PreviewTile firstTile)
      ChangeActiveTile(firstTile);
  }

  public void InitializeTileMap(AutoTileConfiguration autoTileConfiguration, TileShape tileShape)
  {
    AutoTileMap?.QueueFree();
    AutoTileMap = new(1, autoTileConfiguration, MapTileShape(tileShape));
  }

  private static TileSet.TileShapeEnum MapTileShape(TileShape tileShape)
    => tileShape switch
    {
      TileShape.Isometric => TileSet.TileShapeEnum.Isometric,
      _ => TileSet.TileShapeEnum.Square,
    };

  private void ChangeActiveTile(PreviewTile previewTile)
  {
    if (ActiveTile is not null)
      ActiveTile.SelectButton.Modulate = Colors.White;
    previewTile.SelectButton.Modulate = new(r: 0, g: 2, b: 0);
    ActiveTile = previewTile;
    GodotLogger.LOGGER.Log($"Changed active tile: {previewTile.NameLabel.Text}");
  }
}
