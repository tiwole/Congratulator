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
- **WebAssembly** — Blazor WASM client with components (`AddPersonModal`, `BirthdayCard`, `Pager`), pages (`Home`, `All`)

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
- [ ] Solution builds without errors (use `dotnet build -nowarn`)
- [ ] Unit tests are green (use `dotnet test -nowarn`)
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
- Check if CSS variable exists before creating it in `tokens.css`
- Update `wwwroot/css/tokens.css` with component-specific variables
- **For scrollable elements**: Apply `custom-scrollbar` CSS class to enable styled scrollbars

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
- [ ] Menu item added to NavMenu.razor with correct route and icon

## Security Guidelines

### Data Protection
- Never hardcode credentials, API keys, or sensitive data
- Use configuration providers for sensitive settings
- Validate all input parameters and user data
- Sanitize data before database operations
- Use parameterized queries to prevent SQL injection

### Authentication & Authorization
- Implement proper authentication mechanisms
- Use role-based authorization where appropriate
- Validate user permissions before sensitive operations
- Log security-related events for auditing
