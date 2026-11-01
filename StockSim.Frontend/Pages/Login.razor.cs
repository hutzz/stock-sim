using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class Login {
        [CascadingParameter] private Task<AuthenticationState>? AuthState { get; set; }

        private LoginModel user = new();
        private string message = "";
        private bool showValidation;
        private EditForm? form;

        protected override async Task OnInitializedAsync() {
            base.OnInitialized();
            // An established session has no reason to sign in again, so skip past this page.
            if (AuthState == null) return;
            var state = await AuthState;
            if (state.User.Identity?.IsAuthenticated == true) {
                RedirectToDashboard();
            }
        }

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
                // Publish the new session in memory, then route client-side; a full reload would
                // tear the app down to a blank page before the dashboard appears.
                AuthProvider.NotifySignedIn(result.Access!);
                NavigationManager.NavigateTo("/dashboard");
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