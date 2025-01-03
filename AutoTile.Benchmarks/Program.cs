using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;

namespace Qwaitumin.Autotile.Benchmark;

public static class Program
{
  static void Main(string[] args)
  {
    var config = DefaultConfig.Instance
        .WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond));

    BenchmarkSwitcher
        .FromAssembly(Assembly.GetExecutingAssembly())
        .Run(args, config);
  }
}