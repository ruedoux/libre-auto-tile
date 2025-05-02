using Qwaitumin.SimpleTest;

namespace Qwaitumin.AutoTile.Tests;

public static class Program
{
  static void Main()
  {
    if (!new SimpleTestPrinter(Console.WriteLine).RunAll())
      Environment.Exit(1);
    //new SimpleTestPrinter(Console.WriteLine).RunForType(typeof(AutoTilerTest));
  }
}