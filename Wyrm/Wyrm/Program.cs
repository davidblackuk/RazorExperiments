using System.Diagnostics;
using Wyrm.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wyrm.Components;
using Wyrm.Components.Account;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// Registered as a factory (not AddDbContext) because Blazor Server components live for the
// lifetime of the SignalR circuit, not a single request, and need to create short-lived contexts
// per operation. The static-SSR Identity components still get a conventional scoped ApplicationDbContext,
// sourced from the same factory below, so DbContextOptions is only ever built once (avoids a singleton
// factory trying to consume a separately-registered scoped DbContextOptions).
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<ApplicationDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// AddIdentityCore (not AddDefaultIdentity) because AddDefaultIdentity is a Microsoft.AspNetCore.Identity.UI
// convenience wrapper for the Identity.UI Razor Pages scaffold, which this app no longer uses - the cookie
// scheme and email sender it registered implicitly are now registered explicitly below.
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();
builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<IEmailSender<IdentityUser>, IdentityNoOpEmailSender>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorBootstrap();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapGet("/Error", (HttpContext context) =>
{
    context.Response.Headers.CacheControl = "no-store,no-cache";
    context.Response.Headers.Pragma = "no-cache";
    context.Response.Headers.Expires = "-1";

    var requestId = Activity.Current?.Id ?? context.TraceIdentifier;
    var requestIdHtml = string.IsNullOrEmpty(requestId)
        ? ""
        : $"<p><strong>Request ID:</strong> <code>{requestId}</code></p>";

    var html = $"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="utf-8" />
            <title>Error - Wyrm</title>
            <link rel="stylesheet" href="/lib/bootstrap/dist/css/bootstrap.min.css" />
            <link rel="stylesheet" href="/css/site.css" />
        </head>
        <body>
            <div class="container">
                <h1 class="text-danger">Error.</h1>
                <h2 class="text-danger">An error occurred while processing your request.</h2>
                {requestIdHtml}
                <h3>Development Mode</h3>
                <p>
                    Swapping to the <strong>Development</strong> environment displays detailed information about the error that occurred.
                </p>
                <p>
                    <strong>The Development environment shouldn't be enabled for deployed applications.</strong>
                    It can result in displaying sensitive information from exceptions to end users.
                    For local debugging, enable the <strong>Development</strong> environment by setting the <strong>ASPNETCORE_ENVIRONMENT</strong> environment variable to <strong>Development</strong>
                    and restarting the app.
                </p>
            </div>
        </body>
        </html>
        """;

    return Results.Content(html, "text/html");
});

app.MapAdditionalIdentityEndpoints();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
