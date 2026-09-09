<!-- markdownlint-disable MD003 MD020 -->

# C#

Open this file only when writing or modifying C# code. General rules stay in the root `AGENTS.md`.

## Formatting & Linting

- Formatter: `dotnet format` (authoritative — never suggest conflicting style).
- Linter: Roslyn analyzers.

## Error Handling

- Use exceptions for truly exceptional conditions, not control flow.
- Prefer `Result<T>` patterns for expected failure paths.
- Always use `ConfigureAwait(false)` in library code.
- Null safety: enable `<Nullable>enable</Nullable>` — no `!` null-forgiving operator without a comment.
- No bare `catch (Exception)` — catch the most specific type possible.

## Testing

- xUnit is the default test framework.
- Use `FluentAssertions` for assertions.
- `Moq` or `NSubstitute` for mocking.
- Test method naming: `{Method}_{Scenario}_{ExpectedResult}`.
- Separate unit and integration test projects.

## Conventions

- File-scoped namespaces (`namespace Foo;`).
- Prefer records for immutable DTOs.
- Target `net8.0` or latest LTS unless constrained.
- Use `ILogger<T>` — never `Console.WriteLine` in production code.

## Code Generation Defaults

- File-scoped namespace; `using` at top.

## 1. Architecture and Project Structure

- **Minimal APIs by Default:** Prefer Minimal APIs (`Program.cs` endpoint routing) over traditional MVC Controllers for lightweight, high-performance microservices and serverless applications.
- **Clean Architecture:** Separate core business logic from infrastructure. Use distinct class libraries for domain models, application interfaces, and infrastructure implementations.
- **Configuration Management:** Use `IConfiguration` and the Options pattern (`IOptions<T>`) for strongly-typed settings. Never hardcode environment-specific variables.

## 1.1 Configuration Details

- Use `appsettings.json` and `appsettings.{Environment}.json` for application settings.
- Load configuration in this order when using the default host: appsettings files, user secrets in development, environment variables, then command-line arguments.
- Use `__` in environment variables for nested settings, such as `Logging__LogLevel__Default`.
- Use `dotnet user-secrets` for local development secrets, never for production secrets.
- Use `Environment.GetFolderPath` and `Path.Combine` for platform-specific paths.

## 2. C# Language Features & Conventions

- **Nullable Reference Types:** Enable `<Nullable>enable</Nullable>` in all `.csproj` files to catch null-reference exceptions at compile time.
- **Implicit Usings:** Enable `<ImplicitUsings>enable</ImplicitUsings>` to reduce boilerplate `using` directives in file headers.
- **File-scoped Namespaces:** Use file-scoped namespaces (`namespace Company.Project;`) to save horizontal indentation.
- **Records and Init-only Properties:** Use `record` types for immutable data transfer objects (DTOs) and `init` accessors for properties that should not change after object initialization.

## 3. Asynchronous Programming

- **Async All the Way:** Use `async` and `await` for all I/O-bound operations (database calls, HTTP requests, cloud SDK calls).
- **Cancellation Tokens:** Always pass `CancellationToken` down the call stack to allow graceful termination of long-running requests.
- **Task Management:** Avoid `.Wait()` or `.Result` to prevent deadlocks. Use `ValueTask` for high-throughput methods that frequently complete synchronously.

## 4. API Design and Endpoints

- **Endpoint Grouping:** Group Minimal API endpoints using extension methods on `IEndpointRouteBuilder` (e.g., `app.MapGroup("/api/v1/resources")`) to maintain a clean and organized `Program.cs`.
- **Typed Results:** Return `TypedResults` (e.g., `Results.Ok()`, `Results.NotFound()`) rather than primitive types to guarantee compile-time safety and generate accurate OpenAPI/Swagger documentation.

## 5. Cloud and Serverless Optimization

- **Cold Start Mitigation:** When deploying distributed architectures or serverless functions (like AWS Lambda), minimize dependencies in the deployment package. Consider Native AOT (Ahead-of-Time) compilation for extreme latency-sensitive applications.
- **Statelessness:** Ensure all endpoints are entirely stateless. Externalize temporary state to a database or distributed cache.
- **Structured Logging:** Output logs in a structured JSON format using `ILogger` to ensure downstream aggregators and alerting systems can parse telemetry efficiently.

## 6. Dependency Injection (DI)

- **Native Container:** Rely on the built-in `Microsoft.Extensions.DependencyInjection` container.
- **Lifetimes:**
  - `Transient`: For lightweight, stateless services.
  - `Scoped`: For services maintaining state within a single HTTP request lifecycle (e.g., Entity Framework `DbContext`).
  - `Singleton`: Use sparingly; ensure these services are entirely thread-safe.

## 7. Packaging and Local Environment

- **Local Tooling:** Use a `dotnet tool manifest` to manage local CLI utilities (like formatters or ORM tools) consistently across the repository.
- **Containerization:** Include a multi-stage `Dockerfile` optimized for the .NET runtime to ensure parity between local packaging, testing, and production deployment environments.
