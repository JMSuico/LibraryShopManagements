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

// Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MudBlazor
builder.Services.AddMudServices();

// EF Core + Pomelo MySQL (XAMPP)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookService, BookService>();

// ✅ Blazor Server auth — do NOT add HTTP auth middleware
// [Authorize] on Blazor pages is handled by <AuthorizeRouteView>, not HTTP pipeline
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// ✅ No app.UseAuthentication() or app.UseAuthorization() here
// Those are for MVC/Razor Pages — Blazor Server uses AuthenticationStateProvider instead

app.MapRazorComponents<LibraryShopManagement.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
