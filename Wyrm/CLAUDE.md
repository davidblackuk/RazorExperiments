# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

Wyrm is an ASP.NET Core (.NET 10) application for designing and browsing a schema of "repositories" containing "object types," which in turn contain "property types." The entire UI is Blazor Web App components — the schema-design UI ("Designer"), the browsing/data-entry UI ("Explorer"), the home page, and ASP.NET Core Identity's auth UI (login, register, password reset, email confirmation) are all Blazor components; there are no Razor Pages or MVC views anywhere in the app. Designer and Explorer use Interactive Server render mode and BlazorBootstrap; the auth components render statically (no `@rendermode`) since sign-in/out need to write authentication cookies, which requires running before a SignalR circuit exists. It uses ASP.NET Core Identity for auth and EF Core with SQLite for persistence. There is no separate solution-wide test project.

The repo root is `Wyrm/` (the `.slnx` solution file, this `CLAUDE.md`, and `.vscode/` all live here), containing three projects:

- **`Wyrm.DAL/`** — class library: domain models, `ApplicationDbContext`, and EF Core migrations. Namespaces `Wyrm.Models`, `Wyrm.Data`, `Wyrm.Abstractions`.
- **`Wyrm.Services/`** — class library: stateless service helpers used by the UI. Namespace `Wyrm.Services`. Project-references `Wyrm.DAL`.
- **`Wyrm/`** — the ASP.NET Core web project (`Wyrm.csproj`): `Program.cs`, Blazor `Components/`, `ViewModels/`, `wwwroot/`. Project-references both `Wyrm.DAL` and `Wyrm.Services`.

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

### Components — all UI surfaces live here

Everything is a Blazor component under `Components/`; there is no `Pages/` or `Areas/` directory. Structure:

  - `Components/App.razor`, `Routes.razor`, `_Imports.razor` — Blazor Web App root scaffolding (registered in `Program.cs` via `AddRazorComponents().AddInteractiveServerComponents()` / `MapRazorComponents<App>().AddInteractiveServerRenderMode()`, alongside `AddBlazorBootstrap()`). `Routes.razor`'s `AuthorizeRouteView` supplies a `NotAuthorized` template that renders `<RedirectToLogin />` for unauthenticated users (vs. a plain "not authorized" message for authenticated-but-forbidden ones), so hitting any `[Authorize]` route while signed out redirects to `/Account/Login?ReturnUrl=...` instead of showing blank text — this is what replaces `AuthorizeFolder("/")`'s automatic challenge-redirect from the old Razor Pages pipeline.
  - `Components/Layout/MainLayout.razor` — the app's only layout, with the Designer/Explorer nav and a `<LoginDisplay />` component (register/login links when signed out, a greeting + logout form when signed in)
  - `Components/Pages/Home.razor` — the `/` route (former `Pages/Index.cshtml`), `[Authorize]`, no code-behind
  - `Components/Pages/Designer.razor` and `Components/Pages/Explorer.razor` — the schema-design (`/Designer`) and browsing/data-entry (`/Explorer`) page components; each holds all selection/loading state and orchestrates its child components via `IDbContextFactory<ApplicationDbContext>` (not a directly-injected scoped `ApplicationDbContext` — a circuit-scoped context would accumulate tracked entities across the whole session, so each operation creates its own short-lived context). Both use the same three-pane shell pattern (tree sidebar + top/bottom split main pane, via the generic `.explorer-shell`/`.explorer-sidebar`/`.explorer-main`/`.explorer-top`/`.explorer-bottom` CSS classes in `wwwroot/css/explorer.css` — the class names predate Designer's conversion but are generic layout primitives, not Explorer-specific) and BlazorBootstrap components (`Modal`, `Button`, `Grid`, `Alert`, `ConfirmDialog`) for chrome.
  - `Components/Designer/` — `SchemaTree.razor` (Repository/ObjectType tree; each node carries its own inline Add/Edit/Delete `Button`s rather than a shared toolbar), `DesignerToolbar.razor` (Add Repository only — repository/object type delete are per-node, not toolbar-level), `PropertyTypeGrid.razor`, `PropertyTypeDetail.razor`, `ObjectTypeFormModal.razor`, `PropertyTypeFormModal.razor`
  - `Components/Explorer/` — `RepositoryTree.razor`, `InstanceGrid.razor`, `InstanceDetail.razor`, `InstanceFormModal.razor` (Create + Edit), `PropertyFieldEditor.razor`
  - `Components/Shared/` — `RepositoryFormModal.razor`, used by both Designer (Add + Edit, via its `ShowAsync(Repository)` overload) and Explorer (Add only)
  - `Components/Account/` — the auth UI (see below)
  - Designer/Explorer both use a single `ConfirmDialog` (BlazorBootstrap) for all delete confirmations instead of a bespoke modal per delete flow — `await _confirmDialog.ShowAsync(...)` returns the yes/no result directly, so there's no separate show/cancel/confirm state to track per entity
  - Authorization is per-component via `@attribute [Authorize]`/`@attribute [AllowAnonymous]` — there's no folder-convention equivalent for Blazor routes
  - Splitter drag-resize is the one piece of genuinely imperative DOM work, so it stays as plain JS (`wwwroot/js/explorer-splitters.js`) invoked via `IJSRuntime` from `OnAfterRenderAsync`, rather than being reimplemented in C#. Both `Designer.razor` and `Explorer.razor` call `explorerSplitters.init(".explorer-shell")` — the script is generic over that selector, not page-specific despite the name.

`Repository`→`ObjectType` is `DeleteBehavior.Restrict` in `ApplicationDbContext`, so repository delete is blocked in the UI (with an inline error) while it still has object types. `ObjectType`→`PropertyType`/`ObjectInstance` and `PropertyType`→`PropertyValue*` are `DeleteBehavior.Cascade`, so object type and property type delete are always allowed but the confirm dialog warns about what cascades.

When extending the Designer schema-design experience, add a component under `Components/Designer/` and wire it into `Components/Pages/Designer.razor`; when extending the Explorer browsing/data-entry experience, do the same under `Components/Explorer/` and `Components/Pages/Explorer.razor`.

### Auth

Identity is registered via `AddIdentityCore<IdentityUser>` (not `AddDefaultIdentity` — that's a `Microsoft.AspNetCore.Identity.UI` convenience wrapper for the Razor Pages scaffold this app no longer has) plus explicit `AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies()`, with `RequireConfirmedAccount = true`. `app.UseAuthentication()` is required in `Program.cs` (unlike the old `AddDefaultIdentity` setup, `AddIdentityCore` doesn't wire it in implicitly). There's no `AllowAnonymousToPage`-style convention for Blazor routes — every routable component defaults to requiring auth only if it wins routing via `AuthorizeRouteView` and carries `@attribute [Authorize]`, so any new anonymous-facing page needs `@attribute [AllowAnonymous]` added explicitly (see `Components/Account/Pages/Login.razor`, `Register.razor` for the pattern — auth pages themselves have neither attribute since Identity's own sign-in/registration flow is inherently anonymous-reachable).

`Components/Account/` holds the whole auth UI, following the same shape as Microsoft's `dotnet new blazor -au Individual` template:
  - `Components/Account/Pages/` — `Login`, `Register`, `ForgotPassword`(+`Confirmation`), `ResetPassword`(+`Confirmation`), `ConfirmEmail`, `ResendEmailConfirmation`, `RegisterConfirmation`, each a `.razor`/`.razor.cs` pair routed under `/Account/...`. None declare `@rendermode` — they render statically (no SignalR circuit) because `SignInManager`/`UserManager` calls need to write authentication cookies, which requires mutating the HTTP response before it starts streaming; that's impossible once an Interactive Server circuit takes over. Forms use `EditForm` + `[SupplyParameterFromForm]`-bound `InputModel`s (not the scaffolded `PageModel.InputModel` pattern) with `OnValidSubmit` handlers that run during the static POST. `EditForm` already emits its own antiforgery hidden field when antiforgery is enabled — don't add a second `<AntiforgeryToken />` inside an `EditForm` (only the plain, non-`EditForm` `<form>` in `LoginDisplay.razor`'s logout button needs one explicitly).
  - `Components/Account/Shared/AuthLayout.razor` — the glass-card layout (`@layout AuthLayout` on each auth page), replacing the old `_authLayout.cshtml`/`auth.css` Razor Pages layout; `auth.css` itself is unchanged, just now linked globally from `Components/App.razor`'s `<head>` instead of a page-specific Razor layout.
  - `Components/Account/Shared/LoginDisplay.razor` — the navbar auth widget wired into `MainLayout.razor`, replacing `_LoginPartial.cshtml`. Logout is a plain HTML `<form method="post" action="/Account/Logout">` (not an `EditForm`), because signing out also needs to write to the response outside a circuit.
  - `Components/Account/Shared/RedirectToLogin.razor` — used by `Routes.razor`'s `AuthorizeRouteView.NotAuthorized` template (see above).
  - `Components/Account/IdentityRedirectManager.cs` — a small `NavigationManager`-based helper (`RedirectTo(uri)` / `RedirectTo(uri, queryParameters)`) that throws to trigger a real HTTP redirect from within a static-SSR component; used everywhere a page needs to redirect after a form post (e.g. Register → RegisterConfirmation).
  - `Components/Account/IdentityNoOpEmailSender.cs` — a no-op `IEmailSender<IdentityUser>` (registered explicitly in `Program.cs`, since removing `Microsoft.AspNetCore.Identity.UI` also removes its implicit default sender). No email is ever actually sent, matching the app's pre-conversion behavior; `RegisterConfirmation.razor` compensates the same way the old scaffold did, by generating and displaying the confirmation link directly on the page.
  - `Components/Account/IdentityComponentsEndpointRouteBuilderExtensions.cs` — `MapAdditionalIdentityEndpoints()`, a minimal API extension mapping `POST /Account/Logout`. Logout has no routable component at all (only `LoginDisplay`'s form posts to it) since it's pure sign-out-and-redirect with no UI of its own.
  - There's deliberately no custom `AuthenticationStateProvider` — the default one from `AddCascadingAuthenticationState()` (already registered) correctly flows `HttpContext.User` into both static-SSR auth pages and the Designer/Explorer interactive circuit via the existing `[CascadingParameter] Task<AuthenticationState>` pattern; adding a revalidating/persisting provider (as the standard MS template does) would be new, unrequested functionality (periodic security-stamp revalidation), not something this conversion needed for parity.

### Front-end

Designer and Explorer are Blazor Interactive Server, styled with BlazorBootstrap (`Blazor.Bootstrap` NuGet package, registered via `AddBlazorBootstrap()` in `Program.cs`) on top of the same vendored Bootstrap (`wwwroot/lib/bootstrap`) and Font Awesome used elsewhere — field validation happens in component code (e.g. `InstanceFormModal.razor` via `Wyrm.Services/PropertyValueParser.cs`, or inline `Name is required` checks in the Designer form modals). The auth components (`Components/Account/Pages/`) use `EditForm` + `DataAnnotationsValidator`/`ValidationMessage` for validation — server-side only, so validation feedback appears after a full-page form POST rather than live as-you-type (the jquery-validation plugin that the old scaffolded Identity pages used is gone along with them; the vendored jQuery/Bootstrap JS still loaded from `App.razor` predates this conversion and is unrelated). All surfaces share `wwwroot/css/site.css`/`glass.css`; Explorer additionally uses `wwwroot/css/explorer.css`, Designer additionally uses `wwwroot/css/designer.css` (tree node layout with per-node action buttons — distinct from Explorer's plain-text tree nodes), and the auth components additionally use `wwwroot/css/auth.css` (glass-card layout). Font Awesome icon classes are used for iconography — see `Notes.md` for the icon-to-entity mapping convention (e.g. `fa-database` for Repository, `fa-cube` for Object types, `fa-diagram-project` for property types) to keep icon usage consistent when adding new schema-design UI.
