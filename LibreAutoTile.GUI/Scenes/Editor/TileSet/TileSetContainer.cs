using Godot;
using Qwaitumin.LibreAutoTile.GUI.GodotBindings;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet.Data;
using Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet.Display;
using Qwaitumin.LibreAutoTile.Tiling.Search.Models;

namespace Qwaitumin.LibreAutoTile.GUI.Scenes.Editor.TileSet;

public partial class TileSetContainer : Node2D
{
  public readonly TextureRect TileSetTexture;
  public readonly BitmaskDrawer BitmaskDrawer;
  public readonly BitmaskDatabase BitmaskDatabase = new();
  public readonly TileProbability TileProbability;
  public readonly TileDrawer TileDrawer;

  public TileSetContainer()
  {
    TileSetTexture = GodotApi.AddChild<TextureRect>(this, new());
    TileProbability = GodotApi.AddChild<TileProbability>(this, new());
    BitmaskDrawer = GodotApi.AddChild<BitmaskDrawer>(this, new());
    TileDrawer = GodotApi.AddChild<TileDrawer>(this, new());

    TileSetTexture.TextureFilter = TextureFilterEnum.Nearest;
    TileProbability.ZIndex = 1;
  }

  public void UpdateGrid(Rect2I size, Color color, int tileSize, int layer, string fileName)
  {
    TileDrawer.RedrawGrid(size, color, tileSize);
    TileProbability.Clear();

    int startX = size.Position.X;
    int startY = size.Position.Y;
    int endX = size.End.X;
    int endY = size.End.Y;
    for (int x = startX; x < endX; x += tileSize)
    {
      for (int y = startY; y < endY; y += tileSize)
      {
        var bitmaskData = BitmaskDatabase.GetBitmaskData(fileName, new(x, y));
        GD.Print(bitmaskData);
        uint probability = bitmaskData is null ? 1 : bitmaskData.GetProbability(layer);
        TileProbability.AddLabel(TileSetMath.ScaleDownTilePosition(new(x, y), tileSize), probability, tileSize);
      }
    }
  }


  public void Clear()
  {
    BitmaskDatabase.Clear();
    BitmaskDrawer.RedrawBitmask([]);
    BitmaskDrawer.ClearAllDrawn();
    TileProbability.Clear();
  }

  public void UpdateSelectedTileVisibility(bool visible)
  {
    if (visible)
    {
      TileDrawer.ShowSelectedTile();
      BitmaskDrawer.ShowBitmaskGhost();
    }
    else
    {
      TileDrawer.HideSelectedTile();
      BitmaskDrawer.HideBitmaskGhost();
    }
  }

  public void SetNewTexture(Texture2D texture)
  {
    TileSetTexture.Texture = texture;
  }

  public void Redraw(
    string filePath,
    int layer,
    Dictionary<int, string> tileIdToTileNames,
    Dictionary<string, Color> existingTileNamesToColors,
    int tileSize)
  {
    if (tileSize < 1) throw new ArgumentException("Tile size cannot be less than 1");
    BitmaskDrawer.RedrawBitmask([]);

    Dictionary<Rect2I, Color> bitmaskRectanglesToColors = [];
    foreach (var (scaledTilePosition, bitmaskData) in BitmaskDatabase.GetAllByFileName(filePath))
    {
      var snappedTilePosition = scaledTilePosition * tileSize;
      var centreTileId = bitmaskData.GetCentreTileId(layer);
      var tileMask = bitmaskData.GetTileMask(layer);
      var probability = bitmaskData.GetProbability(layer);

      if (centreTileId >= 0)
      {
        if (!tileIdToTileNames.TryGetValue(centreTileId, out var centreTileName))
          throw new ArgumentException($"Centre tile id '{centreTileId}' is not mapped to any name");
        if (!existingTileNamesToColors.TryGetValue(centreTileName, out var color))
          throw new ArgumentException($"Tile name '{centreTileName}' is not mapped to any color");

        Rect2I centreRectangle = TileSetMath.SnappedBitmaskPositionToWorldRectangle(
          snappedTilePosition, TileSetMath.MIDDLE, tileSize);
        bitmaskRectanglesToColors[centreRectangle] = color;
      }

      var tileMaskArray = tileMask.ToArray();
      for (int i = 0; i < tileMaskArray.Length; i++)
      {
        var tileId = tileMaskArray[i];
        if (tileId < 0)
          continue;

        if (!tileIdToTileNames.TryGetValue(tileId, out var tileName))
          throw new ArgumentException($"Tile mask tile id '{tileId}' is not mapped to any name");
        if (!existingTileNamesToColors.TryGetValue(tileName, out var color))
          throw new ArgumentException($"Tile name '{tileName}' is not mapped to any color");

        var bitmaskPosition = TileSetMath.DirectionToPosition(
          (TileMask.SurroundingDirection)i);
        Rect2I bitmaskRectangle = TileSetMath.SnappedBitmaskPositionToWorldRectangle(
          snappedTilePosition, bitmaskPosition, tileSize);
        bitmaskRectanglesToColors[bitmaskRectangle] = color;
      }
    }

    BitmaskDrawer.RedrawBitmask(bitmaskRectanglesToColors);
  }

  public void RedrawBitmaskGhost(Vector2 worldPosition, int tileSize, Color color)
    => BitmaskDrawer.RedrawBitmaskGhost(worldPosition, tileSize, color);

  public void AddBitmask(
    int layer, int tileId, string fileName, Vector2 worldPosition, int tileSize)
  {
    var scaledTilePosition = TileSetMath.ScaleDownTilePosition(worldPosition, tileSize);
    BitmaskDatabase.CreateBitmaskData(fileName, scaledTilePosition);
    var bitmaskData = BitmaskDatabase.GetBitmaskData(fileName, scaledTilePosition)
      ?? throw new NullReferenceException("BitmaskData is null");
    var bitmaskPosition = TileSetMath.DetermineBitmaskPosition(worldPosition, tileSize);
    bitmaskData.AddBitmask(layer, tileId, bitmaskPosition);
  }

  public void RemoveBitmask(int layer, string fileName, Vector2 worldPosition, int tileSize)
  {
    var scaledTilePosition = TileSetMath.ScaleDownTilePosition(worldPosition, tileSize);
    var bitmaskData = BitmaskDatabase.GetBitmaskData(fileName, scaledTilePosition);
    if (bitmaskData is null) return;

    var bitmaskPosition = TileSetMath.DetermineBitmaskPosition(worldPosition, tileSize);
    bitmaskData.RemoveBitmask(layer, bitmaskPosition);

    if (bitmaskData.IsEmpty())
    {
      BitmaskDatabase.GetAllByFileName(fileName).Remove(scaledTilePosition);
      TileProbability.ChangeLabelProbability(scaledTilePosition, 1);
    }
  }

  public void AddProbability(
  int layer, string fileName, Vector2I worldPosition, int value, int tileSize)
  {
    var scaledTilePosition = TileSetMath.ScaleDownTilePosition(worldPosition, tileSize);
    var bitmaskData = BitmaskDatabase.GetBitmaskData(fileName, scaledTilePosition);
    if (bitmaskData is null || bitmaskData.IsEmpty())
      return;

    long currentValue = bitmaskData.GetProbability(layer);
    uint newValue = (uint)Math.Clamp(currentValue + value, 0L, uint.MaxValue);

    bitmaskData.SetProbability(layer, newValue);
    TileProbability.ChangeLabelProbability(scaledTilePosition, newValue);
  }

  public void RemoveTileId(int tileId)
  {
    var db = BitmaskDatabase.GetAll();
    foreach (var (_, positionToPackedTileData) in db)
    {
      foreach (var (position, bitmaskData) in positionToPackedTileData)
      {
        foreach (var layer in bitmaskData.GetLayers())
          if (bitmaskData.GetCentreTileId(layer) == tileId)
            TileProbability.ChangeLabelProbability(position, 0);

        bitmaskData.RemoveTileId(tileId);
      }
    }
  }

  public void ChangeTileId(int newId, int oldId)
  {
    foreach (var (_, positionToPackedTileData) in BitmaskDatabase.GetAll())
      foreach (var (_, bitmaskData) in positionToPackedTileData)
        bitmaskData.ChangeTileId(newId, oldId);
  }
}
