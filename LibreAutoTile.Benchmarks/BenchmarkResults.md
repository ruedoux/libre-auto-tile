## Qwaitumin.LibreAutoTile.Benchmark.AutoTilerBenchmark-20260222-200037


BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.103
  [Host]   : .NET 9.0.12 (9.0.12, 9.0.1225.60609), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.12 (9.0.12, 9.0.1225.60609), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

 Method                        | TileMaskCount | Mean     | Error    | StdDev   | Gen0     | Allocated |
------------------------------ |-------------- |---------:|---------:|---------:|---------:|----------:|
 **PlaceTile_SingleLayer_100x100** | **100**           | **34.85 ms** | **1.244 ms** | **0.068 ms** | **200.0000** |  **11.52 MB** |
 **PlaceTile_SingleLayer_100x100** | **1000**          | **66.87 ms** | **3.163 ms** | **0.173 ms** | **125.0000** |  **11.54 MB** |


## Qwaitumin.LibreAutoTile.Benchmark.TileMaskSearcherBenchmark-20260222-200052


BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.103
  [Host]   : .NET 9.0.12 (9.0.12, 9.0.1225.60609), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.12 (9.0.12, 9.0.1225.60609), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

 Method                                | TileMaskCount | Mean      | Error     | StdDev    | Gen0   | Allocated |
-------------------------------------- |-------------- |----------:|----------:|----------:|-------:|----------:|
 **FindBestMatchSingle_BestCaseScenario**  | **1000**          | **0.0000 ms** | **0.0000 ms** | **0.0000 ms** | **0.0010** |      **48 B** |
 FindBestMatchSingle_WorstCaseScenario | 1000          | 0.0002 ms | 0.0000 ms | 0.0000 ms |      - |         - |
 FindBestMatchBatch_BestCaseScenario   | 1000          | 0.0152 ms | 0.0008 ms | 0.0000 ms | 0.9460 |   48000 B |
 FindBestMatchBatch_WorstCaseScenario  | 1000          | 0.1697 ms | 0.0016 ms | 0.0001 ms |      - |         - |
 **FindBestMatchSingle_BestCaseScenario**  | **10000**         | **0.0000 ms** | **0.0000 ms** | **0.0000 ms** | **0.0010** |      **48 B** |
 FindBestMatchSingle_WorstCaseScenario | 10000         | 0.0005 ms | 0.0000 ms | 0.0000 ms |      - |         - |
 FindBestMatchBatch_BestCaseScenario   | 10000         | 0.1935 ms | 0.0072 ms | 0.0004 ms | 9.5215 |  480000 B |
 FindBestMatchBatch_WorstCaseScenario  | 10000         | 6.5790 ms | 0.6648 ms | 0.0364 ms |      - |         - |


