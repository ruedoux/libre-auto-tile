using System.Collections.Frozen;
using Godot;
using Qwaitumin.LibreAutoTile.Configuration;

namespace Qwaitumin.LibreAutoTile.GodotBindings;

internal class TileMapWrapper
{
  public readonly TileMapLayer TileMapLayer;
  public readonly FrozenDictionary<string, int> ImageFileToSourceId;

  public TileMapWrapper(AutoTileConfiguration autoTileConfiguration)
  {
    Vector2I tileSize = new((int)autoTileConfiguration.TileSize, (int)autoTileConfiguration.TileSize);
    Dictionary<string, HashSet<Vector2I>> imageFileNamesToAtlasPositions = GetImageFileNameToAtlasPositions(
      autoTileConfiguration);

    TileSet tileSet = new();
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

  private static int AddSource(TileSet tileSet, string sourceImagePath, Vector2I tileSize)
  {
    var texture = Image.LoadFromFile(sourceImagePath);

    TileSetAtlasSource source = new()
    {
      TextureRegionSize = tileSize,
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
        foreach (var (atlasPosition, _) in tileMaskDefinition.AtlasPositionToTileMasks)
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