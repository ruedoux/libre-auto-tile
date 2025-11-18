using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;

namespace Qwaitumin.LibreAutoTile.Benchmark;

public static class Program
{
  public static void Main(string[] args)
  {
    var config = DefaultConfig.Instance
        .WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond));

    var summaries = BenchmarkSwitcher
        .FromAssembly(Assembly.GetExecutingAssembly())
        .Run(args, config);

    using var writer = new StreamWriter("BenchmarkResults.md");
    foreach (var summary in summaries)
    {
      writer.WriteLine($"## {summary.Title}\n");
      var accumLogger = new AccumulationLogger();
      MarkdownExporter.Default.ExportToLog(summary, accumLogger);
      writer.WriteLine(accumLogger.GetLog());
      writer.WriteLine();
    }
  }
}