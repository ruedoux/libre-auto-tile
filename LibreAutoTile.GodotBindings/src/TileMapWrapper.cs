using System.Collections.Frozen;
using Godot;
using Qwaitumin.LibreAutoTile.Configuration;

namespace Qwaitumin.LibreAutoTile.GodotBindings;

internal class TileMapWrapper
{
  public readonly TileMapLayer TileMapLayer;
  public readonly FrozenDictionary<string, int> ImageFileToSourceId;

  public TileMapWrapper(
    AutoTileConfiguration autoTileConfiguration,
    TileSet.TileShapeEnum tileShape = TileSet.TileShapeEnum.Square)
  {
    int tileSize = (int)autoTileConfiguration.TileSize;
    Dictionary<string, HashSet<Vector2I>> imageFileNamesToAtlasPositions = GetImageFileNameToAtlasPositions(
      autoTileConfiguration);

    TileSet tileSet = new()
    {
      TileShape = tileShape,
      TileSize = GetTileSizeForShape(tileShape, tileSize)
    };
    if (tileShape == TileSet.TileShapeEnum.Isometric)
      tileSet.TileLayout = TileSet.TileLayoutEnum.DiamondDown;
    Dictionary<string, int> imageFileToSourceId = [];
    foreach (var (imageFileName, atlasPositions) in imageFileNamesToAtlasPositions)
    {
      var sourceId = AddSource(tileSet, imageFileName, tileSize);
      imageFileToSourceId[imageFileName] = sourceId;
      var source = (TileSetAtlasSource)tileSet.GetSource(sourceId);
      foreach (var atlasPosition in atlasPositions)
        source.CreateTile(atlasPosition);
    }

    TileMapLayer = new()
    {
      TileSet = tileSet,
      TextureFilter = CanvasItem.TextureFilterEnum.Nearest
    };

    ImageFileToSourceId = imageFileToSourceId.ToFrozenDictionary();
  }

  private static Vector2I GetTileSizeForShape(TileSet.TileShapeEnum tileShape, int atlasTileSize)
  {
    if (tileShape == TileSet.TileShapeEnum.Isometric)
      return new Vector2I(atlasTileSize, Math.Max(1, atlasTileSize / 2));
    return new Vector2I(atlasTileSize, atlasTileSize);
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

  private static Dictionary<string, HashSet<Vector2I>> GetImageFileNameToAtlasPositions(
    AutoTileConfiguration autoTileConfiguration)
  {
    Dictionary<string, HashSet<Vector2I>> imageFileNamesToAtlasPositions = [];
    foreach (var (_, tileDefinition) in autoTileConfiguration.TileDefinitions)
    {
      foreach (var (ImageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
      {
        foreach (var (atlasPosition, _) in tileMaskDefinition.AtlasPositionToTileMaskAndChance)
        {
          if (!imageFileNamesToAtlasPositions.TryGetValue(ImageFileName, out var atlasPositions))
          {
            atlasPositions = [];
            imageFileNamesToAtlasPositions[ImageFileName] = atlasPositions;
          }
          atlasPositions.Add(GodotTypeMapper.Map(atlasPosition.ToVector2()));
        }
      }
    }

    return imageFileNamesToAtlasPositions;
  }
}
