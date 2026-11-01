using Microsoft.AspNetCore.Components;
using StockSim.Frontend.Services;

namespace StockSim.Frontend.Components {
    public partial class Navbar {
        [Inject] private CustomAuthenticationStateProvider AuthProvider { get; set; } = default!;

        private async Task HandleLogout() {
            // Drop the stored tokens first so the session cannot be refreshed back to life.
            await TokenService.DeleteTokensAsync();
            // Clear the in-memory session and route client-side, avoiding a full reload flash.
            AuthProvider.NotifySignedOut();
            NavigationManager.NavigateTo("/login");
        }
    }
}
