using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.GodotBindings;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.GUI.Models;

public static class AutoTileConfigurationConverter
{
  public static AutoTileConfiguration GetAsAutoTileConfiguration(
    IEnumerable<TileModel> createdTiles, BitmaskDatabase tileDatabase, int tileSize, TileShape tileShape)
  {
    var allMasks = GroupMasksByTileId(tileDatabase);

    Dictionary<uint, TileDefinition> tileDefinitions = [];
    foreach (var tile in createdTiles)
    {
      Color color = tile.Color;
      TileColor tileColor = new(
        r: (byte)color.R8, g: (byte)color.G8, b: (byte)color.B8, a: (byte)color.A8);

      var tileDefinition = TileDefinition.Construct(
        BuildTileMaskDefinitions(allMasks, tile.TileId),
        name: tile.TileName,
        color: tileColor,
        connectionGroup: tile.ConnectionGroup);
      tileDefinitions[(uint)tile.TileId] = tileDefinition;
    }

    return AutoTileConfiguration.Construct(
      (uint)tileSize, tileDefinitions, tileShape: tileShape);
  }

  public static AutoTileConfiguration LoadConfiguration(
    string filePath, TileCollection tiles, BitmaskDatabase bitmaskDatabase)
  {
    if (!File.Exists(filePath))
      GodotLogger.LogErrorAndThrow($"File doesnt exist: '{filePath}'");

    var jsonString = File.ReadAllText(filePath);
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString);
    if (autoTileConfiguration is null)
      GodotLogger.LogErrorAndThrow("Loading json file results in null");

    if (autoTileConfiguration.TileDefinitions.Count == 0)
      GodotLogger.LogErrorAndThrow("To load you need at least one tile definition");

    tiles.Clear();
    bitmaskDatabase.Clear();

    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
      tiles.Add(
        tileId: (int)tileId,
        tileName: tileDefinition.Name,
        color: GodotTypeMapper.Map(tileDefinition.Color),
        connectionGroup: tileDefinition.ConnectionGroup);

    Dictionary<string, Dictionary<Configuration.Models.Vector3, BitmaskData>> imageFileNameToMappedBitmaskData = [];
    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
    {
      foreach (var (imageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
      {
        if (!imageFileNameToMappedBitmaskData.TryGetValue(imageFileName, out var positionToTileData))
        {
          positionToTileData = [];
          imageFileNameToMappedBitmaskData[imageFileName] = positionToTileData;
        }

        foreach (var (position, tileMasksAndChance) in tileMaskDefinition.AtlasPositionToTileMaskAndChance)
        {
          if (!positionToTileData.TryGetValue(position, out var bitmaskData))
          {
            bitmaskData = new();
            positionToTileData[position] = bitmaskData;
          }

          bitmaskData.SetCentreTileId(position.Z, (int)tileId);
          foreach (var (tileMask, chance) in tileMasksAndChance)
          {
            bitmaskData.AddTileMask(position.Z, TileMask.FromArray([.. tileMask]));
            bitmaskData.SetProbability(position.Z, chance);
          }
        }
      }
    }

    foreach (var (imageFileName, mappedTileData) in imageFileNameToMappedBitmaskData)
      foreach (var (position, guiTileData) in mappedTileData)
        bitmaskDatabase.SetPackedTileData(
          imageFileName, GodotTypeMapper.Map(position.ToVector2()), guiTileData);

    return autoTileConfiguration;
  }

  private static (
    List<string> ImageFileNames,
    Dictionary<int, Dictionary<string, Dictionary<Configuration.Models.Vector3, List<TileMaskData>>>> ByTileId)
    GroupMasksByTileId(BitmaskDatabase tileDatabase)
  {
    List<string> imageFileNames = [];
    Dictionary<int, Dictionary<string, Dictionary<Configuration.Models.Vector3, List<TileMaskData>>>> byTileId = [];

    foreach (var (fileName, positionToBitmaskData) in tileDatabase.GetAll())
    {
      string relativeFileName = Path.GetRelativePath(".", fileName);
      imageFileNames.Add(relativeFileName);

      foreach (var (position, bitmaskData) in positionToBitmaskData)
      {
        foreach (var (layer, fullTileMask) in bitmaskData.GetAll())
        {
          int centreTileId = fullTileMask.CentreTileId;
          if (centreTileId < 0)
            continue;

          var positionWithLayer = Configuration.Models.Vector3.From(GodotTypeMapper.Map(position), layer);

          if (!byTileId.TryGetValue(centreTileId, out var imageToPositions))
          {
            imageToPositions = [];
            byTileId[centreTileId] = imageToPositions;
          }

          if (!imageToPositions.TryGetValue(relativeFileName, out var positions))
          {
            positions = [];
            imageToPositions[relativeFileName] = positions;
          }

          if (!positions.TryGetValue(positionWithLayer, out var tileMasks))
          {
            tileMasks = [];
            positions[positionWithLayer] = tileMasks;
          }

          tileMasks.Add(TileMaskData.Construct(
            fullTileMask.TileMask.ToArray(), bitmaskData.GetProbability(layer)));
        }
      }
    }

    return (imageFileNames, byTileId);
  }

  private static Dictionary<string, TileMaskDefinition> BuildTileMaskDefinitions(
    (List<string> ImageFileNames,
      Dictionary<int, Dictionary<string, Dictionary<Configuration.Models.Vector3, List<TileMaskData>>>> ByTileId) allMasks,
    int tileId)
  {
    Dictionary<string, TileMaskDefinition> imageFileNameToTileMaskDefinition = [];
    allMasks.ByTileId.TryGetValue(tileId, out var imageToPositions);

    foreach (var imageFileName in allMasks.ImageFileNames)
    {
      Dictionary<Configuration.Models.Vector3, List<TileMaskData>> positions =
        imageToPositions is not null && imageToPositions.TryGetValue(imageFileName, out var p)
          ? p
          : [];

      imageFileNameToTileMaskDefinition[imageFileName] = TileMaskDefinition.Construct(
        positions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
    }

    return imageFileNameToTileMaskDefinition;
  }

  public static Dictionary<uint, (Configuration.Models.Vector3, string)> GetTileIdToImageLocation(
    AutoTileConfiguration autoTileConfiguration)
  {
    Dictionary<uint, (Configuration.Models.Vector3, string)> tileIdToImageLocation = [];
    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
    {
      var bestImageLocation = FindBestImageLocation(tileDefinition);
      if (bestImageLocation is not null)
        tileIdToImageLocation[tileId] = bestImageLocation.Value;
    }

    return tileIdToImageLocation;
  }

  private static (Configuration.Models.Vector3, string)? FindBestImageLocation(
    TileDefinition tileDefinition)
  {
    var defaultMask = TileMask.FromArray([-1, -1, -1, -1, -1, -1, -1, -1]);
    (Configuration.Models.Vector3, string)? latestMatch = null;
    foreach (var (imageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
    {
      foreach (var (atlasPosition, tileMaskDatas) in tileMaskDefinition.AtlasPositionToTileMaskAndChance)
      {
        if (tileMaskDatas.Any(tileMaskData => TileMask.FromArray([.. tileMaskData.Mask]) == defaultMask))
          return new(atlasPosition, imageFileName);
        latestMatch = new(atlasPosition, imageFileName);
      }
    }

    return latestMatch;
  }
}
