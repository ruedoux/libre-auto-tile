## LibreAutoTile.Benchmark.TileMaskSearcherBenchmark-20260910-215928

```

BenchmarkDotNet v0.15.8, Linux Debian GNU/Linux 12 (bookworm)
AMD Ryzen 7 7800X3D 3.21GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.302
  [Host]   : .NET 9.0.18 (9.0.18, 9.0.1826.31522), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.18 (9.0.18, 9.0.1826.31522), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=3  
LaunchCount=1  UnrollFactor=1  WarmupCount=3  

```
| Method                               | TileMaskCount | LookupCount | Mean      | Allocated |
|------------------------------------- |-------------- |------------ |----------:|----------:|
| **FindBestMatch_ExactMaskFastPath**      | **1000**          | **1000**        | **0.0812 ms** |         **-** |
| FindBestMatch_SearchMiss             | 1000          | 1000        | 0.5357 ms |    8136 B |
| FindBestMatch_RepeatedMiss_WithCache | 1000          | 1000        | 0.1109 ms |    8136 B |
| FindBestMatch_RepeatedMiss_NoCache   | 1000          | 1000        | 0.5356 ms |    8136 B |
| FindBestMatch_UniqueMisses_WithCache | 1000          | 1000        | 0.0798 ms |         - |
| FindBestMatch_UniqueMisses_NoCache   | 1000          | 1000        | 0.0791 ms |         - |
| **FindBestMatch_ExactMaskFastPath**      | **10000**         | **1000**        | **0.0837 ms** |         **-** |
| FindBestMatch_SearchMiss             | 10000         | 1000        | 1.3440 ms |   80136 B |
| FindBestMatch_RepeatedMiss_WithCache | 10000         | 1000        | 0.1248 ms |   80136 B |
| FindBestMatch_RepeatedMiss_NoCache   | 10000         | 1000        | 1.6399 ms |   80136 B |
| FindBestMatch_UniqueMisses_WithCache | 10000         | 1000        | 0.1195 ms |         - |
| FindBestMatch_UniqueMisses_NoCache   | 10000         | 1000        | 0.1481 ms |         - |


