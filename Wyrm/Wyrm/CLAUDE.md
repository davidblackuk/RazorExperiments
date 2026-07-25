# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

Wyrm is an ASP.NET Core (.NET 10) application for designing and browsing a schema of "repositories" containing "object types," which in turn contain "property types." The schema-design UI is Razor Pages; the browsing/data-entry UI ("Explorer") is a Blazor Web App (Interactive Server render mode) living alongside the Razor Pages in the same project. It uses ASP.NET Core Identity for auth and EF Core with SQLite for persistence. There is no separate solution-wide test project.

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

### Pages vs. Areas vs. Components — three UI surfaces over the same data

- **`Pages/`** — the "Designer" CRUD UI for building the schema: `Pages/Designer/` (Repositories), `Pages/ObjectTypes/`, `Pages/PropertyTypes/`. These are full CRUD (Create/Edit/Delete/Details/Index) Razor Pages under the default (non-area) root, and are authorization-locked via `options.Conventions.AuthorizeFolder("/")` in `Program.cs`.
- **`Areas/`** — role-scoped surfaces, each an ASP.NET Core Area with its own `Pages/`, `_ViewImports.cshtml`, `_ViewStart.cshtml`:
  - `Areas/Publisher/` and `Areas/Reader/` — scaffolded placeholder areas (empty `OnGet()`), intended for future publish/consume workflows
  - `Areas/Identity/` — ASP.NET Core Identity's scaffolded account pages (login, register, password reset, email confirmation)
- **`Components/`** — the "Explorer" browsing/data-entry UI (`/Explorer` route), built as Blazor components rather than Razor Pages. This exists specifically because navigating between separate pages for tree/grid/detail/CRUD lost UI state (selected tree node, scroll position, splitter sizes) on every operation; Blazor's Interactive Server render mode keeps all of that as component state across a live SignalR circuit instead. Structure:
  - `Components/App.razor`, `Routes.razor`, `_Imports.razor` — Blazor Web App root scaffolding (registered in `Program.cs` via `AddRazorComponents().AddInteractiveServerComponents()` / `MapRazorComponents<App>().AddInteractiveServerRenderMode()`, alongside the existing `MapRazorPages()`)
  - `Components/Layout/MainLayout.razor` — Blazor's layout, styled to match `Pages/Shared/_Layout.cshtml` (same nav bar, Bootstrap/site.css/glass.css)
  - `Components/Pages/Explorer.razor` — the page component; holds all selection/loading state and orchestrates the child components below via `IDbContextFactory<ApplicationDbContext>` (not a directly-injected scoped `ApplicationDbContext` — a circuit-scoped context would accumulate tracked entities across the whole session, so each operation creates its own short-lived context)
  - `Components/Explorer/` — `RepositoryTree.razor`, `InstanceGrid.razor`, `InstanceDetail.razor`, `InstanceFormModal.razor` (Create + Edit), `DeleteConfirmModal.razor`, `PropertyFieldEditor.razor`
  - Authorization is per-component via `@attribute [Authorize]` (there's no area-folder convention for Blazor routes the way `AuthorizeAreaFolder` works for Razor Pages)
  - Splitter drag-resize is the one piece of genuinely imperative DOM work, so it stays as plain JS (`wwwroot/js/explorer-splitters.js`) invoked via `IJSRuntime` from `OnAfterRenderAsync`, rather than being reimplemented in C#

When adding a new page for the schema-design workflow, put it in `Pages/`; when extending the Explorer browsing/data-entry experience, add a component under `Components/Explorer/` and wire it into `Components/Pages/Explorer.razor`.

### Auth

Identity is wired up in `Program.cs` with `RequireConfirmedAccount = true` and every page under `/` is authorized by default via `AuthorizeFolder("/")`. There's no `AllowAnonymousToPage` currently active (the call is commented out in `Program.cs`), so any new anonymous-facing page needs that convention added explicitly.

### Front-end

Designer (`Pages/`) is server-rendered Razor Pages with Bootstrap (vendored under `wwwroot/lib/bootstrap`) and jQuery + jquery-validation (vendored under `wwwroot/lib/`) for client-side validation. Explorer (`Components/`) is Blazor Interactive Server — no jquery-validation there, field validation happens in `InstanceFormModal.razor` via `Services/PropertyValueParser.cs`. Both share the same Bootstrap/Font Awesome styling and `wwwroot/css/site.css`/`glass.css`; Explorer additionally uses `wwwroot/css/explorer.css`. Font Awesome icon classes are used for iconography in the Designer area — see `Notes.md` for the icon-to-entity mapping convention (e.g. `fa-database` for Repository, `fa-cube` for Object types, `fa-diagram-project` for property types) to keep icon usage consistent when adding new Designer pages.
