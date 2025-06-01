using System.Collections.Frozen;
using System.Diagnostics.Metrics;
using Godot;
using Qwaitumin.LibreAutoTile.Configuration;

namespace Qwaitumin.LibreAutoTile.GodotBindings;

internal class TileMapWrapper
{
  public readonly TileMapLayer TileMapLayer;
  public readonly FrozenDictionary<string, int> ImageFileToSourceId;

  public TileMapWrapper(AutoTileConfiguration autoTileConfiguration)
  {
    Vector2I tileSize = new(autoTileConfiguration.TileSize, autoTileConfiguration.TileSize);
    Dictionary<string, HashSet<Vector2I>> imageFilesToAtlasPositions = GetImageFileToAtlasPositions(
      autoTileConfiguration);

    TileSet tileSet = new();
    Dictionary<string, int> imageFileToSourceId = [];
    foreach (var (imagePath, atlasPositions) in imageFilesToAtlasPositions)
    {
      var sourceId = AddSource(tileSet, imagePath, tileSize);
      imageFileToSourceId[imagePath] = sourceId;
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

  private static Dictionary<string, HashSet<Vector2I>> GetImageFileToAtlasPositions(
    AutoTileConfiguration autoTileConfiguration)
  {
    Dictionary<string, HashSet<Vector2I>> imageFilesToAtlasPositions = [];
    foreach (var (_, tileDefinition) in autoTileConfiguration.TileDefinitions)
    {
      foreach (var (ImageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
      {
        foreach (var (atlasPosition, _) in tileMaskDefinition.AtlasPositionToTileMasks)
        {
          if (!imageFilesToAtlasPositions.TryGetValue(ImageFileName, out var atlasPositions))
          {
            atlasPositions = [];
            imageFilesToAtlasPositions[ImageFileName] = atlasPositions;
          }
          atlasPositions.Add(GodotTypeMapper.Map(atlasPosition.ToVector2()));
        }
      }
    }

    return imageFilesToAtlasPositions;
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
}