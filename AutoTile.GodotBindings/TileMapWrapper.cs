using System.Collections.Frozen;
using Godot;
using Qwaitumin.AutoTile.Configuration;

namespace Qwaitumin.AutoTile.GodotBindings;

internal class TileMapWrapper
{
  public readonly TileMapLayer TileMapLayer;
  public readonly FrozenDictionary<int, int> TileIdToSourceId;

  public TileMapWrapper(string imageDirectoryPath, AutoTileConfiguration autoTileConfiguration)
  {
    var pathToTileIds = AssignPathsToTileIds(imageDirectoryPath, autoTileConfiguration);
    Vector2I tileSize = new(autoTileConfiguration.TileSize, autoTileConfiguration.TileSize);
    Dictionary<int, HashSet<Vector2I>> tileIdtoAtlasPositions = GetTileIdsToAtlasPositions(
      autoTileConfiguration);

    TileSet tileSet = new();
    Dictionary<int, int> tileIdToSourceId = [];
    foreach (var kv in pathToTileIds)
    {
      var sourceId = AddSource(tileSet, kv.Key, tileSize);
      foreach (var tileId in kv.Value)
      {
        tileIdToSourceId[tileId] = sourceId;
        AssignTilesToSource(tileSet, sourceId, tileIdtoAtlasPositions[tileId]);
      }
    }

    TileMapLayer = new()
    {
      TileSet = tileSet,
      TextureFilter = CanvasItem.TextureFilterEnum.Nearest
    };
    TileIdToSourceId = tileIdToSourceId.ToFrozenDictionary();
  }

  private static Dictionary<int, HashSet<Vector2I>> GetTileIdsToAtlasPositions(
    AutoTileConfiguration autoTileConfiguration)
  {
    Dictionary<int, HashSet<Vector2I>> tileIdtoAtlasPositions = [];
    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
    {
      foreach (var (ImageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
      {
        foreach (var (atlasPosition, _) in tileMaskDefinition.AtlasPositionToTileMasks)
        {
          if (!tileIdtoAtlasPositions.TryGetValue((int)tileId, out var atlasPositions))
          {
            atlasPositions = [];
            tileIdtoAtlasPositions[(int)tileId] = atlasPositions;
          }
          atlasPositions.Add(GodotTypeMapper.Map(atlasPosition.ToVector2()));
        }
      }
    }

    return tileIdtoAtlasPositions;
  }

  private static Dictionary<string, HashSet<int>> AssignPathsToTileIds(
    string imageDirectoryPath, AutoTileConfiguration autoTileConfiguration)
  {
    Dictionary<string, HashSet<int>> pathToTileIds = [];

    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
    {
      foreach (var (ImageFileName, _) in tileDefinition.ImageFileNameToTileMaskDefinition)
      {
        var fullPath = Path.Combine(imageDirectoryPath, ImageFileName);
        if (!pathToTileIds.TryGetValue(fullPath, out var tileIds))
        {
          tileIds = [];
          pathToTileIds[fullPath] = tileIds;
        }

        tileIds.Add((int)tileId);
      }
    }

    return pathToTileIds;
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

  private static void AssignTilesToSource(
    TileSet tileSet, int sourceId, HashSet<Vector2I> atlasPositions)
  {
    var source = (TileSetAtlasSource)tileSet.GetSource(sourceId);
    foreach (var atlasPosition in atlasPositions)
      source.CreateTile(atlasPosition);
  }
}