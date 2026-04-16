using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LibraryShopManagement.Features.Helpers
{
    public class AuthorizedComponentBase : ComponentBase
    {
        [Inject] protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;

        protected bool IsAuthenticated { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            // Safe to call here because prerender is disabled —
            // this only runs after the SignalR circuit is established,
            // so in-memory auth state is available.
            var state = await AuthStateProvider.GetAuthenticationStateAsync();
            IsAuthenticated = state.User.Identity?.IsAuthenticated == true;

            if (!IsAuthenticated)
            {
                Navigation.NavigateTo("/login");
                return;
            }

            await LoadDataAsync();
        }

        protected virtual Task LoadDataAsync() => Task.CompletedTask;
    }
}
