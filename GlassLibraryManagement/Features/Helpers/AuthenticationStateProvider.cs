using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using GlassLibraryManagement.Features.Data.Models;

namespace GlassLibraryManagement.Features.Helpers
{
    public class AppAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public User? CurrentUser { get; private set; }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult(new AuthenticationState(_currentUser));

        public void MarkUserAsAuthenticated(User user)
        {
            CurrentUser = user;
            var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ], "AppAuth");

            _currentUser = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void MarkUserAsLoggedOut()
        {
            CurrentUser = null;
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

    public class RedirectToLogin : ComponentBase
    {
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            if (!(authState.User.Identity?.IsAuthenticated ?? false))
            {
                Navigation.NavigateTo("/login", forceLoad: false);
            }
        }
    }

    public class AuthorizedComponentBase : RedirectToLogin
    {
        protected bool IsAuthenticated { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            var state = await AuthStateProvider.GetAuthenticationStateAsync();
            IsAuthenticated = state.User.Identity?.IsAuthenticated == true;

            if (!IsAuthenticated)
            {
                Navigation.NavigateTo("/login", forceLoad: false);
                return;
            }

            await LoadDataAsync();
        }

        protected virtual Task LoadDataAsync() => Task.CompletedTask;
    }
}
