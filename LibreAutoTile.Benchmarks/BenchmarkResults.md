## Qwaitumin.LibreAutoTile.Benchmark.AutoTilerBenchmark-20260520-165043

```

BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=15  
LaunchCount=1  UnrollFactor=1  WarmupCount=4  

```
| Method               | ChunkSize | UseStaticMap | Mean      | Allocated |
|--------------------- |---------- |------------- |----------:|----------:|
| **PlaceTiles_NoCache**   | **256**       | **False**        |  **49.58 ms** |  **14.43 MB** |
| PlaceTiles_WithCache | 256       | False        |  42.05 ms |  14.43 MB |
| **PlaceTiles_NoCache**   | **256**       | **True**         |  **39.49 ms** |   **2.78 MB** |
| PlaceTiles_WithCache | 256       | True         |  32.35 ms |   2.78 MB |
| **PlaceTiles_NoCache**   | **512**       | **False**        | **205.47 ms** |  **62.12 MB** |
| PlaceTiles_WithCache | 512       | False        | 154.20 ms |  62.12 MB |
| **PlaceTiles_NoCache**   | **512**       | **True**         | **157.12 ms** |  **11.95 MB** |
| PlaceTiles_WithCache | 512       | True         | 109.04 ms |  11.95 MB |


## Qwaitumin.LibreAutoTile.Benchmark.AutoTilerConcurrentBenchmark-20260520-165105

```

BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=15  
LaunchCount=1  UnrollFactor=1  WarmupCount=4  

```
| Method                                | LayerCount | UseStaticMap | ChunkSize | Mean      | Allocated |
|-------------------------------------- |----------- |------------- |---------- |----------:|----------:|
| **PlaceTiles_DifferentLayers_Sequential** | **4**          | **False**        | **256**       | **183.31 ms** |   **57.7 MB** |
| PlaceTiles_DifferentLayers_Parallel   | 4          | False        | 256       |  59.44 ms |   57.7 MB |
| **PlaceTiles_DifferentLayers_Sequential** | **4**          | **True**         | **256**       | **145.21 ms** |   **11.1 MB** |
| PlaceTiles_DifferentLayers_Parallel   | 4          | True         | 256       |  51.06 ms |   11.1 MB |
| **PlaceTiles_DifferentLayers_Sequential** | **8**          | **False**        | **256**       | **367.18 ms** | **115.39 MB** |
| PlaceTiles_DifferentLayers_Parallel   | 8          | False        | 256       |  80.89 ms |  115.4 MB |
| **PlaceTiles_DifferentLayers_Sequential** | **8**          | **True**         | **256**       | **313.58 ms** |  **22.19 MB** |
| PlaceTiles_DifferentLayers_Parallel   | 8          | True         | 256       |  66.65 ms |   22.2 MB |


## Qwaitumin.LibreAutoTile.Benchmark.TileMaskSearcherBenchmark-20260520-165139

```

BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=3  
LaunchCount=1  UnrollFactor=1  WarmupCount=3  

```
| Method                               | TileMaskCount | LookupCount | Mean      | Allocated |
|------------------------------------- |-------------- |------------ |----------:|----------:|
| **FindBestMatch_ExactMaskFastPath**      | **1000**          | **1000**        | **0.0679 ms** |         **-** |
| FindBestMatch_SearchMiss             | 1000          | 1000        | 0.5786 ms |    8136 B |
| FindBestMatch_RepeatedMiss_WithCache | 1000          | 1000        | 0.1177 ms |    8136 B |
| FindBestMatch_RepeatedMiss_NoCache   | 1000          | 1000        | 0.5781 ms |    8136 B |
| FindBestMatch_UniqueMisses_WithCache | 1000          | 1000        | 0.0721 ms |         - |
| FindBestMatch_UniqueMisses_NoCache   | 1000          | 1000        | 0.0753 ms |         - |
| **FindBestMatch_ExactMaskFastPath**      | **10000**         | **1000**        | **0.0687 ms** |         **-** |
| FindBestMatch_SearchMiss             | 10000         | 1000        | 1.5126 ms |   80136 B |
| FindBestMatch_RepeatedMiss_WithCache | 10000         | 1000        | 0.1176 ms |   80136 B |
| FindBestMatch_RepeatedMiss_NoCache   | 10000         | 1000        | 1.4624 ms |   80136 B |
| FindBestMatch_UniqueMisses_WithCache | 10000         | 1000        | 0.0864 ms |         - |
| FindBestMatch_UniqueMisses_NoCache   | 10000         | 1000        | 0.0888 ms |         - |


## Qwaitumin.LibreAutoTile.Benchmark.TileMaskSearcherConcurrentBenchmark-20260520-165142

```

BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                   | TileMaskCount | WorkerCount | LookupCount | Mean      | Allocated |
|------------------------- |-------------- |------------ |------------ |----------:|----------:|
| **FindBestMatch_Sequential** | **1000**          | **2**           | **1000**        | **0.0076 ms** |         **-** |
| FindBestMatch_Parallel   | 1000          | 2           | 1000        | 0.0052 ms |    2145 B |
| **FindBestMatch_Sequential** | **1000**          | **4**           | **1000**        | **0.0075 ms** |         **-** |
| FindBestMatch_Parallel   | 1000          | 4           | 1000        | 0.0048 ms |    2662 B |
| **FindBestMatch_Sequential** | **1000**          | **8**           | **1000**        | **0.0075 ms** |         **-** |
| FindBestMatch_Parallel   | 1000          | 8           | 1000        | 0.0045 ms |    3310 B |


