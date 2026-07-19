# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

Wyrm is an ASP.NET Core Razor Pages application (.NET 10) for designing and browsing a schema of "repositories" containing "object types," which in turn contain "property types." It uses ASP.NET Core Identity for auth and EF Core with SQLite for persistence. There is no separate solution-wide test project.

The repo root for the app is `Wyrm/Wyrm` (the `.slnx` solution file lives one level up in `Wyrm/`).

## Commands

Run all commands from `Wyrm/Wyrm/` (the project directory containing `Wyrm.csproj`).

- Restore/build: `dotnet build`
- Run the app: `dotnet run` (serves on `http://localhost:5114` and `https://localhost:7184`, per `Properties/launchSettings.json`)
- Apply/update the database: `dotnet ef database update` (requires `dotnet-ef` tool; uses the `DefaultConnection` string in `appsettings.json`, a SQLite file `Wyrm.db` created in the project directory)
- Add a new migration after model changes: `dotnet ef migrations add <Name>`
- There are no automated tests in this repo currently.

## Architecture

### Domain model (hierarchical schema designer)

The core domain is a three-level hierarchy defined in `Models/`:

- `Repository` — top-level container (`Models/Repository.cs`)
- `ObjectType` — belongs to a `Repository`, defines a "type of object" (`Models/ObjectType.cs`)
- `PropertyType` — belongs to an `ObjectType`, defines an attribute/field with a `PropertyDataType` (`Models/PropertyType.cs`, `Models/DataType.cs`: String, Memo, Int, Number, DateTime, Date)

All three entities share the same audit pattern: `CreatedById`/`CreatedAt`/`UpdatedById`/`UpdatedAt` plus `CreatedBy`/`UpdatedBy` navigation properties to `IdentityUser`. When adding CRUD pages for a new entity, follow this same audit pattern and replicate it (see `Pages/Designer/Create.cshtml.cs` for the canonical example: resolve the current user id via `ClaimTypes.NameIdentifier`, stamp audit fields server-side before validation, and `ModelState.Remove(...)` the navigation properties since they aren't form-bound).

EF Core relationships and delete behavior are configured in `Data/ApplicationDbContext.cs` (`OnModelCreating`) — note the mix of `DeleteBehavior.Restrict`/`NoAction` on audit FKs (to avoid multiple cascade paths through `IdentityUser`) and `DeleteBehavior.Cascade` from `ObjectType` down to `PropertyType`. Migrations live in `Data/Migrations/`.

### Pages vs. Areas — two parallel UIs over the same data

There are two separate sets of UI surfaces reading the same EF Core model, serving different personas:

- **`Pages/`** — the "Designer" CRUD UI for building the schema: `Pages/Designer/` (Repositories), `Pages/ObjectTypes/`, `Pages/PropertyTypes/`. These are full CRUD (Create/Edit/Delete/Details/Index) Razor Pages under the default (non-area) root, and are authorization-locked via `options.Conventions.AuthorizeFolder("/")` in `Program.cs`.
- **`Areas/`** — read-oriented/role-oriented surfaces, each an ASP.NET Core Area with its own `Pages/`, `_ViewImports.cshtml`, `_ViewStart.cshtml`:
  - `Areas/Explorer/` — browsing view of repositories/object types (currently read-only index pages)
  - `Areas/Publisher/` and `Areas/Reader/` — scaffolded placeholder areas (empty `OnGet()`), intended for future publish/consume workflows
  - `Areas/Identity/` — ASP.NET Core Identity's scaffolded account pages (login, register, password reset, email confirmation)

When adding a new page for the schema-design workflow, put it in `Pages/`; when adding a new browsing/consumption view, put it in the relevant `Areas/*` folder, matching that area's existing `_ViewImports.cshtml`/layout.

### Auth

Identity is wired up in `Program.cs` with `RequireConfirmedAccount = true` and every page under `/` is authorized by default via `AuthorizeFolder("/")`. There's no `AllowAnonymousToPage` currently active (the call is commented out in `Program.cs`), so any new anonymous-facing page needs that convention added explicitly.

### Front-end

Server-rendered Razor Pages with Bootstrap (vendored under `wwwroot/lib/bootstrap`) and jQuery + jquery-validation (vendored under `wwwroot/lib/`) for client-side validation. Custom styles are in `wwwroot/css/site.css`, `glass.css`, and `auth.css`. Font Awesome icon classes are used for iconography in the Designer area — see `Notes.md` for the icon-to-entity mapping convention (e.g. `fa-database` for Repository, `fa-cube` for Object types, `fa-diagram-project` for property types) to keep icon usage consistent when adding new Designer pages.
