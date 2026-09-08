## Qwaitumin.LibreAutoTile.Benchmark.AutoTilerBenchmark-20260907-211210

```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 7800X3D 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.111
  [Host]   : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=15  
LaunchCount=1  UnrollFactor=1  WarmupCount=4  

```
| Method               | ChunkSize | UseStaticMap | Mean      | Median    | Allocated |
|--------------------- |---------- |------------- |----------:|----------:|----------:|
| **PlaceTiles_NoCache**   | **256**       | **False**        |  **50.18 ms** |  **50.10 ms** |  **14.43 MB** |
| PlaceTiles_WithCache | 256       | False        |  41.44 ms |  38.47 ms |  14.43 MB |
| **PlaceTiles_NoCache**   | **256**       | **True**         |  **39.72 ms** |  **37.05 ms** |   **2.78 MB** |
| PlaceTiles_WithCache | 256       | True         |  33.92 ms |  26.45 ms |   2.78 MB |
| **PlaceTiles_NoCache**   | **512**       | **False**        | **201.66 ms** | **201.65 ms** |  **62.12 MB** |
| PlaceTiles_WithCache | 512       | False        | 149.11 ms | 149.14 ms |  62.12 MB |
| **PlaceTiles_NoCache**   | **512**       | **True**         | **153.37 ms** | **152.80 ms** |  **11.95 MB** |
| PlaceTiles_WithCache | 512       | True         | 104.00 ms | 103.64 ms |  11.95 MB |


## Qwaitumin.LibreAutoTile.Benchmark.AutoTilerConcurrentBenchmark-20260907-211232

```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 7800X3D 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.111
  [Host]   : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=15  
LaunchCount=1  UnrollFactor=1  WarmupCount=4  

```
| Method                                | LayerCount | UseStaticMap | ChunkSize | Mean      | Allocated |
|-------------------------------------- |----------- |------------- |---------- |----------:|----------:|
| **PlaceTiles_DifferentLayers_Sequential** | **4**          | **False**        | **256**       | **179.23 ms** |   **57.7 MB** |
| PlaceTiles_DifferentLayers_Parallel   | 4          | False        | 256       |  59.48 ms |   57.7 MB |
| **PlaceTiles_DifferentLayers_Sequential** | **4**          | **True**         | **256**       | **144.75 ms** |   **11.1 MB** |
| PlaceTiles_DifferentLayers_Parallel   | 4          | True         | 256       |  48.84 ms |   11.1 MB |
| **PlaceTiles_DifferentLayers_Sequential** | **8**          | **False**        | **256**       | **365.51 ms** | **115.39 MB** |
| PlaceTiles_DifferentLayers_Parallel   | 8          | False        | 256       |  78.78 ms |  115.4 MB |
| **PlaceTiles_DifferentLayers_Sequential** | **8**          | **True**         | **256**       | **322.57 ms** |  **22.19 MB** |
| PlaceTiles_DifferentLayers_Parallel   | 8          | True         | 256       |  69.14 ms |   22.2 MB |


## Qwaitumin.LibreAutoTile.Benchmark.TileMaskSearcherBenchmark-20260907-211306

```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 7800X3D 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.111
  [Host]   : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=3  
LaunchCount=1  UnrollFactor=1  WarmupCount=3  

```
| Method                               | TileMaskCount | LookupCount | Mean      | Allocated |
|------------------------------------- |-------------- |------------ |----------:|----------:|
| **FindBestMatch_ExactMaskFastPath**      | **1000**          | **1000**        | **0.0708 ms** |         **-** |
| FindBestMatch_SearchMiss             | 1000          | 1000        | 0.5747 ms |    8136 B |
| FindBestMatch_RepeatedMiss_WithCache | 1000          | 1000        | 0.1106 ms |    8136 B |
| FindBestMatch_RepeatedMiss_NoCache   | 1000          | 1000        | 0.6085 ms |    8136 B |
| FindBestMatch_UniqueMisses_WithCache | 1000          | 1000        | 0.0838 ms |         - |
| FindBestMatch_UniqueMisses_NoCache   | 1000          | 1000        | 0.0734 ms |         - |
| **FindBestMatch_ExactMaskFastPath**      | **10000**         | **1000**        | **0.0674 ms** |         **-** |
| FindBestMatch_SearchMiss             | 10000         | 1000        | 1.4526 ms |   80136 B |
| FindBestMatch_RepeatedMiss_WithCache | 10000         | 1000        | 0.1163 ms |   80136 B |
| FindBestMatch_RepeatedMiss_NoCache   | 10000         | 1000        | 1.4276 ms |   80136 B |
| FindBestMatch_UniqueMisses_WithCache | 10000         | 1000        | 0.0883 ms |         - |
| FindBestMatch_UniqueMisses_NoCache   | 10000         | 1000        | 0.1002 ms |         - |


## Qwaitumin.LibreAutoTile.Benchmark.TileMaskSearcherConcurrentBenchmark-20260907-211309

```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 7800X3D 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.111
  [Host]   : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.19 (9.0.19, 9.0.1926.36724), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  StdDev=0.0000 ms  

```
| Method                   | TileMaskCount | WorkerCount | LookupCount | Mean      | Allocated |
|------------------------- |-------------- |------------ |------------ |----------:|----------:|
| **FindBestMatch_Sequential** | **1000**          | **2**           | **1000**        | **0.0075 ms** |         **-** |
| FindBestMatch_Parallel   | 1000          | 2           | 1000        | 0.0056 ms |    2142 B |
| **FindBestMatch_Sequential** | **1000**          | **4**           | **1000**        | **0.0081 ms** |         **-** |
| FindBestMatch_Parallel   | 1000          | 4           | 1000        | 0.0047 ms |    2664 B |
| **FindBestMatch_Sequential** | **1000**          | **8**           | **1000**        | **0.0075 ms** |         **-** |
| FindBestMatch_Parallel   | 1000          | 8           | 1000        | 0.0045 ms |    3303 B |


