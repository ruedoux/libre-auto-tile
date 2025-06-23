using Qwaitumin.LibreAutoTile.Configuration;
using Qwaitumin.LibreAutoTile.Configuration.Models;
using Qwaitumin.LibreAutoTile.Tiling;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests.Tiling;


[SimpleTestClass]
public class TilingSetTest
{
  private string jsonString = "";

  [SimpleBeforeAll]
  public void BeforeAll()
  {
    jsonString = File.ReadAllText("../resources/configurations/ExampleConfiguration.json");
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesSingleNotFullSetSquare_WhenCalled()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, autoTileConfiguration);
    TileDataVerifier tileDataVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Zero, new(-1, -1, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Left, new(-1, -1, -1, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Right, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, -1, -1, 0, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Right, new(-1, -1, -1, -1, -1, -1, -1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Right, new(-1, 0, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomRight, new(-1, 0, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(-1, 0, -1, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Right, new(-1, 0, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomLeft, new(-1, 0, -1, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Bottom, new(-1, 0, -1, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Right, new(-1, 0, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.BottomRight, new(-1, 0, -1, -1, -1, -1, -1, 0));
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesSingleNotFullSetEmptySquare_WhenCalled()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, autoTileConfiguration);
    TileDataVerifier tileDataVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Left, new(-1, -1, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopLeft, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, -1, -1, 0, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Top, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, -1, -1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Right, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomRight, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Right, new(-1, 0, -1, -1, -1, 0, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Bottom, new(-1, -1, -1, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Right, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomRight, new(-1, 0, -1, -1, -1, -1, -1, 0));

  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesMultipleNotFullSetSquare_WhenCalled()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, autoTileConfiguration);
    TileDataVerifier tileDataVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    for (int x = -10; x < 10; x++)
      for (int y = -10; y < 10; y++)
        autoTiler.PlaceTile(0, new(x, y), 1);

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Zero, new(-1, -1, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Left, new(-1, -1, -1, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, -1, -1, -1, -1, -1, -1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopLeft, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, 0, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, -1, -1, 0, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, 0, -1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Right, new(-1, 0, -1, -1, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.BottomRight, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Right, new(-1, 0, -1, -1, -1, 0, -1, 0));

    tileDataVerifier.PlaceTileAndVerify(0, Vector2.Bottom, new(-1, 0, -1, 0, -1, -1, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Zero, new(-1, 0, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Left, new(-1, 0, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tileDataVerifier.Verify(0, Vector2.BottomLeft, new(-1, 0, -1, 0, -1, -1, -1, -1));
    tileDataVerifier.Verify(0, Vector2.Top, new(-1, -1, -1, 0, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.Right, new(-1, 0, -1, -1, -1, 0, -1, 0));
    tileDataVerifier.Verify(0, Vector2.BottomRight, new(-1, 0, -1, -1, -1, -1, -1, 0));
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesMultipleFullSetEmptySquare_WhenCalled()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, autoTileConfiguration);
    TileDataVerifier tileDataVerifier = new(autoTiler, autoTileConfiguration);

    for (int x = -10; x < 10; x++)
      for (int y = -10; y < 10; y++)
        autoTiler.PlaceTile(0, new(x, y), 2);

    // When
    // Then
    tileDataVerifier.PlaceTileAndVerify(3, Vector2.Left, new(-1, -1, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(3, Vector2.TopLeft, new(-1, -1, -1, -1, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.Left, new(-1, 3, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(3, Vector2.BottomLeft, new(-1, 3, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(3, Vector2.Left, new(-1, 3, -1, -1, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.TopLeft, new(-1, -1, -1, -1, -1, 3, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(3, Vector2.Top, new(-1, -1, -1, -1, -1, -1, -1, 3));
    tileDataVerifier.Verify(3, Vector2.Left, new(-1, 3, -1, -1, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.TopLeft, new(-1, -1, -1, 3, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.BottomLeft, new(-1, 3, -1, -1, -1, -1, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(3, Vector2.TopRight, new(-1, -1, -1, -1, -1, -1, -1, 3));
    tileDataVerifier.Verify(3, Vector2.Left, new(-1, 3, -1, -1, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.TopLeft, new(-1, -1, -1, 3, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.BottomLeft, new(-1, 3, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(3, Vector2.Top, new(-1, -1, -1, 3, -1, -1, -1, 3));

    tileDataVerifier.PlaceTileAndVerify(3, Vector2.Right, new(-1, 3, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(3, Vector2.Left, new(-1, 3, -1, -1, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.TopLeft, new(-1, -1, -1, 3, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.BottomLeft, new(-1, 3, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(3, Vector2.Top, new(-1, -1, -1, 3, -1, -1, -1, 3));
    tileDataVerifier.Verify(3, Vector2.TopRight, new(-1, -1, -1, -1, -1, 3, -1, 3));

    tileDataVerifier.PlaceTileAndVerify(3, Vector2.BottomRight, new(-1, 3, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(3, Vector2.Left, new(-1, 3, -1, -1, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.TopLeft, new(-1, -1, -1, 3, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.BottomLeft, new(-1, 3, -1, -1, -1, -1, -1, -1));
    tileDataVerifier.Verify(3, Vector2.Top, new(-1, -1, -1, 3, -1, -1, -1, 3));
    tileDataVerifier.Verify(3, Vector2.TopRight, new(-1, -1, -1, -1, -1, 3, -1, 3));
    tileDataVerifier.Verify(3, Vector2.Right, new(-1, 3, -1, -1, -1, 3, -1, -1));

    tileDataVerifier.PlaceTileAndVerify(3, Vector2.Bottom, new(-1, -1, -1, 3, -1, -1, -1, 3));
    tileDataVerifier.Verify(3, Vector2.Left, new(-1, 3, -1, -1, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.TopLeft, new(-1, -1, -1, 3, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.BottomLeft, new(-1, 3, -1, 3, -1, -1, -1, -1));
    tileDataVerifier.Verify(3, Vector2.Top, new(-1, -1, -1, 3, -1, -1, -1, 3));
    tileDataVerifier.Verify(3, Vector2.TopRight, new(-1, -1, -1, -1, -1, 3, -1, 3));
    tileDataVerifier.Verify(3, Vector2.Right, new(-1, 3, -1, -1, -1, 3, -1, -1));
    tileDataVerifier.Verify(3, Vector2.BottomRight, new(-1, 3, -1, -1, -1, -1, -1, 3));
  }
}
