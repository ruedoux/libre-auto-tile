## Qwaitumin.LibreAutoTile.Benchmark.AutoTilerBenchmark-20251118-213622


BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.110
  [Host]   : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

 Method                        | TileMaskCount | Mean     | Error    | StdDev   | Gen0     | Allocated |
------------------------------ |-------------- |---------:|---------:|---------:|---------:|----------:|
 **PlaceTile_SingleLayer_100x100** | **100**           | **37.78 ms** | **1.244 ms** | **0.068 ms** | **214.2857** |  **11.54 MB** |
 **PlaceTile_SingleLayer_100x100** | **1000**          | **76.95 ms** | **1.912 ms** | **0.105 ms** | **142.8571** |  **11.54 MB** |


## Qwaitumin.LibreAutoTile.Benchmark.TileMaskSearcherBenchmark-20251118-213638


BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.110
  [Host]   : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

 Method                                | TileMaskCount | Mean      | Error     | StdDev    | Gen0   | Allocated |
-------------------------------------- |-------------- |----------:|----------:|----------:|-------:|----------:|
 **FindBestMatchSingle_BestCaseScenario**  | **1000**          | **0.0000 ms** | **0.0000 ms** | **0.0000 ms** | **0.0010** |      **48 B** |
 FindBestMatchSingle_WorstCaseScenario | 1000          | 0.0002 ms | 0.0000 ms | 0.0000 ms |      - |         - |
 FindBestMatchBatch_BestCaseScenario   | 1000          | 0.0155 ms | 0.0010 ms | 0.0001 ms | 0.9460 |   48000 B |
 FindBestMatchBatch_WorstCaseScenario  | 1000          | 0.1989 ms | 0.0005 ms | 0.0000 ms |      - |         - |
 **FindBestMatchSingle_BestCaseScenario**  | **10000**         | **0.0000 ms** | **0.0000 ms** | **0.0000 ms** | **0.0010** |      **48 B** |
 FindBestMatchSingle_WorstCaseScenario | 10000         | 0.0006 ms | 0.0000 ms | 0.0000 ms |      - |         - |
 FindBestMatchBatch_BestCaseScenario   | 10000         | 0.1962 ms | 0.0197 ms | 0.0011 ms | 9.5215 |  480000 B |
 FindBestMatchBatch_WorstCaseScenario  | 10000         | 7.7548 ms | 0.5230 ms | 0.0287 ms |      - |         - |


