# Todo App — Blazor WebAssembly

Blazor WASM SPA port of the [uml-ai-gen](https://github.com) Avalonia desktop app. Same API and features: Todo Lists (CRUD + GroupByName) and Alarms (CRUD).

## Projects

| Project | Role |
|---------|------|
| `TodoApp.Blazor` | Blazor WebAssembly client (SPA) |
| `TodoAppApi` | ASP.NET Core REST API (SQLite) |
| `TodoApp.TestHarness` | E2E host — WASM + API na jedné URL |

## Run locally

1. Start the API (port **5000**):

   ```bash
   dotnet run --project TodoAppApi
   ```

2. Start the Blazor client (port **5154**):

   ```bash
   dotnet run --project TodoApp.Blazor
   ```

3. Open http://localhost:5154

API base URL: `appsettings.Development.json` (`http://localhost:5000`) pro standalone dev; prázdné `ApiBaseUrl` v `appsettings.json` použije stejný origin (test harness / hosted režim).

## Tests

Project `TodoAppTest` (from [uml-ai-gen](https://github.com)):

| Type | Framework | What it covers |
|------|-----------|----------------|
| Integration | NUnit + `WebApplicationFactory` | API CRUD, seeders, in-memory SQLite |
| E2E | NUnit + Playwright | Blazor UI přes `TodoApp.TestHarness` (jedna URL) |

```bash
# Integration only
dotnet test TodoAppTest/TodoAppTest.csproj --filter "Category!=E2e"

# E2E — install Playwright browsers first (see below)
pwsh TodoAppTest/bin/Debug/net10.0/playwright.ps1 install
dotnet test TodoAppTest/TodoAppTest.csproj --filter "Category=E2e"
```

E2E spouští `TodoApp.TestHarness` in-process (náhodný port na `127.0.0.1`). Není potřeba běžící `TodoAppApi` ani `TodoApp.Blazor` dev server.

### E2E (Playwright)

1. Install browsers: `pwsh TodoAppTest/bin/Debug/net10.0/playwright.ps1 install`
2. Optional: `TodoAppTest/playwright.runsettings` or env `HEADED=0` / `HEADED=1` for headless/headed.

Same scenarios as [uml-ai-gen](https://github.com) Avalonia E2E: Show, Create, Edit, Delete, GroupByName — deterministic Playwright locators (`GetByRole`, `GetByLabel`, table row counts).

## Stack

- .NET 10
- Blazor WebAssembly (standalone SPA)
- [MudBlazor](https://mudblazor.com/) UI components
- NSwag-generated HTTP client (same as Avalonia app)
- Entity Framework Core + SQLite on the API
