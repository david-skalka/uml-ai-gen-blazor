# E2E — Playwright + NUnit

Deterministické UI testy pro Blazor WASM (stejné scénáře jako Avalonia E2E v [uml-ai-gen](https://github.com)).

## Soubory

| Soubor | Účel |
|--------|------|
| `E2eTestBase.cs` | Seeding/cleanup přes `[Property("Seeder", ...)]` |
| `Infrastructure/E2eTestHost.cs` | API + Blazor dev server |
| `Utils/PlaywrightPageExtensions.cs` | Pomocné lokátory a `EventuallyHaveCountAsync` |
| `TodoListE2eTests.cs`, `AlarmE2eTests.cs` | CRUD + GroupByName scénáře |

## Spuštění

```bash
pwsh TodoAppTest/bin/Debug/net10.0/playwright.ps1 install
dotnet test TodoAppTest/TodoAppTest.csproj --filter "Category=E2e"
```

Headless/headed: `TodoAppTest/playwright.runsettings` nebo env `HEADED=0` / `HEADED=1`.
