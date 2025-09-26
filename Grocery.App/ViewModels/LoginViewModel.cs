using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Helpers;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.App.ViewModels
{
    public partial class LoginViewModel(IAuthService authService, IClientService clientService, GlobalViewModel global) : BaseViewModel
    {
        private readonly IAuthService _authService = authService;
        private readonly IClientService _clientService = clientService;
        private readonly GlobalViewModel _global = global;

        // ============================
        // Login Properties
        // ============================
        [ObservableProperty]
        private string email = "user3@mail.com";

        [ObservableProperty]
        private string password = "user3";

        [ObservableProperty]
        private string? loginMessage;

        // ============================
        // Register Properties
        // ============================
        [ObservableProperty]
        private string? registerName;

        [ObservableProperty]
        private string? registerEmail;

        [ObservableProperty]
        private string? registerPassword;

        [ObservableProperty]
        private string? registerMessage;

        // ============================
        // Login Command
        // ============================
        [RelayCommand]
        private void Login()
        {
            var authenticatedClient = _authService.Login(Email, Password);
            if (authenticatedClient != null)
            {
                LoginMessage = $"Welkom {authenticatedClient.Name}!";
                _global.Client = authenticatedClient;
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new AppShell();
                }
            }
            else
            {
                LoginMessage = "Ongeldige inloggegevens.";
            }
        }

        // ============================
        // Register Command
        // ============================
        [RelayCommand]
        private void Register()
        {
            // Validatie
            if (string.IsNullOrWhiteSpace(RegisterName) ||
                string.IsNullOrWhiteSpace(RegisterEmail) ||
                string.IsNullOrWhiteSpace(RegisterPassword))
            {
                RegisterMessage = "Alle velden zijn verplicht.";
                return;
            }

            if (RegisterPassword.Length < 6)
            {
                RegisterMessage = "Wachtwoord is te zwak. Minimaal 6 tekens.";
                return;
            }

            // Check uniek e-mail
            if (_clientService.Get(RegisterEmail) != null)
            {
                RegisterMessage = "E-mailadres is al geregistreerd.";
                return;
            }

            // Maak nieuw account
            string hashedPassword = PasswordHelper.HashPassword(RegisterPassword);
            Client newClient = new Client(0, RegisterName, RegisterEmail, hashedPassword);

            // Voeg client toe (hier nog simulatie, afhankelijk van repository)
            // TODO: Voeg daadwerkelijk toe aan repository/db
            RegisterMessage = "Registratie succesvol! Je kunt nu inloggen.";

            // Optioneel: leeg velden na registratie
            RegisterName = "";
            RegisterEmail = "";
            RegisterPassword = "";
        }
    }
}
