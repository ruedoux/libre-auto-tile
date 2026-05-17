## Qwaitumin.LibreAutoTile.Benchmark.AutoTilerConfigurationBenchmark-20260517-222153


BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

 Method                                      | Mean     | Error     | StdDev    | Gen0     | Gen1     | Gen2     | Allocated |
-------------------------------------------- |---------:|----------:|----------:|---------:|---------:|---------:|----------:|
 PlaceTile_SingleLayer_100x100_Configuration | 4.588 ms | 0.9735 ms | 0.0534 ms | 117.1875 | 117.1875 | 117.1875 |   3.79 MB |


## Qwaitumin.LibreAutoTile.Benchmark.AutoTilerRandomBenchmark-20260517-222159


BenchmarkDotNet v0.15.6, Linux Arch Linux
AMD Ryzen 7 7800X3D 2.98GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.104
  [Host]   : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4
  ShortRun : .NET 9.0.14 (9.0.14, 9.0.1426.11910), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

 Method                               | TileMaskCount | Mean     | Error     | StdDev    | Gen0     | Gen1    | Gen2    | Allocated |
------------------------------------- |-------------- |---------:|----------:|----------:|---------:|--------:|--------:|----------:|
 **PlaceTile_SingleLayer_100x100_Random** | **100**           | **6.608 ms** | **1.1595 ms** | **0.0636 ms** | **132.8125** | **39.0625** | **23.4375** |   **5.89 MB** |
 **PlaceTile_SingleLayer_100x100_Random** | **1000**          | **6.670 ms** | **0.6908 ms** | **0.0379 ms** | **117.1875** | **23.4375** |  **7.8125** |    **5.9 MB** |
 **PlaceTile_SingleLayer_100x100_Random** | **2000**          | **6.818 ms** | **0.2930 ms** | **0.0161 ms** | **117.1875** | **31.2500** |  **7.8125** |    **5.9 MB** |


