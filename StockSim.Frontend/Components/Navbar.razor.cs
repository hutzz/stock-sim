using Microsoft.AspNetCore.Components;
using StockSim.Frontend.Services;

namespace StockSim.Frontend.Components {
    public partial class Navbar {
        private async Task HandleLogout() {
            // Drop the stored tokens first so the session cannot be refreshed back to life.
            await TokenService.DeleteTokensAsync();
            // A full reload restarts the app, which rebuilds the auth state from (now empty) storage.
            NavigationManager.NavigateTo("/login", true);
        }
    }
}
