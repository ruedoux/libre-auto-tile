using System.Numerics;
using Qwaitumin.AutoTile;
using Qwaitumin.SimpleTest;


namespace Qwaitumin.AutoTileTests;


[SimpleTestClass]
public class TileLoaderTest
{
  const string BITMASK_NAME = "bitmask";
  const string TILE1_NAME = "name1";
  const string TILE2_NAME = "name2";
  const string IMAGE1_NAME = "name1.jpg";
  const string IMAGE2_NAME = "name2.jpg";
  const int TILE1_LAYER = 0;
  const int TILE2_LAYER = 1;
  const int TILE1_ID = 0;
  const int TILE2_ID = 1;

  private readonly AutoTileConfig autoTileConfig = AutoTileConfig.Construct(
      16,
      new() { { TILE1_NAME, new(Layer: TILE1_LAYER, BitmaskName: BITMASK_NAME, ImageFileName: IMAGE1_NAME) }, { TILE2_NAME, new(Layer: TILE2_LAYER, BitmaskName: BITMASK_NAME, ImageFileName: IMAGE2_NAME) } },
      new() { { BITMASK_NAME, new() { { 0, Vector2.Zero } } } });

  [SimpleTestMethod]
  public void LoadTiles_ShouldCorrectlyLoadTiles_WhenCalled()
  {
    // Given
    using SimpleTestDirectory testDirectory = new();
    File.Create(testDirectory.GetRelativePath(IMAGE1_NAME));
    File.Create(testDirectory.GetRelativePath(IMAGE2_NAME));

    string[] tileNameToids = new string[] { TILE1_NAME, TILE2_NAME };
    TileLoader tileLoader = new(testDirectory.AbsolutePath, autoTileConfig, tileNameToids);

    // When
    var tiles = tileLoader.LoadTiles();

    // Then
    Assertions.AssertEqual(2, tiles.Count);
    Assertions.AssertTrue(tiles.ContainsKey(new(TILE1_ID, TILE1_NAME)));
    Assertions.AssertTrue(tiles.ContainsKey(new(TILE2_ID, TILE2_NAME)));
    Assertions.AssertEqual(testDirectory.AbsolutePath + "/" + TILE1_NAME + ".jpg", tiles[new(TILE1_ID, TILE1_NAME)].ImagePath);
    Assertions.AssertEqual(testDirectory.AbsolutePath + "/" + TILE2_NAME + ".jpg", tiles[new(TILE2_ID, TILE2_NAME)].ImagePath);
  }
}