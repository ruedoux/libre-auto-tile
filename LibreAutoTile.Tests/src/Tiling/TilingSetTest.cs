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
  public void PlaceTile_CorrectlyPlacesNotFullSetFilledSquare_WhenMapEmpty()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, autoTileConfiguration);
    TilingStateVerifier tilingStateVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    VerifyNotFullSetFilledSquare(tilingStateVerifier);
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesNotFullSetEmptySquare_WhenMapEmpty()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, autoTileConfiguration);
    TilingStateVerifier tilingStateVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    VerifyNotFullSetEmptySquare(tilingStateVerifier);

  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesFullSetFilledSquare_WhenMapEmpty()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, autoTileConfiguration);
    TilingStateVerifier tilingStateVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    VerifyFullSetFilledSquare(tilingStateVerifier);
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesFullSetEmptySquare_WhenMapEmpty()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, autoTileConfiguration);
    TilingStateVerifier tilingStateVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    VerifyFullSetEmptySquare(tilingStateVerifier);
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesNotFullSetFilledSquare_WhenOtherTilesPresent()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, autoTileConfiguration);
    TilingStateVerifier tilingStateVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    for (int x = -10; x < 10; x++)
      for (int y = -10; y < 10; y++)
        autoTiler.PlaceTile(0, new(x, y), 1);

    VerifyNotFullSetFilledSquare(tilingStateVerifier);
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesNotFullSetEmptySquare_WhenOtherTilesPresent()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, autoTileConfiguration);
    TilingStateVerifier tilingStateVerifier = new(autoTiler, autoTileConfiguration);

    // When
    // Then
    for (int x = -10; x < 10; x++)
      for (int y = -10; y < 10; y++)
        autoTiler.PlaceTile(0, new(x, y), 1);

    VerifyNotFullSetEmptySquare(tilingStateVerifier);
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesFullSetFilledSquare_WhenOtherTilesPresent()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, autoTileConfiguration);
    TilingStateVerifier tilingStateVerifier = new(autoTiler, autoTileConfiguration);

    for (int x = -10; x < 10; x++)
      for (int y = -10; y < 10; y++)
        autoTiler.PlaceTile(0, new(x, y), 3);

    // When
    // Then
    VerifyFullSetFilledSquare(tilingStateVerifier);
  }

  [SimpleTestMethod]
  public void PlaceTile_CorrectlyPlacesFullSetEmptySquare_WhenOtherTilesPresent()
  {
    // Given
    var autoTileConfiguration = AutoTileConfiguration.FromJsonString(jsonString)
      ?? throw new ArgumentException();
    AutoTiler autoTiler = new(1, autoTileConfiguration);
    TilingStateVerifier tilingStateVerifier = new(autoTiler, autoTileConfiguration);

    for (int x = -10; x < 10; x++)
      for (int y = -10; y < 10; y++)
        autoTiler.PlaceTile(0, new(x, y), 3);

    // When
    // Then
    VerifyFullSetEmptySquare(tilingStateVerifier);
  }

  public static void VerifyNotFullSetFilledSquare(TilingStateVerifier tilingStateVerifier)
  {
    tilingStateVerifier.AddTile(0, Vector2.Zero, new(-1, -1, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Bottom, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Left, new(-1, -1, -1, 0, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(-1, 0, -1, -1, -1, 0, -1, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Right, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Zero, new(-1, 0, -1, 0, -1, 0, -1, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Top, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Left, new(-1, 0, -1, 0, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Top, new(-1, -1, -1, 0, -1, 0, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Right, new(-1, 0, -1, -1, -1, -1, -1, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.BottomRight, new(-1, 0, -1, -1, -1, -1, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Bottom, new(-1, 0, -1, 0, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Right, new(-1, 0, -1, -1, -1, 0, -1, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.BottomLeft, new(-1, 0, -1, 0, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Bottom, new(-1, 0, -1, 0, -1, -1, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Left, new(-1, 0, -1, 0, -1, 0, -1, -1));
    tilingStateVerifier.Verify();
  }

  public static void VerifyNotFullSetEmptySquare(TilingStateVerifier tilingStateVerifier)
  {
    tilingStateVerifier.AddTile(0, Vector2.Left, new(-1, -1, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.TopLeft, new(-1, -1, -1, -1, -1, 0, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Left, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.BottomLeft, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Left, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Top, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.TopLeft, new(-1, -1, -1, 0, -1, 0, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, -1, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.Top, new(-1, -1, -1, 0, -1, -1, -1, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Right, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.TopRight, new(-1, -1, -1, -1, -1, 0, -1, 0));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.BottomRight, new(-1, 0, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.Right, new(-1, 0, -1, -1, -1, 0, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(0, Vector2.Bottom, new(-1, -1, -1, 0, -1, -1, -1, 0));
    tilingStateVerifier.UpdateTile(0, Vector2.BottomLeft, new(-1, 0, -1, 0, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(0, Vector2.BottomRight, new(-1, 0, -1, -1, -1, -1, -1, 0));
    tilingStateVerifier.Verify();
  }

  public static void VerifyFullSetFilledSquare(TilingStateVerifier tilingStateVerifier)
  {
    tilingStateVerifier.AddTile(2, Vector2.Zero, new(-1, -1, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.Top, new(-1, -1, -1, -1, -1, 2, -1, -1));
    tilingStateVerifier.UpdateTile(2, Vector2.Zero, new(-1, 2, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.Bottom, new(-1, 2, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(2, Vector2.Zero, new(-1, 2, -1, -1, -1, 2, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.Left, new(-1, -1, -1, 2, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(2, Vector2.Zero, new(-1, 2, -1, -1, -1, 2, -1, 2));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.Right, new(-1, -1, -1, -1, -1, -1, -1, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Zero, new(-1, 2, -1, 2, -1, 2, -1, 2));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.TopLeft, new(-1, -1, -1, 2, 2, 2, -1, -1));
    tilingStateVerifier.UpdateTile(2, Vector2.Zero, new(2, 2, -1, 2, -1, 2, -1, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Top, new(-1, -1, -1, -1, -1, 2, 2, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Left, new(-1, 2, 2, 2, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.TopRight, new(-1, -1, -1, -1, -1, 2, 2, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Zero, new(2, 2, 2, 2, -1, 2, -1, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Top, new(-1, -1, -1, 2, 2, 2, 2, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Right, new(2, 2, -1, -1, -1, -1, -1, 2));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.BottomRight, new(2, 2, -1, -1, -1, -1, -1, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Zero, new(2, 2, 2, 2, 2, 2, -1, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Bottom, new(-1, 2, 2, 2, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(2, Vector2.Right, new(2, 2, -1, -1, -1, 2, 2, 2));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.BottomLeft, new(-1, 2, 2, 2, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(2, Vector2.Zero, new(2, 2, 2, 2, 2, 2, 2, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Bottom, new(2, 2, 2, 2, -1, -1, -1, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Left, new(-1, 2, 2, 2, 2, 2, -1, -1));
    tilingStateVerifier.Verify();
  }

  public static void VerifyFullSetEmptySquare(TilingStateVerifier tilingStateVerifier)
  {
    tilingStateVerifier.AddTile(2, Vector2.Top, new(-1, -1, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.Bottom, new(-1, -1, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.Left, new(-1, -1, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.Right, new(-1, -1, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.TopLeft, new(-1, -1, -1, 2, -1, 2, -1, -1));
    tilingStateVerifier.UpdateTile(2, Vector2.Top, new(-1, -1, -1, -1, -1, -1, -1, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Left, new(-1, 2, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.TopRight, new(-1, -1, -1, -1, -1, 2, -1, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Top, new(-1, -1, -1, 2, -1, -1, -1, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Right, new(-1, 2, -1, -1, -1, -1, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.BottomRight, new(-1, 2, -1, -1, -1, -1, -1, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Bottom, new(-1, -1, -1, 2, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(2, Vector2.Right, new(-1, 2, -1, -1, -1, 2, -1, -1));
    tilingStateVerifier.Verify();

    tilingStateVerifier.AddTile(2, Vector2.BottomLeft, new(-1, 2, -1, 2, -1, -1, -1, -1));
    tilingStateVerifier.UpdateTile(2, Vector2.Bottom, new(-1, -1, -1, 2, -1, -1, -1, 2));
    tilingStateVerifier.UpdateTile(2, Vector2.Left, new(-1, 2, -1, -1, -1, 2, -1, -1));
    tilingStateVerifier.Verify();
  }
}
