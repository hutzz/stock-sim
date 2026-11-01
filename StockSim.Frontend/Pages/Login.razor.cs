using Microsoft.AspNetCore.Components.Forms;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class Login {
        private LoginModel user = new();
        private string message = "";
        private bool showValidation;
        private EditForm? form;

        protected override void OnAfterRender(bool firstRender) {
            if (firstRender && form?.EditContext != null) {
                form.EditContext.OnValidationStateChanged += HandleValidationStateChanged;
            }
        }

        public void Dispose() {
            if (form?.EditContext != null) {
                form.EditContext.OnValidationStateChanged -= HandleValidationStateChanged;
            }
        }

        private void HandleValidationStateChanged(object sender, ValidationStateChangedEventArgs args) {
            // Keep the panel visible while server-side errors remain, hide it once everything validates.
            showValidation = ((EditContext)sender).GetValidationMessages().Any() || !string.IsNullOrEmpty(message);
            StateHasChanged();
        }

        private async Task Valid() {
            showValidation = false;
            try {
                var result = await LoginService.Login(user);
                await TokenService.StoreTokensAsync(result);
                NavigationManager.NavigateTo("/dashboard", true);
            }
            catch (Exception e) {
                message = e.Message;
                showValidation = true;
            }
        }
        private void Invalid() {
            showValidation = true;
            StateHasChanged();
        }
        private void RedirectToDashboard() {
            NavigationManager.NavigateTo("/dashboard");
        }
    }
}