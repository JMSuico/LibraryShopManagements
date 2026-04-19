using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using LibraryShopManagement.Features.Data;
using LibraryShopManagement.Features.Helpers;
using LibraryShopManagement.Features.Repositories.Implementations;
using LibraryShopManagement.Features.Repositories.Interfaces;
using LibraryShopManagement.Features.Services.Implementations;
using LibraryShopManagement.Features.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Blazor Server ──────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── MudBlazor ──────────────────────────────────────────────────────────────
builder.Services.AddMudServices();

// ── EF Core + Pomelo MySQL (XAMPP) ─────────────────────────────────────────
// Hardcoded version avoids AutoDetect connecting to MySQL at startup
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString,
        new MySqlServerVersion(new Version(8, 0, 36))));

// ── Repositories ───────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

// ── Services ───────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IThemeService, ThemeService>();

// ── Authentication + Authorization ─────────────────────────────────────────
// Cookie scheme registered so [Authorize] + AuthorizeRouteView work correctly.
// Actual auth state is managed by CustomAuthenticationStateProvider (in-memory).
// The cookie middleware is transparent — our Blazor auth state provider controls
// who is logged in, independent of HTTP cookies.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// ── Build ──────────────────────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Authentication → Authorization must be in this order
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<LibraryShopManagement.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
