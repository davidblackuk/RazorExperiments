# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

Wyrm is an ASP.NET Core (.NET 10) application for designing and browsing a schema of "repositories" containing "object types," which in turn contain "property types." Both the schema-design UI ("Designer") and the browsing/data-entry UI ("Explorer") are Blazor Web App components (Interactive Server render mode) built with BlazorBootstrap, living alongside a small remaining set of Razor Pages (home page, ASP.NET Core Identity's scaffolded account pages). It uses ASP.NET Core Identity for auth and EF Core with SQLite for persistence. There is no separate solution-wide test project.

The repo root is `Wyrm/` (the `.slnx` solution file, this `CLAUDE.md`, and `.vscode/` all live here), containing three projects:

- **`Wyrm.DAL/`** — class library: domain models, `ApplicationDbContext`, and EF Core migrations. Namespaces `Wyrm.Models`, `Wyrm.Data`, `Wyrm.Abstractions`.
- **`Wyrm.Services/`** — class library: stateless service helpers used by the UI. Namespace `Wyrm.Services`. Project-references `Wyrm.DAL`.
- **`Wyrm/`** — the ASP.NET Core web project (`Wyrm.csproj`): `Program.cs`, Blazor `Components/`, Razor `Pages/`, `Areas/Identity/`, `ViewModels/`, `wwwroot/`. Project-references both `Wyrm.DAL` and `Wyrm.Services`.

All three projects share the same root namespace (`Wyrm`) via an explicit `<RootNamespace>Wyrm</RootNamespace>` in `Wyrm.DAL.csproj`/`Wyrm.Services.csproj`, so folder-based sub-namespaces (`Wyrm.Models`, `Wyrm.Data`, `Wyrm.Services`, `Wyrm.Abstractions`) are identical to what they'd be in a single-project layout — code moved between these projects doesn't need its `namespace`/`using` statements touched, only its project.

## Commands

Run all commands from `Wyrm/` (the solution root, containing `Wyrm.slnx`).

- Restore/build: `dotnet build Wyrm.slnx`
- Run the app: `dotnet run --project Wyrm/Wyrm.csproj` (serves on `http://localhost:5114` and `https://localhost:7184`, per `Wyrm/Properties/launchSettings.json`)
- Apply/update the database: `dotnet ef database update --project Wyrm.DAL/Wyrm.DAL.csproj --startup-project Wyrm/Wyrm.csproj` (requires `dotnet-ef` tool; uses the `DefaultConnection` string in `Wyrm/appsettings.json`, a SQLite file `Wyrm.db` created in `Wyrm/`)
- Add a new migration after model changes: `dotnet ef migrations add <Name> --project Wyrm.DAL/Wyrm.DAL.csproj --startup-project Wyrm/Wyrm.csproj`
- `--project` points at `Wyrm.DAL` because that's where `ApplicationDbContext` and `Data/Migrations/` live; `--startup-project` points at `Wyrm` because that's where the DI container (connection string, `UseSqlite`) is configured, in `Program.cs`.
- There are no automated tests in this repo currently.

## Architecture

### Domain model (hierarchical schema designer)

The core domain is a three-level hierarchy defined in `Wyrm.DAL/Models/`:

- `Repository` — top-level container (`Models/Repository.cs`)
- `ObjectType` — belongs to a `Repository`, defines a "type of object" (`Models/ObjectType.cs`)
- `PropertyType` — belongs to an `ObjectType`, defines an attribute/field with a `PropertyDataType` (`Models/PropertyType.cs`, `Models/DataType.cs`: String, Memo, Int, Number, DateTime, Date)

All three entities share the same audit pattern: `CreatedById`/`CreatedAt`/`UpdatedById`/`UpdatedAt` plus `CreatedBy`/`UpdatedBy` navigation properties to `IdentityUser`, and implement the marker interface `Wyrm.DAL/Abstractions/IAuditModifications.cs`. When adding CRUD for a new entity, follow this same audit pattern (see `Components/Pages/Designer.razor`'s `SaveRepositoryAsync`/`SaveObjectTypeAsync`/`SavePropertyTypeAsync` for the canonical example, in the `Wyrm` web project: resolve the current user id via a cascading `AuthenticationState`'s `ClaimTypes.NameIdentifier`, and stamp audit fields server-side before `SaveChangesAsync`).

Every new `ObjectType` is auto-seeded with a fixed set of system `PropertyType`s (`IsSystemProperty = true`) built by `Wyrm.Services/ObjectTypeSystemProperties.cs`: `Name`, `Description`, `Category`, plus four "audit mirror" properties (`Who Created`, `When Created`, `Who Updated`, `When Updated` — named in `Wyrm.Services/SystemPropertyNames.cs`) whose values are stamped automatically from the owning `ObjectInstance`'s own audit fields rather than user-entered. `SystemPropertyNames.IsAuditMirror(name)` is what both Designer (hiding Edit/Delete for system properties) and Explorer (excluding them from the instance edit form) key off of.

EF Core relationships and delete behavior are configured in `Wyrm.DAL/Data/ApplicationDbContext.cs` (`OnModelCreating`) — note the mix of `DeleteBehavior.Restrict`/`NoAction` on audit FKs (to avoid multiple cascade paths through `IdentityUser`) and `DeleteBehavior.Cascade` from `ObjectType` down to `PropertyType`. Migrations live in `Wyrm.DAL/Data/Migrations/`.

### Pages vs. Areas vs. Components — UI surfaces over the same data

- **`Pages/`** — now just the home page (`Pages/Index.cshtml`) and Razor Pages scaffolding (`_Layout.cshtml`, `_ViewImports.cshtml`, etc.). Authorization-locked via `options.Conventions.AuthorizeFolder("/")` in `Program.cs`. The schema-design CRUD that used to live here (`Pages/Designer/`, `Pages/ObjectTypes/`, `Pages/PropertyTypes/`) has been removed in favor of the Blazor Designer described below.
- **`Areas/`** — `Areas/Identity/` only: ASP.NET Core Identity's scaffolded account pages (login, register, password reset, email confirmation). (The `Publisher`/`Reader` placeholder areas that used to live here were removed as unused scaffolding.)
- **`Components/`** — both the "Designer" (schema CRUD, `/Designer` route) and "Explorer" (browsing/data-entry, `/Explorer` route) UIs, built as Blazor components rather than Razor Pages. This exists specifically because navigating between separate pages for tree/grid/detail/CRUD lost UI state (selected tree node, scroll position, splitter sizes) on every operation; Blazor's Interactive Server render mode keeps all of that as component state across a live SignalR circuit instead. Both UIs share the same three-pane shell pattern (tree sidebar + top/bottom split main pane, via the generic `.explorer-shell`/`.explorer-sidebar`/`.explorer-main`/`.explorer-top`/`.explorer-bottom` CSS classes in `wwwroot/css/explorer.css` — the class names predate Designer's conversion but are generic layout primitives, not Explorer-specific) and BlazorBootstrap components (`Modal`, `Button`, `Grid`, `Alert`, `ConfirmDialog`) for chrome. Structure:
  - `Components/App.razor`, `Routes.razor`, `_Imports.razor` — Blazor Web App root scaffolding (registered in `Program.cs` via `AddRazorComponents().AddInteractiveServerComponents()` / `MapRazorComponents<App>().AddInteractiveServerRenderMode()`, alongside `AddBlazorBootstrap()` and the remaining `MapRazorPages()`)
  - `Components/Layout/MainLayout.razor` — Blazor's layout, styled to match `Pages/Shared/_Layout.cshtml` (same nav bar, Bootstrap/site.css/glass.css)
  - `Components/Pages/Designer.razor` and `Components/Pages/Explorer.razor` — the page components; each holds all selection/loading state and orchestrates its child components via `IDbContextFactory<ApplicationDbContext>` (not a directly-injected scoped `ApplicationDbContext` — a circuit-scoped context would accumulate tracked entities across the whole session, so each operation creates its own short-lived context)
  - `Components/Designer/` — `SchemaTree.razor` (Repository/ObjectType tree; each node carries its own inline Add/Edit/Delete `Button`s rather than a shared toolbar), `DesignerToolbar.razor` (Add Repository only — repository/object type delete are per-node, not toolbar-level), `PropertyTypeGrid.razor`, `PropertyTypeDetail.razor`, `ObjectTypeFormModal.razor`, `PropertyTypeFormModal.razor`
  - `Components/Explorer/` — `RepositoryTree.razor`, `InstanceGrid.razor`, `InstanceDetail.razor`, `InstanceFormModal.razor` (Create + Edit), `PropertyFieldEditor.razor`
  - `Components/Shared/` — `RepositoryFormModal.razor`, used by both Designer (Add + Edit, via its `ShowAsync(Repository)` overload) and Explorer (Add only)
  - Both pages use a single `ConfirmDialog` (BlazorBootstrap) for all delete confirmations instead of a bespoke modal per delete flow — `await _confirmDialog.ShowAsync(...)` returns the yes/no result directly, so there's no separate show/cancel/confirm state to track per entity
  - Authorization is per-component via `@attribute [Authorize]` (there's no area-folder convention for Blazor routes the way `AuthorizeAreaFolder` works for Razor Pages)
  - Splitter drag-resize is the one piece of genuinely imperative DOM work, so it stays as plain JS (`wwwroot/js/explorer-splitters.js`) invoked via `IJSRuntime` from `OnAfterRenderAsync`, rather than being reimplemented in C#. Both `Designer.razor` and `Explorer.razor` call `explorerSplitters.init(".explorer-shell")` — the script is generic over that selector, not page-specific despite the name.

`Repository`→`ObjectType` is `DeleteBehavior.Restrict` in `ApplicationDbContext`, so repository delete is blocked in the UI (with an inline error) while it still has object types. `ObjectType`→`PropertyType`/`ObjectInstance` and `PropertyType`→`PropertyValue*` are `DeleteBehavior.Cascade`, so object type and property type delete are always allowed but the confirm dialog warns about what cascades.

When extending the Designer schema-design experience, add a component under `Components/Designer/` and wire it into `Components/Pages/Designer.razor`; when extending the Explorer browsing/data-entry experience, do the same under `Components/Explorer/` and `Components/Pages/Explorer.razor`.

### Auth

Identity is wired up in `Program.cs` with `RequireConfirmedAccount = true` and every page under `/` is authorized by default via `AuthorizeFolder("/")`. There's no `AllowAnonymousToPage` currently active (the call is commented out in `Program.cs`), so any new anonymous-facing page needs that convention added explicitly.

### Front-end

Designer and Explorer (both `Components/`) are Blazor Interactive Server, styled with BlazorBootstrap (`Blazor.Bootstrap` NuGet package, registered via `AddBlazorBootstrap()` in `Program.cs`) on top of the same vendored Bootstrap (`wwwroot/lib/bootstrap`) and Font Awesome used elsewhere — no jquery-validation in either; field validation happens in component code (e.g. `InstanceFormModal.razor` via `Wyrm.Services/PropertyValueParser.cs`, or inline `Name is required` checks in the Designer form modals). The remaining Razor Pages surface (home page, `Areas/Identity/`) still uses jQuery + jquery-validation (vendored under `wwwroot/lib/`) for client-side validation. All surfaces share `wwwroot/css/site.css`/`glass.css`; Explorer additionally uses `wwwroot/css/explorer.css` and Designer additionally uses `wwwroot/css/designer.css` (tree node layout with per-node action buttons — distinct from Explorer's plain-text tree nodes). Font Awesome icon classes are used for iconography — see `Notes.md` for the icon-to-entity mapping convention (e.g. `fa-database` for Repository, `fa-cube` for Object types, `fa-diagram-project` for property types) to keep icon usage consistent when adding new schema-design UI.
