## Qwaitumin.LibreAutoTile.Benchmark.AutoTilerConfigurationBenchmark-20260514-202123


BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

 Method                                      | Mean     | Error    | StdDev   | Gen0     | Allocated |
-------------------------------------------- |---------:|---------:|---------:|---------:|----------:|
 PlaceTile_SingleLayer_100x100_Configuration | 10.96 ms | 0.274 ms | 0.015 ms | 171.8750 |   8.58 MB |


## Qwaitumin.LibreAutoTile.Benchmark.AutoTilerRandomBenchmark-20260514-202131


BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

 Method                               | TileMaskCount | Mean     | Error    | StdDev   | Gen0     | Allocated |
------------------------------------- |-------------- |---------:|---------:|---------:|---------:|----------:|
 **PlaceTile_SingleLayer_100x100_Random** | **100**           | **16.81 ms** | **0.805 ms** | **0.044 ms** | **312.5000** |  **15.54 MB** |
 **PlaceTile_SingleLayer_100x100_Random** | **1000**          | **18.10 ms** | **3.984 ms** | **0.218 ms** | **312.5000** |  **15.56 MB** |


## Qwaitumin.LibreAutoTile.Benchmark.TileMaskSearcherConfigurationBenchmark-20260514-202146


BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

 Method                                           | Mean      | Error     | StdDev    | Gen0   | Allocated |
------------------------------------------------- |----------:|----------:|----------:|-------:|----------:|
 FindBestMatchAll_Configuration_BestCaseScenario  | 0.0062 ms | 0.0013 ms | 0.0001 ms | 0.2136 |  10.78 KB |
 FindBestMatchAll_Configuration_WorstCaseScenario | 0.0087 ms | 0.0002 ms | 0.0000 ms | 0.2136 |  10.83 KB |


## Qwaitumin.LibreAutoTile.Benchmark.TileMaskSearcherRandomizedBenchmark-20260514-202200


BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

 Method                                       | TileMaskCount | Mean      | Error     | StdDev    | Gen0   | Allocated |
--------------------------------------------- |-------------- |----------:|----------:|----------:|-------:|----------:|
 **FindBestMatchSingle_Random_BestCaseScenario**  | **1000**          | **0.0000 ms** | **0.0000 ms** | **0.0000 ms** | **0.0010** |      **48 B** |
 FindBestMatchSingle_Random_WorstCaseScenario | 1000          | 0.0001 ms | 0.0000 ms | 0.0000 ms | 0.0010 |      48 B |
 FindBestMatchBatch_Random_BestCaseScenario   | 1000          | 0.0160 ms | 0.0005 ms | 0.0000 ms | 0.9460 |   48000 B |
 FindBestMatchBatch_Random_WorstCaseScenario  | 1000          | 0.0484 ms | 0.0005 ms | 0.0000 ms | 0.9155 |   48000 B |
 **FindBestMatchSingle_Random_BestCaseScenario**  | **10000**         | **0.0000 ms** | **0.0000 ms** | **0.0000 ms** | **0.0010** |      **48 B** |
 FindBestMatchSingle_Random_WorstCaseScenario | 10000         | 0.0001 ms | 0.0000 ms | 0.0000 ms | 0.0010 |      48 B |
 FindBestMatchBatch_Random_BestCaseScenario   | 10000         | 0.2026 ms | 0.0186 ms | 0.0010 ms | 9.5215 |  480000 B |
 FindBestMatchBatch_Random_WorstCaseScenario  | 10000         | 5.2032 ms | 0.3557 ms | 0.0195 ms | 7.8125 |  480000 B |


