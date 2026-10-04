using System.Collections.Frozen;
using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;

namespace Qwaitumin.LibreAutoTile.GodotBindings;

internal class TileMapWrapper
{
  public readonly TileMapLayer TileMapLayer;
  public readonly FrozenDictionary<string, int> ImageFileToSourceId;

  public TileMapWrapper(
    AutoTileConfiguration autoTileConfiguration,
    TileShape tileShape = TileShape.Square)
  {
    int tileSize = (int)autoTileConfiguration.TileSize;
    var imageFileNamesToAtlasPositions = AutoTileConfigurationExtractor.GetImageFileNameToAtlasPositions(
      autoTileConfiguration);

    TileSet.TileShapeEnum godotTileShape = GodotTypeMapper.Map(tileShape);
    TileSet tileSet = new()
    {
      TileShape = godotTileShape,
      TileSize = GodotTypeMapper.Map(tileShape.GetCellSize(tileSize))
    };
    if (godotTileShape == TileSet.TileShapeEnum.Isometric)
      tileSet.TileLayout = TileSet.TileLayoutEnum.DiamondDown;
    Dictionary<string, int> imageFileToSourceId = [];
    foreach (var (imageFileName, atlasPositions) in imageFileNamesToAtlasPositions)
    {
      var sourceId = AddSource(tileSet, imageFileName, tileSize);
      imageFileToSourceId[imageFileName] = sourceId;
      var source = (TileSetAtlasSource)tileSet.GetSource(sourceId);
      foreach (var atlasPosition in atlasPositions)
        source.CreateTile(GodotTypeMapper.Map(atlasPosition));
    }

    TileMapLayer = new()
    {
      TileSet = tileSet,
      TextureFilter = CanvasItem.TextureFilterEnum.Nearest
    };

    ImageFileToSourceId = imageFileToSourceId.ToFrozenDictionary();
  }

  private static int AddSource(TileSet tileSet, string sourceImagePath, int tileSize)
  {
    var texture = Image.LoadFromFile(sourceImagePath);

    TileSetAtlasSource source = new()
    {
      TextureRegionSize = new Vector2I(tileSize, tileSize),
      Texture = ImageTexture.CreateFromImage(texture)
    };

    return tileSet.AddSource(source);
  }
}
