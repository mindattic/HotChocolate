# HotChocolate

A minimal GraphQL gateway in C#: one HotChocolate endpoint that merges a public REST API and a local SQLite database into a single User type, all in one Program.cs.

![C#](https://img.shields.io/badge/C%23-ASP.NET%20Core-512BD4) ![.NET 7](https://img.shields.io/badge/.NET-7-512BD4) ![HotChocolate 13.5](https://img.shields.io/badge/HotChocolate-13.5-E10098) ![EF Core SQLite](https://img.shields.io/badge/EF%20Core-SQLite-003B57) ![Status example](https://img.shields.io/badge/Status-example-6b7280)

```text
            GraphQL client  (browser IDE at http://localhost:5095/graphql)
                   │   query { user(id: 1) { id name email payments { amount date } } }
                   ▼
        ┌────────────────────────────────┐
        │  GettingStarted (ASP.NET Core) │
        │  HotChocolate  Query.GetUser   │
        └───────┬──────────────┬─────────┘
                │              │
   HttpClient   │              │  EF Core
                ▼              ▼
   jsonplaceholder.typicode.com    payments.db (SQLite)
   /users/{id}  -> name, email     Payments where UserId = id
                │              │
                └──── merged ──┘
                       ▼
                 one User object
```

This project is a self-hosted GraphQL server built with HotChocolate. It acts as a gateway that unifies data from a public REST API (JSONPlaceholder) and a local SQLite database (`payments.db`) behind one GraphQL endpoint.

## Why

- See the gateway pattern end to end in a single `Program.cs`: one query, two back ends, one response.
- Learn how a HotChocolate resolver pulls services (`IHttpClientFactory`, a `DbContext`) straight into its parameters.
- Start from something that runs with `dotnet run` and seeds its own sample data.

## Features

- A GraphQL API with a single endpoint, `/graphql`.
- Fetches user profiles from a remote API: `https://jsonplaceholder.typicode.com/users/{id}`.
- Fetches payment history from a local SQLite database through Entity Framework Core.
- Combines both sources into one `User` GraphQL type.
- Seeds two sample payments for user 1 on first start if the table is empty, and prints the table to the console.
- Built with ASP.NET Core, HotChocolate 13.5 and Entity Framework Core 7 with SQLite.

## Quick start

Prerequisites: the .NET 7 SDK.

```bash
git clone https://github.com/mindattic/HotChocolate.git
cd HotChocolate/GettingStarted
dotnet run
```

Open `http://localhost:5095/graphql` in your browser (the `http` launch profile opens it for you). Paste this query and click Run:

```graphql
query {
  user(id: 1) {
    id
    name
    email
    payments {
      amount
      date
    }
  }
}
```

The response carries the user's name and email from JSONPlaceholder and their payments from `payments.db`, in one JSON object.

## How it works

The `user(id)` query is the `Query.GetUserAsync` resolver in `Program.cs`. It:

1. Calls the JSONPlaceholder API to get the user's name and email.
2. Queries `payments.db` for payment records with a matching `UserId`.
3. Merges both into one `User` and returns it. If the API returns nothing, the name is `Unknown`.

Schema:

| Type | Fields |
|---|---|
| `User` | `id`, `name`, `email`, `payments` |
| `Payment` | `paymentId`, `userId`, `amount`, `date` |

## Sample data

`Program.cs` creates the database if it does not exist and, when the `Payments` table is empty, adds two payments for user 1 (100.00 and 150.00, dated 10 and 5 days before the first run).

`GettingStarted/CreateDatabase.py` rebuilds `payments.db` from scratch with the same two payments at fixed dates (2025-03-21 and 2025-03-26):

```bash
python CreateDatabase.py
```

## Project layout

| Path | What it is |
|---|---|
| `GettingStarted/Program.cs` | Host setup, GraphQL registration, the `Query` resolver, models and `PaymentDbContext` |
| `GettingStarted/GettingStarted.csproj` | .NET 7 web project with HotChocolate and EF Core SQLite packages |
| `GettingStarted/payments.db` | The sample SQLite database |
| `GettingStarted/CreateDatabase.py` | Rebuilds the sample database |
| `GettingStarted/Properties/launchSettings.json` | `http` profile on port 5095 |

## Limitations

- This is a learning example, not a production gateway: there is no authentication, caching, batching (DataLoader) or error handling around the REST call.
- Projections, filtering and sorting are registered on the server, but the `user` field does not opt into them.
- The startup message prints `http://localhost:5000/graphql`, but the launch profile serves on port 5095; use 5095.
- The repository also contains committed `bin`, `obj` and `.vs` folders from a local build; they are not needed to run it.

## Documentation

- [HotChocolate documentation](https://chillicream.com/docs/hotchocolate) for the GraphQL server used here.

## License

This repository has no LICENSE file; all rights are reserved.

Part of [MindAttic](https://mindattic.com) — see more projects at [github.com/mindattic](https://github.com/mindattic).
