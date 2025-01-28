using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Text.Json;
using System.Diagnostics;
using System.Net.Http.Headers; // Per AuthenticationHeaderValue
using Microsoft.Maui.Storage;   // Per SecureStorage

namespace Interfaccia_C_.ViewModel
{
    public  class MainPageViewModel
    {
        public ICommand GuardaOffertaCommand { get; set; }

        public MainPageViewModel()
        {
            // 1) Avviamo il controllo del token
            CheckToken();

            // 2) Inizializza i comandi (se servono)
            GuardaOffertaCommand = new Command(() =>
            {
                Debug.WriteLine("Comando GuardaOfferta eseguito");
            });
        }

        private async void CheckToken()
        {
            // Leggiamo il token da SecureStorage
            var token = await SecureStorage.GetAsync("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                // Se mancante, reindirizza al Login
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                Debug.WriteLine($"Token presente: {token.Substring(0, Math.Min(token.Length, 10))}...");
            }
        }

    }
}
