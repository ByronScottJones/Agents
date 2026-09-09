# Performance Engineering

Load this file when optimizing, capacity planning, or investigating latency,
throughput, memory, or resource consumption.

## Method

- Define measurable targets and representative workloads before optimizing.
- Establish a baseline, identify the bottleneck, make one focused change, and measure again.
- Use percentiles and resource saturation, not averages alone.
- Prefer profiling and tracing evidence over intuition.

## Validation

- Use load, stress, soak, and capacity tests according to the risk being evaluated.
- Test realistic data sizes, concurrency, dependencies, and failure conditions.
- Record environment, workload, tool version, and results so measurements are repeatable.
- Do not trade away correctness, security, or operability without an explicit decision.
