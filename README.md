<p align="center">
  <img src="icon.png" alt="GM.Mediator Samples" width="140" height="140" />
</p>

# GM.Mediator Samples

[![CI](https://github.com/gmetskhvarishvili/GM.Mediator.Samples/actions/workflows/ci.yml/badge.svg)](https://github.com/gmetskhvarishvili/GM.Mediator.Samples/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A small ASP.NET Core Web API that shows how to use **[GM.Mediator](https://www.nuget.org/packages/GM.Mediator)**
to build a clean, CQRS-style application: thin controllers that send commands through a mediator,
with cross-cutting concerns (validation, performance logging) implemented as pipeline behaviours.

## Using GM.Mediator

The package is DI-friendly and has no dependencies beyond
`Microsoft.Extensions.DependencyInjection.Abstractions`. Targets `net10.0`.

### Install

```bash
dotnet add package GM.Mediator
```

### Register

Scan one or more assemblies for handlers and behaviors:

```csharp
using GM.Mediator;

services.AddGMMediator(typeof(Program).Assembly);

// Optional: choose the service lifetime (defaults to Scoped)
services.AddGMMediator(ServiceLifetime.Transient, typeof(Program).Assembly);
```

### Requests with a response

```csharp
public record GetUser(int Id) : IRequest<UserDto>;

public class GetUserHandler : IRequestHandler<GetUser, UserDto>
{
    public Task<UserDto> Handle(GetUser request, CancellationToken ct) => /* ... */;
}

// usage
UserDto user = await mediator.Send(new GetUser(42));
```

### Requests without a response

```csharp
public record DeleteUser(int Id) : IRequest;

public class DeleteUserHandler : IRequestHandler<DeleteUser>
{
    public Task Handle(DeleteUser request, CancellationToken ct) => /* ... */;
}

await mediator.Send(new DeleteUser(42));
```

### Notifications (multiple handlers)

```csharp
public record UserCreated(int Id) : INotification;

public class SendWelcomeEmail : INotificationHandler<UserCreated> { /* ... */ }
public class UpdateAnalytics  : INotificationHandler<UserCreated> { /* ... */ }

await mediator.Publish(new UserCreated(42)); // both handlers run
```

### Pipeline behaviors

Behaviors wrap handlers and run in registration order (first registered = outermost).
Both response and no-response variants are supported.

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        // before
        var response = await next();
        // after
        return response;
    }
}
```

Open generic behaviors are registered automatically by `AddGMMediator`. This repository shows
both behaviors in action — see [`GM.Mediator.Sample.Application/Behaviours`](GM.Mediator.Sample.Application/Behaviours).

## What this demonstrates

- **Commands with a response** — `CreateSampleCommand : IRequest<string>` and its handler.
- **Commands without a response** — `UpdateSampleCommand : IRequest` and its handler.
- **Pipeline behaviours** — a `RequestValidationBehavior` (FluentValidation) and a
  `RequestPerformanceBehaviour` (logs requests slower than 500 ms), wired up in registration order.
- **Automatic validator discovery** — `AddValidatorsFromAssembly`, with a
  `CreateSampleCommandValidator` that the pipeline runs before the handler.
- **Clean error handling** — a validation failure becomes an RFC 9457 `ProblemDetails` (HTTP 400)
  via an `IExceptionHandler`, instead of an unhandled 500.
- **Thin controllers** — a `BaseController` exposes `Mediator`; controllers just send and return.

## Project structure

```
GM.Mediator.Samples/
├── GM.Mediator.Sample.API/           # ASP.NET Core Web API (controllers, Program, exception handler)
├── GM.Mediator.Sample.Application/   # Commands, handlers, validators, pipeline behaviours, DI
└── tests/
    └── GM.Mediator.Sample.Tests/     # Unit tests + in-memory API integration tests
```

The **Application** layer has no dependency on ASP.NET Core — it only knows about GM.Mediator and
FluentValidation. The **API** layer wires it up and exposes it over HTTP.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- The [`GM.Mediator`](https://www.nuget.org/packages/GM.Mediator) NuGet package (restored automatically).

## Running

```bash
dotnet run --project GM.Mediator.Sample.API
```

Then open **https://localhost:7186/swagger** to try the endpoints, or use the ready-made requests in
[`GM.Mediator.Sample.API.http`](GM.Mediator.Sample.API/GM.Mediator.Sample.API.http).

### Endpoints

| Method | Route | Body | Result |
| --- | --- | --- | --- |
| `POST` | `/samples` | `{ "test": "hello" }` | `200` + `"Property hello passed successfully!"` |
| `POST` | `/samples` | `{ "test": "" }` | `400` ProblemDetails (validation failed) |
| `PUT` | `/samples` | `{ "test": "hello" }` | `200` |

Example:

```bash
curl -k -X POST https://localhost:7186/samples \
  -H "Content-Type: application/json" \
  -d '{ "test": "hello" }'
```

## How a request flows

```
HTTP POST /samples
  → SamplesController.Create
    → IMediator.Send(CreateSampleCommand)
      → RequestValidationBehavior   (runs validators; throws on failure -> 400)
        → RequestPerformanceBehaviour  (times the handler; warns if slow)
          → CreateSampleCommandHandler   (does the work, returns the result)
```

## Testing

```bash
dotnet test
```

The suite covers the handler and validator (unit) and the endpoints end-to-end using
`WebApplicationFactory` (integration) — including that an invalid command returns `400`.

## License

MIT — see [LICENSE](LICENSE).
