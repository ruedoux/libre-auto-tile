## Qwaitumin.LibreAutoTile.Benchmark.AutoTilerBenchmark-20260519-234113

```

BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=5  
LaunchCount=1  UnrollFactor=1  WarmupCount=4  

```
| Method               | ChunkSize | Mean      | Error     | StdDev   | Allocated |
|--------------------- |---------- |----------:|----------:|---------:|----------:|
| **PlaceTiles_NoCache**   | **256**       |  **56.38 ms** |  **3.924 ms** | **0.607 ms** |  **45.27 MB** |
| PlaceTiles_WithCache | 256       |  51.93 ms |  4.656 ms | 1.209 ms |  47.97 MB |
| **PlaceTiles_NoCache**   | **512**       | **217.99 ms** | **13.816 ms** | **3.588 ms** | **184.27 MB** |
| PlaceTiles_WithCache | 512       | 171.37 ms |  4.369 ms | 1.135 ms | 195.13 MB |


## Qwaitumin.LibreAutoTile.Benchmark.AutoTilerConcurrentBenchmark-20260519-234121

```

BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4

Job=ShortRun  InvocationCount=1  IterationCount=5  
LaunchCount=1  UnrollFactor=1  WarmupCount=4  

```
| Method                                        | LayerCount | Mean      | Error     | StdDev   | Allocated |
|---------------------------------------------- |----------- |----------:|----------:|---------:|----------:|
| **PlaceTiles_DifferentLayers_256x256_Sequential** | **4**          | **205.26 ms** |  **8.795 ms** | **2.284 ms** | **151.32 MB** |
| PlaceTiles_DifferentLayers_256x256_Parallel   | 4          |  67.89 ms |  7.579 ms | 1.968 ms | 151.33 MB |
| **PlaceTiles_DifferentLayers_256x256_Sequential** | **8**          | **401.68 ms** | **27.189 ms** | **7.061 ms** | **302.74 MB** |
| PlaceTiles_DifferentLayers_256x256_Parallel   | 8          |  94.26 ms | 15.679 ms | 4.072 ms | 302.75 MB |


