using Microsoft.JSInterop;
using LibraryShopManagement.Features.Services.Interfaces;

namespace LibraryShopManagement.Features.Services.Implementations
{
    /// <summary>
    /// Scoped (per-user-circuit) theme service.
    /// Persists Dark/Light preference to browser localStorage.
    /// Non-static — injected via DI, never shared across users.
    /// </summary>
    public class ThemeService : IThemeService
    {
        private readonly IJSRuntime _js;
        private const string StorageKey = "lsm_theme";

        public bool IsDarkMode { get; private set; } = false;

        public event Action? OnThemeChanged;

        public ThemeService(IJSRuntime js)
        {
            _js = js;
        }

        /// <summary>
        /// Call once in OnAfterRenderAsync(firstRender) — reads localStorage.
        /// Must be after render because JSInterop requires an active circuit.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                var stored = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
                IsDarkMode = stored == "dark";
                await ApplyHtmlAttributeAsync();
            }
            catch
            {
                // JS not available yet — safe default (light)
                IsDarkMode = false;
            }
        }

        /// <summary>
        /// Toggles between dark and light, saves preference, notifies subscribers.
        /// </summary>
        public async Task ToggleThemeAsync()
        {
            IsDarkMode = !IsDarkMode;
            try
            {
                await _js.InvokeVoidAsync("localStorage.setItem", StorageKey,
                    IsDarkMode ? "dark" : "light");
                await ApplyHtmlAttributeAsync();
            }
            catch { /* circuit not ready */ }

            OnThemeChanged?.Invoke();
        }

        /// Sets data-theme="dark"|"light" on <html> so CSS :root variables respond
        private async Task ApplyHtmlAttributeAsync()
        {
            try
            {
                await _js.InvokeVoidAsync("document.documentElement.setAttribute",
                    "data-theme", IsDarkMode ? "dark" : "light");
            }
            catch { }
        }
    }
}
