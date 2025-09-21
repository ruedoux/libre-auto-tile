using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;
using Qwaitumin.LibreAutoTile.Tiling.Search;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Tiling;

public class TilingStateVerifier(AutoTiler autoTiler, AutoTileConfiguration autoTileConfiguration)
{
  private readonly AutoTiler autoTiler = autoTiler;
  private readonly AutoTileConfiguration autoTileConfiguration = autoTileConfiguration;
  private readonly Dictionary<Vector2, (int TileId, TileMask TileMask)> expectedMasks = [];

  public void UpdateTile(int tileId, Vector2 position, TileMask tileMask)
  {
    expectedMasks[position] = new(tileId, tileMask);
  }

  public void AddTile(int tileId, Vector2 position, TileMask tileMask)
  {
    autoTiler.PlaceTile(0, position, tileId);
    expectedMasks[position] = new(tileId, tileMask);
  }

  public void Verify()
  {
    foreach (var (position, maskPacked) in expectedMasks)
    {
      TileData resultTileData = autoTiler.GetTile(0, position);
      var shouldBeMaskAndAtlas = GetAtlasAndMaskFromConfiguration(maskPacked.TileId, maskPacked.TileMask);
      Assertions.AssertEqual(shouldBeMaskAndAtlas.TileAtlas, resultTileData.TileAtlas,
        $"Mask at position {position} is {resultTileData.TileMask}, but should be: {shouldBeMaskAndAtlas.TileMask}");
    }
  }

  private (TileAtlas TileAtlas, TileMask TileMask) GetAtlasAndMaskFromConfiguration(
    int tileId, TileMask tileMask)
  {
    var tileDefinition = autoTileConfiguration.TileDefinitions[(uint)tileId];
    TileAtlas defaultTileAtlas = new(new(), "<None>");
    foreach (var (imageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
    {
      foreach (var (atlasPosition, tileMasks) in tileMaskDefinition.AtlasPositionToTileMasks)
      {
        foreach (var tileMaskArray in tileMasks)
        {
          var candidateTileMask = TileMask.FromArray([.. tileMaskArray]);
          defaultTileAtlas = new(atlasPosition.ToVector2(), imageFileName);
          if (candidateTileMask == tileMask)
            return (defaultTileAtlas, candidateTileMask);
        }
      }
    }

    return (defaultTileAtlas, new(-999));
  }
}

public class TileDataVerifier(AutoTiler autoTiler, AutoTileConfiguration autoTileConfiguration)
{
  private readonly AutoTiler autoTiler = autoTiler;
  private readonly AutoTileConfiguration autoTileConfiguration = autoTileConfiguration;

  public void PlaceTileAndVerify(int tileId, Vector2 position, TileMask tileMask)
  {
    autoTiler.PlaceTile(0, position, tileId);
    Verify(tileId, position, tileMask);
  }

  public void Verify(int tileId, Vector2 position, TileMask tileMask)
  {
    TileData resultTileData = autoTiler.GetTile(0, position);
    var shouldBeMaskAndAtlas = GetAtlasAndMaskFromConfiguration(tileId, tileMask);
    Assertions.AssertEqual(shouldBeMaskAndAtlas.TileAtlas, resultTileData.TileAtlas,
      $"Mask is {resultTileData.TileMask}, but should be: {shouldBeMaskAndAtlas.TileMask}");
  }

  private (TileAtlas TileAtlas, TileMask TileMask) GetAtlasAndMaskFromConfiguration(
    int tileId, TileMask tileMask)
  {
    var tileDefinition = autoTileConfiguration.TileDefinitions[(uint)tileId];
    TileAtlas defaultTileAtlas = new(new(), "<None>");
    foreach (var (imageFileName, tileMaskDefinition) in tileDefinition.ImageFileNameToTileMaskDefinition)
    {
      foreach (var (atlasPosition, tileMasks) in tileMaskDefinition.AtlasPositionToTileMasks)
      {
        foreach (var tileMaskArray in tileMasks)
        {
          var candidateTileMask = TileMask.FromArray([.. tileMaskArray]);
          defaultTileAtlas = new(atlasPosition.ToVector2(), imageFileName);
          if (candidateTileMask == tileMask)
            return (defaultTileAtlas, candidateTileMask);
        }
      }
    }

    return (defaultTileAtlas, new(-999));
  }
}