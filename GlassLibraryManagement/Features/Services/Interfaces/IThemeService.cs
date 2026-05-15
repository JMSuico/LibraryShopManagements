namespace GlassLibraryManagement.Features.Services.Interfaces
{
    public interface IThemeService
    {
        bool IsDarkMode { get; }
        event Action OnThemeChanged;
        Task InitializeAsync();
        Task ToggleThemeAsync();
    }
}
