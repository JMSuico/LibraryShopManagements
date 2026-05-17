using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using GlassLibraryManagement.Features.Data;
using GlassLibraryManagement.Features.Helpers;
using GlassLibraryManagement.Features.Repositories.Implementations;
using GlassLibraryManagement.Features.Repositories.Interfaces;
using GlassLibraryManagement.Features.Services.Implementations;
using GlassLibraryManagement.Features.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString,
        new MySqlServerVersion(new Version(8, 0, 36))), ServiceLifetime.Transient);

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IBookRepository, BookRepository>();
builder.Services.AddTransient<IBorrowRepository, BorrowRepository>();
builder.Services.AddTransient<IReturnRepository, ReturnRepository>();
builder.Services.AddTransient<IBorrowAndReturnRepository, BorrowAndReturnRepository>();
builder.Services.AddTransient<IReservationRepository, ReservationRepository>();

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IBookService, BookService>();
builder.Services.AddTransient<IBorrowService, BorrowService>();
builder.Services.AddTransient<IReturnService, ReturnService>();
builder.Services.AddTransient<IBorrowAndReturnService, BorrowAndReturnService>();
builder.Services.AddTransient<IReservationService, ReservationService>();
builder.Services.AddTransient<IThemeService, ThemeService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, AppAuthenticationStateProvider>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<GlassLibraryManagement.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
