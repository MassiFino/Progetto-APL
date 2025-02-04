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
        public ICommand GoToSearchCommand { get; }


        // Inizializza il comando con la logica di navigazione
        



        public MainPageViewModel()
        {
            // 1) Avviamo il controllo del token
            CheckToken();

            GuardaOffertaCommand = new Command(() =>
            {
                Debug.WriteLine("Comando GuardaOfferta eseguito");
            });

            GoToSearchCommand = new Command(async () => await Shell.Current.GoToAsync("//SearchPage"));

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
