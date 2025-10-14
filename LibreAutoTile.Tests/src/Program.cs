using Qwaitumin.SimpleTest;

namespace Qwaitumin.LibreAutoTile.Tests;

public static class Program
{
  public static void Main(string[] args)
  {
    if (!new SimpleTestPrinter(Console.WriteLine).Run(args))
      Environment.Exit(1);
  }
}