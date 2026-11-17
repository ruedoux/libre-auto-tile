## LibreAutoTile.Benchmark.AutoTilerBenchmark-20261117-184042

```

BenchmarkDotNet v0.15.8, Linux Debian GNU/Linux 12 (bookworm)
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.404
  [Host]   : .NET 9.0.23 (9.0.23, 9.0.2326.52216), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.23 (9.0.23, 9.0.2326.52216), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=15  
LaunchCount=1  UnrollFactor=1  WarmupCount=4  

```
| Method               | TileGridSize | Mean      | Allocated |
|--------------------- |------------- |----------:|----------:|
| **PlaceTiles_NoCache**   | **256**          |  **27.10 ms** |         **-** |
| PlaceTiles_WithCache | 256          |  14.53 ms |         - |
| **PlaceTiles_NoCache**   | **512**          | **105.81 ms** |         **-** |
| PlaceTiles_WithCache | 512          |  61.88 ms |         - |


## LibreAutoTile.Benchmark.AutoTilerChunkBenchmark-20261117-184050

```

BenchmarkDotNet v0.15.8, Linux Debian GNU/Linux 12 (bookworm)
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.404
  [Host]   : .NET 9.0.23 (9.0.23, 9.0.2326.52216), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.23 (9.0.23, 9.0.2326.52216), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=15  
LaunchCount=1  UnrollFactor=1  WarmupCount=4  

```
| Method     | TileGridSize | StorageChunkSize | ChunkPoolCapacity | Mean     | Allocated  |
|----------- |------------- |----------------- |------------------ |---------:|-----------:|
| **PlaceTiles** | **256**          | **32**               | **0**                 | **15.23 ms** |  **4195840 B** |
| **PlaceTiles** | **256**          | **32**               | **64**                | **15.86 ms** |          **-** |
| **PlaceTiles** | **256**          | **64**               | **0**                 | **16.40 ms** |  **4194688 B** |
| **PlaceTiles** | **256**          | **64**               | **64**                | **15.19 ms** |          **-** |
| **PlaceTiles** | **256**          | **128**              | **0**                 | **14.84 ms** |  **4194400 B** |
| **PlaceTiles** | **256**          | **128**              | **64**                | **14.47 ms** |          **-** |
| **PlaceTiles** | **512**          | **32**               | **0**                 | **65.03 ms** | **16783360 B** |
| **PlaceTiles** | **512**          | **32**               | **64**                | **67.14 ms** | **12587520 B** |
| **PlaceTiles** | **512**          | **64**               | **0**                 | **67.92 ms** | **16778752 B** |
| **PlaceTiles** | **512**          | **64**               | **64**                | **62.67 ms** |          **-** |
| **PlaceTiles** | **512**          | **128**              | **0**                 | **63.01 ms** | **16777600 B** |
| **PlaceTiles** | **512**          | **128**              | **64**                | **59.39 ms** |          **-** |


## LibreAutoTile.Benchmark.AutoTilerConcurrentBenchmark-20261117-184108

```

BenchmarkDotNet v0.15.8, Linux Debian GNU/Linux 12 (bookworm)
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.404
  [Host]   : .NET 9.0.23 (9.0.23, 9.0.2326.52216), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.23 (9.0.23, 9.0.2326.52216), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=15  
LaunchCount=1  UnrollFactor=1  WarmupCount=4  

```
| Method                                | LayerCount | TileGridSize | Mean      | Allocated |
|-------------------------------------- |----------- |------------- |----------:|----------:|
| **PlaceTiles_DifferentLayers_Sequential** | **4**          | **256**          | **113.51 ms** |         **-** |
| PlaceTiles_DifferentLayers_Parallel   | 4          | 256          |  46.05 ms |    2376 B |
| **PlaceTiles_DifferentLayers_Sequential** | **8**          | **256**          | **216.60 ms** |         **-** |
| PlaceTiles_DifferentLayers_Parallel   | 8          | 256          |  65.49 ms |    3240 B |


## LibreAutoTile.Benchmark.TileMaskSearcherBenchmark-20261117-184121

```

BenchmarkDotNet v0.15.8, Linux Debian GNU/Linux 12 (bookworm)
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.404
  [Host]   : .NET 9.0.23 (9.0.23, 9.0.2326.52216), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.23 (9.0.23, 9.0.2326.52216), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=3  
LaunchCount=1  UnrollFactor=1  WarmupCount=3  

```
| Method                               | TileMaskCount | LookupCount | Mean      | Allocated |
|------------------------------------- |-------------- |------------ |----------:|----------:|
| **FindBestMatch_ExactMaskFastPath**      | **1000**          | **1000**        | **0.0668 ms** |         **-** |
| FindBestMatch_SearchMiss             | 1000          | 1000        | 0.5778 ms |    8136 B |
| FindBestMatch_RepeatedMiss_WithCache | 1000          | 1000        | 0.1096 ms |    8136 B |
| FindBestMatch_RepeatedMiss_NoCache   | 1000          | 1000        | 0.5403 ms |    8136 B |
| FindBestMatch_UniqueMisses_WithCache | 1000          | 1000        | 0.0717 ms |         - |
| FindBestMatch_UniqueMisses_NoCache   | 1000          | 1000        | 0.0728 ms |         - |
| **FindBestMatch_ExactMaskFastPath**      | **10000**         | **1000**        | **0.0691 ms** |         **-** |
| FindBestMatch_SearchMiss             | 10000         | 1000        | 1.3467 ms |   80136 B |
| FindBestMatch_RepeatedMiss_WithCache | 10000         | 1000        | 0.1131 ms |   80136 B |
| FindBestMatch_RepeatedMiss_NoCache   | 10000         | 1000        | 1.3814 ms |   80136 B |
| FindBestMatch_UniqueMisses_WithCache | 10000         | 1000        | 0.0904 ms |         - |
| FindBestMatch_UniqueMisses_NoCache   | 10000         | 1000        | 0.0959 ms |         - |


## LibreAutoTile.Benchmark.TileMaskSearcherConcurrentBenchmark-20261117-184124

```

BenchmarkDotNet v0.15.8, Linux Debian GNU/Linux 12 (bookworm)
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.404
  [Host]   : .NET 9.0.23 (9.0.23, 9.0.2326.52216), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.23 (9.0.23, 9.0.2326.52216), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                   | TileMaskCount | WorkerCount | LookupCount | Mean      | Allocated |
|------------------------- |-------------- |------------ |------------ |----------:|----------:|
| **FindBestMatch_Sequential** | **1000**          | **2**           | **1000**        | **0.0074 ms** |         **-** |
| FindBestMatch_Parallel   | 1000          | 2           | 1000        | 0.0055 ms |    2143 B |
| **FindBestMatch_Sequential** | **1000**          | **4**           | **1000**        | **0.0075 ms** |         **-** |
| FindBestMatch_Parallel   | 1000          | 4           | 1000        | 0.0048 ms |    2671 B |
| **FindBestMatch_Sequential** | **1000**          | **8**           | **1000**        | **0.0074 ms** |         **-** |
| FindBestMatch_Parallel   | 1000          | 8           | 1000        | 0.0047 ms |    3349 B |


