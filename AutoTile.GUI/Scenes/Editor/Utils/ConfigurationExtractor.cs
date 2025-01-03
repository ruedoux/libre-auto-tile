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

    editorTiles.ClearAllExceptActiveTile();
    bitmaskContainer.TileDatabase.Clear();

    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
      editorTiles.AddTile((int)tileId, tileDefinition.Name, GodotTypeMapper.Map(tileDefinition.Color));

    foreach (var (tileId, tileDefinition) in autoTileConfiguration.TileDefinitions)
    {
      foreach (var (imageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
      {
        foreach (var (position, tileMask) in tileMaskDefinition.AtlasPositionToTileMasks)
        {
          //bitmaskContainer.TileDatabase.SetPackedTileData(
          //  imageFileName
          //);
        }
      }
    }
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
      Dictionary<Configuration.Vector2, List<int[]>> positionsToTileMaskDefinitions = [];
      foreach (var (position, tileData) in positionToTileData)
      {
        List<int[]> tileMasks = [];
        foreach (var (_, fullTileMask) in tileData.LayerFullTileMask)
        {
          var centreTileId = fullTileMask.CentreTileId;
          var tileMask = fullTileMask.TileMask;
          if (centreTileId == tileId)
            tileMasks.Add(tileMask.ToArray());
        }

        if (tileMasks.Count > 0)
          positionsToTileMaskDefinitions[GodotTypeMapper.Map(position)] = tileMasks;
      }

      imageFileNameToTileMaskDefinition[Path.GetFileName(fileName)] = TileMaskDefinition.Construct(
        positionsToTileMaskDefinitions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
    }

    return imageFileNameToTileMaskDefinition;
  }
}