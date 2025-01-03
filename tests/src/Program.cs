using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Qwaitumin.AutoTile;
using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTileTests;


public class BitmaskDefinition
{
  public readonly ImmutableArray<int> IndexToTileIds;
  public readonly ImmutableArray<byte> TileIdIndexToBitmasks;

  public BitmaskDefinition(int[] indexToTileIds, byte[] tileIdIndexToBitmasks)
  {
    if (indexToTileIds.Length != 8)
      throw new ArgumentException($"Index to tile array lenght must be 8, but is: {indexToTileIds.Length}");
    if (tileIdIndexToBitmasks.Length != 8)
      throw new ArgumentException($"Index to bitmask array lenght must be 8, but is: {indexToTileIds.Length}");

    IndexToTileIds = indexToTileIds.ToImmutableArray();
    TileIdIndexToBitmasks = tileIdIndexToBitmasks.ToImmutableArray();
  }
}

public static class Program
{
  static void Main()
  {
    new SimpleTestPrinter(Console.WriteLine).Run();
  }
}