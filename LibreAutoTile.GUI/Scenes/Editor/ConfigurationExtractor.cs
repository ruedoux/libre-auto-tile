using Godot;
using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.GodotBindings;
using Qwaitumin.LibreAutoTile.GUI.GodotBindings;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet.Data;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.UI.Tiles;
using Qwaitumin.LibreAutoTile.Tiling;


namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor;

public static class ConfigurationExtractor
{
  public static void LoadConfiguration(
    string filePath, EditorTiles editorTiles, TileSetContainer bitmaskContainer)
  {
    if (!File.Exists(filePath))
      GodotLogger.LogErrorAndThrow($"File doesnt exist: '{filePath}'");

    var jsonString = File.ReadAllText(filePath);
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString);
    if (autoTileConfiguration is null)
      GodotLogger.LogErrorAndThrow("Loading json file results in null");

    if (autoTileConfiguration.TileDefinitions.Count == 0)
      GodotLogger.LogErrorAndThrow("To load you need at least one tile definition");

    editorTiles.ClearAll();
    bitmaskContainer.BitmaskDatabase.Clear();

    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
      editorTiles.AddTile(
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
        bitmaskContainer.BitmaskDatabase.SetPackedTileData(
          imageFileName, GodotTypeMapper.Map(position.ToVector2()), guiTileData);
  }

  public static AutoTileConfiguration GetAsAutoTileConfiguration(
    HashSet<GuiTile> createdTiles, BitmaskDatabase tileDatabase, int tileSize)
  {
    Dictionary<uint, TileDefinition> tileDefinitions = [];
    foreach (var guiTile in createdTiles)
    {
      Color color = guiTile.ColorPickerButton.Color;
      TileColor tileColor = new(
        r: (byte)color.R8, g: (byte)color.G8, b: (byte)color.B8, a: (byte)color.A8);

      var tileDefinition = TileDefinition.Construct(
        GetImageFileNameToTileMaskDefinition(tileDatabase, guiTile.TileId),
        name: guiTile.TileName,
        color: tileColor,
        connectionGroup: guiTile.ConnectionGroup);
      tileDefinitions[(uint)guiTile.TileId] = tileDefinition;
    }

    return AutoTileConfiguration.Construct((uint)tileSize, tileDefinitions);
  }

  private static Dictionary<string, TileMaskDefinition> GetImageFileNameToTileMaskDefinition(
    BitmaskDatabase tileDatabase, int tileId)
  {
    Dictionary<string, TileMaskDefinition> imageFileNameToTileMaskDefinition = [];
    foreach (var (fileName, positionToBitmaskData) in tileDatabase.GetAll())
    {
      Dictionary<Configuration.Models.Vector3, List<TileMaskData>> positionsToTileMaskDefinitions = [];
      foreach (var (position, bitmaskData) in positionToBitmaskData)
      {
        foreach (var (layer, fullTileMask) in bitmaskData.GetAll())
        {
          var centreTileId = fullTileMask.CentreTileId;
          var tileMask = fullTileMask.TileMask;
          if (centreTileId != tileId) continue;

          var positionWithLayer = Configuration.Models.Vector3.From(
            GodotTypeMapper.Map(position), layer);
          if (!positionsToTileMaskDefinitions.TryGetValue(positionWithLayer, out var tileMasks))
          {
            tileMasks = [];
            positionsToTileMaskDefinitions[positionWithLayer] = tileMasks;
          }
          tileMasks.Add(TileMaskData.Construct(
            tileMask.ToArray(),
            bitmaskData.GetProbability(layer)));
        }
      }

      imageFileNameToTileMaskDefinition[Path.GetRelativePath(".", fileName)] = TileMaskDefinition.Construct(
        positionsToTileMaskDefinitions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
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
