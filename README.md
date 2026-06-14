# AmdahlExperiment — Parallel Scalability Analysis

A C# console application investigating the scalability of a parallel
algorithm in light of **Amdahl's Law**. The program processes a single
input file (in three size variants: small, medium, large) through three
distinct phases: split, parallel sort, and merge.

## Project Structure
AmdahlExperiment/

├── Core/

│   ├── FileGenerator.cs       # Generates 3 test files (small/medium/large)

│   ├── SelectionSorter.cs     # O(n²) sorting algorithm

│   ├── FileSplitter.cs        # Splits input into P chunk files (sequential)

│   ├── Merger.cs              # Merges P sorted chunks into one file (sequential)

│   ├── ParallelProcessor.cs   # Orchestrates split → parallel sort → merge

│   ├── BenchmarkRunner.cs     # Runs experiments, exports CSV

│   ├── AmdahlCalculator.cs    # Amdahl's Law calculations

│   └── BenchmarkResult.cs     # Result data model

├── Data/                       # Generated input files

├── Results/                    # CSV output

├── Temp/                       # Intermediate chunk files (created at runtime)

└── Program.cs

## Experiment Overview

Three datasets of different sizes are generated automatically:

| Dataset | Integers  |
| ------- | --------- |
| small   | 10 000    |
| medium  | 50 000    |
| large   | 150 000   |

Each dataset is processed in two modes:

**Sequential (P=1):** In accordance with the assignment specification,
the sequential baseline sorts the entire array in one pass using
Selection Sort (O(n²)), without split and merge phases.

**Parallel (P=2..N):** The file is split into P chunks (written to
disk), each chunk is sorted by a separate worker thread, and the
sorted chunks are merged back into a single result.
Split (sequential) → Sort (parallel, P workers) → Merge (sequential)

## Speedup and Amdahl's Law

Speedup is calculated as:
Sp = T1 / Tp

Amdahl's Law predicts the theoretical speedup achievable given a
parallel fraction `f`:
S(P) = 1 / ((1 - f) + f/P)

The theoretical maximum speedup as P → ∞ is:
S_max = 1 / (1 - f)

The application prints `f` and `S_max` for each dataset at the end of
its benchmark run, and includes them in the generated CSV files.

## Sample Results

The exact values depend on the hardware and operating system used.
On the development machine (8 cores), a typical run produced:

| Dataset | f (parallel fraction) | Theoretical max speedup |
| ------- | ---------------------- | ------------------------ |
| small   | ~0.87                   | ~7.6                     |
| medium  | ~0.98                   | ~57                      |
| large   | ~0.99                   | ~117                     |

## Design Decisions

This section documents notable design choices and the reasoning
behind them.

### Parallel fraction estimation

`f` is calculated once, from the first parallel run (P=2), and reused
for all subsequent Amdahl predictions. This provides a single,
consistent baseline for comparing the theoretical model against
experimental results across all values of P.

### Sequential merge implementation

The merge phase sequentially merges sorted chunks pairwise:
`merge(merge(merge(c1, c2), c3), c4)...`, giving O(n·P) complexity
rather than the ideal O(n) achievable with a k-way merge (e.g. using
a priority queue). A k-way merge was not implemented because the goal
of the assignment was to study Amdahl's Law rather than to provide
the most efficient merge strategy.

### Thread isolation

Each worker thread operates on its own chunk file
(`chunk_0.txt`, `chunk_1.txt`, ...) with no shared mutable state.
Threads do not access common arrays, lists, or locks, so no
synchronization primitives are required and no race conditions occur.

### Sequential vs. parallel comparison

Per the assignment specification, the sequential (P=1) run does not
include split or merge phases — it sorts the entire array in one
pass. The parallel runs (P≥2) include split, file I/O, sort, and
merge. The measured speedup therefore also reflects the overhead
introduced by these additional phases — overhead that Amdahl's Law
attributes to the sequential portion of a program.

### Speedup exceeding the number of workers

For small and medium datasets, speedup occasionally exceeds the
number of workers. This is not a violation of Amdahl's Law, but a
consequence of how the problem size changes under partitioning:
because Selection Sort has O(n²) complexity, splitting the input into
P chunks reduces the total work from O(n²) to approximately O(n²/P),
since each of the P workers sorts a chunk of size n/P at cost
O((n/P)²). The observed speedup therefore reflects both the parallel
fraction f and this reduction in total computational work, and should
not be interpreted as pure parallel efficiency.

## Running the Project

### Requirements

* .NET 8 SDK (or compatible version)

### Run

```bash
dotnet run
```

On startup, the application will:

1. Generate datasets if they do not already exist,
2. Run the sequential benchmark (P=1),
3. Run parallel benchmarks for P=2..N (N = CPU core count),
4. Print the parallel fraction `f` and theoretical maximum speedup for each dataset,
5. Export detailed results to CSV files in `Results/`.

## Observations

* Larger datasets have a higher parallel fraction `f`, approaching 1, resulting in speedup that remains high over a wider range of P.
* Smaller datasets reach their theoretical speedup limit quickly — for the small dataset, the limit (~7.6) is nearly reached by P=5.
* Beyond a certain point, increasing P yields diminishing returns due to I/O overhead in the split and merge phases, consistent with the sequential bottleneck described by Amdahl's Law.

## Educational Objectives

This project demonstrates:

* implementation of an O(n²) sorting algorithm,
* decomposition of a task into sequential and parallel phases,
* dynamic measurement of execution time per phase,
* application of Amdahl's Law to predict and interpret speedup,
* critical analysis of the limitations and design trade-offs of a parallel implementation.