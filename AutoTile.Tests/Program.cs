using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTile.Tests;

public static class Program
{
  static void Main()
  {
    if (!new SimpleTestPrinter(Console.WriteLine).Run())
      Environment.Exit(1);
  }
}