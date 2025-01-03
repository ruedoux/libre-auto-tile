```

BenchmarkDotNet v0.14.0, Arch Linux
AMD Ryzen 7 7800X3D, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.105
  [Host]   : .NET 9.0.4 (9.0.425.16305), X64 AOT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1
WarmupCount=3

```

| Method                                   | N         |          Mean |         Error |        StdDev |       Gen0 | Allocated |
| ---------------------------------------- | --------- | ------------: | ------------: | ------------: | ---------: | --------: |
| **FindBestMatchSingle_BestCaseScenario** | **1000**  | **0.0000 ms** | **0.0000 ms** | **0.0000 ms** | **0.0010** |  **48 B** |
| FindBestMatchSingle_WorstCaseScenario    | 1000      |     0.0001 ms |     0.0000 ms |     0.0000 ms |          - |         - |
| FindBestMatchBatch_BestCaseScenario      | 1000      |     0.0193 ms |     0.0028 ms |     0.0002 ms |     0.9460 |   48000 B |
| FindBestMatchBatch_WorstCaseScenario     | 1000      |     0.1969 ms |     0.0028 ms |     0.0002 ms |          - |         - |
| **FindBestMatchSingle_BestCaseScenario** | **10000** | **0.0000 ms** | **0.0000 ms** | **0.0000 ms** | **0.0010** |  **48 B** |
| FindBestMatchSingle_WorstCaseScenario    | 10000     |     0.0001 ms |     0.0000 ms |     0.0000 ms |          - |         - |
| FindBestMatchBatch_BestCaseScenario      | 10000     |     0.2441 ms |     0.0286 ms |     0.0016 ms |     9.5215 |  480000 B |
| FindBestMatchBatch_WorstCaseScenario     | 10000     |     9.2223 ms |     0.9602 ms |     0.0526 ms |          - |      22 B |
