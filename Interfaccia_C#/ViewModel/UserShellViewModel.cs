using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Text.Json.Serialization;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;


namespace Interfaccia_C_.ViewModel
{
    public class UserShellViewModel
    {
        // Comando di logout
        public ICommand LogoutCommand { get; }

        public UserShellViewModel()
        {
            // Inizializza il comando
            LogoutCommand = new Command(ExecuteLogout);

        }

        // Metodo che esegue il logout
        private async void ExecuteLogout()
        {

            SecureStorage.Remove("jwt_token");
            // Mostra un alert di logout
            await App.Current.MainPage.DisplayAlert("Logout", "Sei stato disconnesso.", "OK");

            // Crea una nuova Shell specifica per il ruolo Host
            var appShell = new AppShell();

            // Imposta la nuova Shell come finestra principale
            Application.Current.MainPage = appShell;

            // Naviga alla pagina di login
            await appShell.GoToAsync("//LoginPage");
        }
    }
}