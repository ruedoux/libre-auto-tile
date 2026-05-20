using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
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
        .AddJob(Job.ShortRun)
        .HideColumns("Error", "StdDev", "RatioSD", "Alloc Ratio")
        .WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond));

    var summaries = BenchmarkSwitcher
        .FromAssembly(Assembly.GetExecutingAssembly())
        .Run(args, config);

    using var writer = new StreamWriter("BenchmarkResults.md");

    foreach (var summary in summaries)
    {
      writer.WriteLine($"## {summary.Title}");
      writer.WriteLine();

      var logger = new AccumulationLogger();
      MarkdownExporter.GitHub.ExportToLog(summary, logger);
      writer.WriteLine(logger.GetLog());

      writer.WriteLine();
    }
  }
}