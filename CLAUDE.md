# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Congratulator ("Поздравлятор") — a birthday management SPA. ASP.NET Core Web API backend + Blazor WebAssembly frontend. Stores person records with photos in Yandex S3, backed by PostgreSQL.

## CLAUDE tip

When you view files, add CLAUDE-*something*.md files for indexing. This will save you tokens and context and help you quickly understand what files are in that folder.

In this file, write down what's in that folder, what it does and how it does it, why it exists, etc.

These are instructions for yourself.

## Build & Run Commands

```bash
# Build entire solution
dotnet build Congratulator.sln

# Run API server (default: https://localhost:7228)
dotnet run --project src/Congratulator.Api

# Run all tests
dotnet test src/Congratulator.XUnitTests

# Run a single test
dotnet test src/Congratulator.XUnitTests --filter "FullyQualifiedName~TestMethodName"

# Add EF Core migration
dotnet ef migrations add MigrationName -p src/Congratulator.Infrastructure -s src/Congratulator.Api

# Apply migrations (also runs automatically on startup when MigrateDbOnStartup=true)
dotnet ef database update -p src/Congratulator.Infrastructure -s src/Congratulator.Api
```

## Architecture

Layered clean architecture with 7 projects:

```
Api → Core → SharedKernel
 ↓                ↑
Infrastructure ───┘
       ↑
SharedKernel.Contracts (DTOs, enums — referenced by multiple layers)
WebAssembly (standalone Blazor WASM client)
XUnitTests (repository tests with InMemory EF provider)
```

- **Api** — Single controller (`PersonsController`), FluentValidation auto-validation, Serilog, Swagger at `/api/swagger`, CORS for Blazor client
- **Core** — Service-per-operation pattern: `CreatePersonService`, `UpdatePersonService`, `DeletePersonService`, `GetPersonsService`, `GetPagedPersonsService`. Auto-registered via reflection in `CoreConfiguration`
- **Infrastructure** — `CongratulatorDbContext` (PostgreSQL via Npgsql), `PersonRepository` with complex filtering/sorting, `YandexS3Service` for photo storage, AutoMapper profiles. Auto-registered via reflection in `AddInfrastructure`
- **SharedKernel** — `Person` entity, repository/service interfaces
- **SharedKernel.Contracts** — Request/response DTOs, `PersonModel` (with computed `Age`, `NextBirthday`, `DaysUntilBirthday`), enums (`RelationshipType`, `SortVariants`)
- **WebAssembly** — Blazor WASM client with components (`AddPersonModal`, `Pagination`, `PersonCard`), pages (`Home`, `All`, `Chart`, `Testing`, `UiKitIcons`)

## Key Technical Details

- **.NET 9 / C# 13** with nullable reference types enabled
- **DateOnly** used for birth dates with custom EF Core value converter
- **Enums stored as strings** in PostgreSQL
- **Birthday sorting** uses MMDD format for year-independent ordering
- **Pagination** — fixed page size of 8; fetches pageSize+1 to determine `HasNext`
- **Photo upload flow**: base64 in API request → stream → Yandex S3; defaults to `"default.png"`
- **JSON naming**: custom `JsonCamelCaseWithDotsNamingPolicy` (PascalCase → camelCase, preserving dots)
- **Auto-migration** on startup controlled by `MigrateDbOnStartup` config flag
- **Connection string** key: `ConnectionStrings:IdentityConnectionString`
- **Swagger** available in Development at `/api/swagger`
- **Fetch all persons** for aggregation: `GET /persons?all=true&page=1&pageSize=10000` — `GetPagedPersonsService` is used when `all=true`, no server-side cap on `pageSize`
- **IBM Plex Mono** font loaded in `index.html` — use for numeric/monospace content in UI

## Configuration

Required in `appsettings.json`:
- `ConnectionStrings:IdentityConnectionString` — PostgreSQL connection string
- `YandexS3` section — `AccessKey`, `SecretKey`, `BucketName`, `ServiceURL`, `Region`

## MANDATORY CODING STANDARDS

### Task Clarification
- **ALWAYS** ask for clarification if task formulation is unclear or ambiguous
- **ALWAYS** ask if there are multiple architectural approaches (composition vs duplication vs extension)
- **ALWAYS** present options when creating components based on existing ones:
    - Option 1: Reuse existing component (composition) ⭐ **PREFERRED BY DEFAULT**
    - Option 2: Create separate implementation (duplication)
    - Option 3: Extend existing component
- Do not make assumptions about what the user wants
- Ask specific questions to understand the exact requirements
- If you see opportunity to reuse existing code, **ask first** before duplicating

### “THREE QUESTIONS” RULE (MANDATORY!)

Before writing code for ANY new component/service/function, ask yourself:

1. **Can this be reused?** — Is there an existing component/code that can be wrapped or extended?
2. **How many implementation options exist?** — Are there alternative architectural approaches?
3. **What is easier to maintain?** — Which approach will create less technical debt and duplication?

**MANDATORY:** If there are 2+ implementation options, present them to the user with a brief summary of pros/cons.

**Examples of when you must ask:**
- Creating a component based on an existing one
- Adding functionality to an existing class vs creating a new one
- Choosing between inheritance and composition
- Any decision that may lead to code duplication

### C# Code Structure
- Use `var` everywhere
- Use file-scoped namespaces in classes
- Use primary constructors in classes
- Keep methods under 50 lines when possible
- Use meaningful parameter and variable names
- Prefer async/await pattern for asynchronous operations
- Use .editorconfig file for naming rules
- Add XML documentation for public APIs in project interfaces only
- Use PowerShell as terminal

### PowerShell Commands
- **ALWAYS** execute PowerShell commands (Test-Path, New-Item, etc.) using `powershell -Command` wrapper
- **NEVER** run PowerShell cmdlets directly in bash
- Example: `powershell -Command "Test-Path 'C:\path\to\file'"`

### Before Task Completion Checklist
- [ ] Solution builds without errors (`dotnet build` — bare `-nowarn` flag doesn't work, omit it)
- [ ] Unit tests are green (`dotnet test`)
- [ ] Files/folders checked for existence before creation

## 🚫 FORBIDDEN ACTIONS (NEVER DO)

### Version Management
- **NEVER** change .NET Framework versions in project files (.csproj) without explicit permission
- **NEVER** update NuGet package versions without explicit permission
- **NEVER** modify global.json settings without explicit permission
- **NEVER** change any dependency versions without explicit permission

### Git Operations
- **NEVER** execute any git commands without explicit permission
- **NEVER** use git add, commit, push, restore, checkout, merge or any other git operations
- Only read-only git commands (git status, git log) are allowed for information purposes

### Error Handling
- **NEVER** check input arguments or throw ArgumentException (unless specifically requested)
- **NEVER** use generic Exception - use specific exception types

### Development
- **NEVER** create files/folders without checking if they exist first
- **NEVER** leave build errors after implementation - task is not complete until build is clean
- **NEVER** leave failing unit tests without notifying user first
## Blazor Component Development

### 🔴 MANDATORY BLAZOR ACTIONS

#### Hot Reload & Rebuilding
- When the dev server is running, `dotnet build` will fail with a PDB lock error — this is expected
- CSS and Razor changes are picked up by hot reload automatically — no rebuild needed
- In you need to rebuild solution: use the `/rebuild` skill (runs `rebuild.ps1`)

#### Component Structure
- Always create 3 files: `Component.razor`, `Component.razor.cs`, `Component.razor.css`
- Add `[Parameter]` properties for configuration
- Use standard HTML attributes (disabled, etc.) instead of custom state parameters
- Implement `ValueChanged` EventCallback for two-way binding
- Add `ValueExpression` for form validation support

#### CSS Requirements
- **MUST use CSS variables** - NO hardcoded colors/sizes
- Use **rem** units for CSS
- Use `rgba()` format for all colors: `rgba(255, 255, 255, 1)`
- Include `box-sizing: border-box` and appropriate `display` properties
- LumexUI CSS variables (`--lumex-*`) come from `bin/lumexui/theme` — imported in `wwwroot/css/app.css`. There is no `tokens.css` for component variables; put custom tokens directly in the component's `.razor.css`
- **For scrollable elements**: Apply `custom-scrollbar` CSS class to enable styled scrollbars

#### LumexUI Usage Rules
- **ALWAYS** use LumexUI components (`LumexCard`, `LumexChip`, `LumexButton`, `LumexSpinner`, `LumexSkeleton`, etc.) — never build UI primitives from scratch when LumexUI has them
- **NEVER** write Tailwind utility classes directly in `.razor` markup — Tailwind is only used internally by LumexUI
- All custom styling goes in **`.razor.css` scoped files** as named CSS classes, not inline Tailwind
- To customise a LumexUI component, use its `Class=` parameter to attach a scoped CSS class, then write the overrides in `.razor.css`
- Use `--lumex-*` CSS variables for all colors, radii, shadows, and spacing to stay theme-aware (light/dark)

#### Icon Requirements
- **ALWAYS** use RemixIcon CSS classes for icons: `<i class="ri-iconname-style"></i>`
- **Icon naming pattern**: `ri-{icon-name}-{style}` where style is `fill` or `line`
- **Examples**: `ri-home-fill`, `ri-star-line`, `ri-add-circle-fill`, `ri-close-line`
- **Reference file**: All available icons are listed in `Congratulator.WebAssembly/wwwroot/index.html`
- **NEVER** use SVG files or `_content` paths for icons anymore
- Icons are font-based and can be styled with CSS `color`, `font-size`, etc.

### Component Completion Checklist
- [ ] CSS variables used instead of hardcoded values
- [ ] All 3 component files created (.razor, .razor.cs, .razor.css)
- [ ] New page: add route to `Routes.Pages`, add nav item in `MainLayout.razor`

## WebAssembly Conventions

### BasePageComponent
All pages **and** components that need common UI logic must inherit from `BasePageComponent` (not `ComponentBase`). It provides:
- `GetRelationshipColor(RelationshipType)` → `ThemeColor` — **use this everywhere**, do not invent new color mappings
- `DeleteEntity(name, action)` — deletion with toast notification
- `OpenInNewTab(url)` — JS interop helper
- `NotificationService` (Blazor.Sonner toasts) and `JsRuntime` already injected

### RelationshipType → ThemeColor mapping
Always use `GetRelationshipColor()` from `BasePageComponent`. The canonical mapping is:
| RelationshipType | ThemeColor |
|---|---|
| Family | Warning |
| Friend | Success |
| Mate | Secondary |
| Coworker | Primary |
| Unknown | Default |

This mapping is used for avatars, chips, chart colours, and everything else — keep it consistent.

### HTTP client
Use `HttpClientFactory.CreateClient("ApiClient")` — the named client is pre-configured with the API base URL.
