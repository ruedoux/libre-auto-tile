using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Qwaitumin.AutoTile.Configuration;
using Qwaitumin.AutoTile.GUI.Core.GodotBindings;
using Qwaitumin.AutoTile.GUI.Scenes.Editor.Tiles;
using Qwaitumin.AutoTile.GUI.Scenes.Editor.TileSet;


namespace Qwaitumin.AutoTile.GUI.Scenes.Editor.Utils;

public record EditorContext(
  int TileSize,
  HashSet<GuiTile> CreatedTiles,
  TileDatabase TileDatabase
);

public static class ConfigurationExtractor
{
  public static void LoadConfiguration(
    string filePath, EditorTiles editorTiles, BitmaskContainer bitmaskContainer)
  {
    if (!File.Exists(filePath))
      throw new FileNotFoundException($"File doesnt exist: '{filePath}'");

    var jsonString = File.ReadAllText(filePath);
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException("Loading json file results in null");

    if (autoTileConfiguration.TileDefinitions.Count == 0)
      throw new ArgumentException("To load you need at least one tile definition");

    editorTiles.ClearAll();
    bitmaskContainer.TileDatabase.Clear();

    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
      editorTiles.AddTile((int)tileId, tileDefinition.Name, GodotTypeMapper.Map(tileDefinition.Color));

    Dictionary<string, Dictionary<Configuration.Vector3, GuiTileData>> imageFileNameToMappedTileData = [];
    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
    {
      foreach (var (imageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
      {
        if (!imageFileNameToMappedTileData.TryGetValue(imageFileName, out var positionToTileData))
        {
          positionToTileData = [];
          imageFileNameToMappedTileData[imageFileName] = positionToTileData;
        }

        foreach (var (position, tileMasks) in tileMaskDefinition.AtlasPositionToTileMasks)
        {
          if (!positionToTileData.TryGetValue(position, out var guiTileData))
          {
            guiTileData = new();
            positionToTileData[position] = guiTileData;
          }

          guiTileData.SetCentreTileId(position.Z, (int)tileId);
          foreach (var tileMask in tileMasks)
            guiTileData.AddTileMask(position.Z, TileMask.FromArray([.. tileMask]));
        }
      }
    }

    foreach (var (imageFileName, mappedTileData) in imageFileNameToMappedTileData)
      foreach (var (position, guiTileData) in mappedTileData)
        bitmaskContainer.TileDatabase.SetPackedTileData(
          imageFileName, GodotTypeMapper.Map(position.ToVector2()), guiTileData);
  }

  public static AutoTileConfiguration GetAsAutoTileConfiguration(EditorContext editorContext)
  {
    Dictionary<uint, TileDefinition> tileDefinitions = [];
    foreach (var guiTile in editorContext.CreatedTiles)
    {
      Color color = guiTile.ColorPickerButton.Color;
      TileColor tileColor = new(
        r: (byte)color.R8, g: (byte)color.G8, b: (byte)color.B8, a: (byte)color.A8);

      var tileDefinition = TileDefinition.Construct(
        GetImageFileNameToTileMaskDefinition(editorContext, guiTile.TileId),
        name: guiTile.TileName,
        color: tileColor);
      tileDefinitions[(uint)guiTile.TileId] = tileDefinition;
    }

    return AutoTileConfiguration.Construct(editorContext.TileSize, tileDefinitions);
  }

  private static Dictionary<string, TileMaskDefinition> GetImageFileNameToTileMaskDefinition(
    EditorContext editorContext, int tileId)
  {
    Dictionary<string, TileMaskDefinition> imageFileNameToTileMaskDefinition = [];
    foreach (var (fileName, positionToTileData) in editorContext.TileDatabase.GetAll())
    {
      Dictionary<Configuration.Vector3, List<int[]>> positionsToTileMaskDefinitions = [];
      foreach (var (position, tileData) in positionToTileData)
      {
        foreach (var (layer, fullTileMask) in tileData.LayerFullTileMask)
        {
          var centreTileId = fullTileMask.CentreTileId;
          var tileMask = fullTileMask.TileMask;
          if (centreTileId != tileId) continue;

          var positionWithLayer = Configuration.Vector3.From(
            GodotTypeMapper.Map(position), layer);
          if (!positionsToTileMaskDefinitions.TryGetValue(positionWithLayer, out var tileMasks))
          {
            tileMasks = [];
            positionsToTileMaskDefinitions[positionWithLayer] = tileMasks;
          }
          tileMasks.Add(tileMask.ToArray());
        }
      }

      imageFileNameToTileMaskDefinition[Path.GetFileName(fileName)] = TileMaskDefinition.Construct(
        positionsToTileMaskDefinitions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
    }

    return imageFileNameToTileMaskDefinition;
  }
}