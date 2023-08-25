using StockSim.Frontend.Models;
using Microsoft.AspNetCore.Components;

namespace StockSim.Frontend.Pages {
    public partial class Register {
        // absolute comedy
        // https://stackoverflow.com/questions/64108983/blazor-not-discovering-new-pages-in-subfolders
        private RegisterModel user = new();
        private string message = "";

        private async Task Valid() {
            try {
                var result = await RegisterService.Register(user);
                message = "";
                StateHasChanged();
                RedirectToLogin();
            }
            catch (Exception e) {
                message = e.Message;
            }
        }
        private void Invalid() {
        }
        private void RedirectToLogin() {
            NavigationManager.NavigateTo("/login");
        }
    }
}