using Godot;

namespace Qwaitumin.LibreAutoTile.GUI.Models;

public class TileCollection
{
  private readonly List<TileModel> tiles = [];

  public IReadOnlyList<TileModel> Tiles => tiles;
  public TileModel? ActiveTile { get; private set; }

  public bool Contains(TileModel tile) => tiles.Contains(tile);
  public int IndexOf(TileModel tile) => tiles.IndexOf(tile);

  public TileModel Add(int tileId, string tileName, Color color, uint? connectionGroup)
  {
    if (tiles.Any(t => t.TileId == tileId))
      throw new ArgumentException($"Cannot create tile with already taken id: '{tileId}'");
    if (tiles.Any(t => t.TileName == tileName))
      throw new ArgumentException($"Cannot create tile with already taken name: '{tileName}'");

    var tile = new TileModel
    {
      TileId = tileId,
      TileName = tileName,
      Color = color,
      ConnectionGroup = connectionGroup,
    };
    tiles.Add(tile);
    return tile;
  }

  public TileModel AddNew()
  {
    Random random = new();
    return Add(
      tileId: GetNextFreeTileId(),
      tileName: GetNewTileName([.. tiles.Select(t => t.TileName)]),
      color: new Color(
        r: (float)random.NextDouble(),
        g: (float)random.NextDouble(),
        b: (float)random.NextDouble(),
        a: 0.7f),
      connectionGroup: null);
  }

  public void Remove(TileModel tile)
  {
    tiles.Remove(tile);
    if (ActiveTile == tile)
      ActiveTile = null;
  }

  public void Clear()
  {
    tiles.Clear();
    ActiveTile = null;
  }

  public void SetActive(TileModel tile)
    => ActiveTile = tile;

  public void TryChangeName(TileModel tile, string newTileName)
  {
    if (tiles.Any(t => t != tile && t.TileName == newTileName))
      newTileName = GetNewTileName([.. tiles.Select(t => t.TileName)], newTileName + "-copy");

    tile.TileName = newTileName;
  }

  public void TryChangeId(TileModel tile, string text)
  {
    int newId = int.TryParse(text, out var parsed) ? parsed : 0;
    if (newId < 0)
      newId = 0;

    if (tiles.Any(t => t != tile && t.TileId == newId))
      newId = GetNextFreeTileId();

    tile.TileId = newId;
  }

  public void TryChangeConnectionGroup(TileModel tile, string text)
  {
    int sanitized = int.TryParse(text, out var parsed) ? parsed : -1;
    tile.ConnectionGroup = sanitized < 0 ? null : (uint?)sanitized;
  }

  public void MoveUp(TileModel tile)
  {
    int index = tiles.IndexOf(tile);
    if (index <= 0)
      return;
    (tiles[index - 1], tiles[index]) = (tiles[index], tiles[index - 1]);
  }

  public void MoveDown(TileModel tile)
  {
    int index = tiles.IndexOf(tile);
    if (index < 0 || index >= tiles.Count - 1)
      return;
    (tiles[index + 1], tiles[index]) = (tiles[index], tiles[index + 1]);
  }

  private int GetNextFreeTileId()
  {
    var assignedIds = tiles.Select(t => t.TileId).ToHashSet();
    int nextFreeId = 0;
    while (assignedIds.Contains(nextFreeId))
      nextFreeId++;
    return nextFreeId;
  }

  private static string GetNewTileName(HashSet<string> names, string defaultName = "Tile")
  {
    string newName = defaultName;
    int index = 0;
    while (names.Contains(newName))
      newName = defaultName + index++.ToString();
    return newName;
  }
}
