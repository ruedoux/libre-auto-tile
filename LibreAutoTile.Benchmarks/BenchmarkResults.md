## LibreAutoTile.Benchmark.AutoTilerBenchmark-20261117-202358

```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.114
  [Host]   : .NET 9.0.22 (9.0.22, 9.0.2226.52218), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.22 (9.0.22, 9.0.2226.52218), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=15  
LaunchCount=1  UnrollFactor=1  WarmupCount=4  

```
| Method               | TileGridSize | Mean      | Allocated  |
|--------------------- |------------- |----------:|-----------:|
| **PlaceTiles_NoCache**   | **256**          |  **25.31 ms** |  **520.08 KB** |
| PlaceTiles_WithCache | 256          |  14.67 ms |  520.08 KB |
| **PlaceTiles_NoCache**   | **512**          | **101.39 ms** | **2064.08 KB** |
| PlaceTiles_WithCache | 512          |  60.07 ms | 2064.08 KB |


## LibreAutoTile.Benchmark.AutoTilerChunkBenchmark-20261117-202405

```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.114
  [Host]   : .NET 9.0.22 (9.0.22, 9.0.2226.52218), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.22 (9.0.22, 9.0.2226.52218), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=15  
LaunchCount=1  UnrollFactor=1  WarmupCount=4  

```
| Method     | TileGridSize | StorageChunkSize | ChunkPoolCapacity | Mean     | Allocated   |
|----------- |------------- |----------------- |------------------ |---------:|------------:|
| **PlaceTiles** | **256**          | **32**               | **0**                 | **15.71 ms** |  **4617.58 KB** |
| **PlaceTiles** | **256**          | **32**               | **64**                | **14.72 ms** |   **520.08 KB** |
| **PlaceTiles** | **256**          | **64**               | **0**                 | **15.88 ms** |  **4616.45 KB** |
| **PlaceTiles** | **256**          | **64**               | **64**                | **15.30 ms** |   **520.08 KB** |
| **PlaceTiles** | **256**          | **128**              | **0**                 | **15.82 ms** |  **4616.17 KB** |
| **PlaceTiles** | **256**          | **128**              | **64**                | **14.25 ms** |   **520.08 KB** |
| **PlaceTiles** | **512**          | **32**               | **0**                 | **65.30 ms** | **18454.08 KB** |
| **PlaceTiles** | **512**          | **32**               | **64**                | **66.18 ms** | **14356.58 KB** |
| **PlaceTiles** | **512**          | **64**               | **0**                 | **65.05 ms** | **18449.58 KB** |
| **PlaceTiles** | **512**          | **64**               | **64**                | **60.22 ms** |  **2064.08 KB** |
| **PlaceTiles** | **512**          | **128**              | **0**                 | **61.82 ms** | **18448.45 KB** |
| **PlaceTiles** | **512**          | **128**              | **64**                | **60.95 ms** |  **2064.08 KB** |


## LibreAutoTile.Benchmark.AutoTilerConcurrentBenchmark-20261117-202423

```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.114
  [Host]   : .NET 9.0.22 (9.0.22, 9.0.2226.52218), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.22 (9.0.22, 9.0.2226.52218), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=15  
LaunchCount=1  UnrollFactor=1  WarmupCount=4  

```
| Method                                | LayerCount | TileGridSize | Mean      | Allocated |
|-------------------------------------- |----------- |------------- |----------:|----------:|
| **PlaceTiles_DifferentLayers_Sequential** | **4**          | **256**          | **104.24 ms** |   **2.03 MB** |
| PlaceTiles_DifferentLayers_Parallel   | 4          | 256          |  42.16 ms |   2.03 MB |
| **PlaceTiles_DifferentLayers_Sequential** | **8**          | **256**          | **206.38 ms** |   **4.06 MB** |
| PlaceTiles_DifferentLayers_Parallel   | 8          | 256          |  64.47 ms |   4.07 MB |


## LibreAutoTile.Benchmark.TileMaskSearcherBenchmark-20261117-202436

```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.114
  [Host]   : .NET 9.0.22 (9.0.22, 9.0.2226.52218), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.22 (9.0.22, 9.0.2226.52218), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=3  
LaunchCount=1  UnrollFactor=1  WarmupCount=3  

```
| Method                               | TileMaskCount | LookupCount | Mean      | Allocated |
|------------------------------------- |-------------- |------------ |----------:|----------:|
| **FindBestMatch_ExactMaskFastPath**      | **1000**          | **1000**        | **0.0734 ms** |         **-** |
| FindBestMatch_SearchMiss             | 1000          | 1000        | 0.5700 ms |    8136 B |
| FindBestMatch_RepeatedMiss_WithCache | 1000          | 1000        | 0.1191 ms |    8136 B |
| FindBestMatch_RepeatedMiss_NoCache   | 1000          | 1000        | 0.5664 ms |    8136 B |
| FindBestMatch_UniqueMisses_WithCache | 1000          | 1000        | 0.0748 ms |         - |
| FindBestMatch_UniqueMisses_NoCache   | 1000          | 1000        | 0.0719 ms |         - |
| **FindBestMatch_ExactMaskFastPath**      | **10000**         | **1000**        | **0.0703 ms** |         **-** |
| FindBestMatch_SearchMiss             | 10000         | 1000        | 1.4735 ms |   80136 B |
| FindBestMatch_RepeatedMiss_WithCache | 10000         | 1000        | 0.1194 ms |   80136 B |
| FindBestMatch_RepeatedMiss_NoCache   | 10000         | 1000        | 1.4923 ms |   80136 B |
| FindBestMatch_UniqueMisses_WithCache | 10000         | 1000        | 0.0915 ms |         - |
| FindBestMatch_UniqueMisses_NoCache   | 10000         | 1000        | 0.1024 ms |         - |


## LibreAutoTile.Benchmark.TileMaskSearcherConcurrentBenchmark-20261117-202440

```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.114
  [Host]   : .NET 9.0.22 (9.0.22, 9.0.2226.52218), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.22 (9.0.22, 9.0.2226.52218), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                   | TileMaskCount | WorkerCount | LookupCount | Mean      | Allocated |
|------------------------- |-------------- |------------ |------------ |----------:|----------:|
| **FindBestMatch_Sequential** | **1000**          | **2**           | **1000**        | **0.0073 ms** |         **-** |
| FindBestMatch_Parallel   | 1000          | 2           | 1000        | 0.0051 ms |    2144 B |
| **FindBestMatch_Sequential** | **1000**          | **4**           | **1000**        | **0.0072 ms** |         **-** |
| FindBestMatch_Parallel   | 1000          | 4           | 1000        | 0.0047 ms |    2665 B |
| **FindBestMatch_Sequential** | **1000**          | **8**           | **1000**        | **0.0072 ms** |         **-** |
| FindBestMatch_Parallel   | 1000          | 8           | 1000        | 0.0044 ms |    3300 B |


