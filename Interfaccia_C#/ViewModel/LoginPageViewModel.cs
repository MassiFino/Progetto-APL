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


namespace Interfaccia_C_.ViewModel
{
    public partial class LoginPageViewModel : INotifyPropertyChanged
    {

        private string username;
        private string password;


        // HttpClient statico per evitare la ricreazione in ogni richiesta
        private static readonly HttpClient _httpClient = new HttpClient();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public ICommand GoToRegisterCommand { get; }

        public LoginPageViewModel()
        {
            LoginCommand = new Command(async () => await Login());
            // Inizializza il comando con la logica di navigazione
            GoToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//RegisterPage"));
        }


        private async Task Login()
        {
            // Verifica che i campi non siano vuoti
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Errore", "Inserisci sia il nome utente che la password.", "OK");
                return;
            }

            // Prepara l'oggetto per la richiesta di login
            var loginRequest = new
            {
                username = this.Username,
                password = this.Password
            };

            // Serializza l'oggetto in JSON

            var json = JsonSerializer.Serialize(loginRequest);
            //mando l'oggetto json tramite richiesta http

            Debug.WriteLine("Contenuto della richiesta: " + json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Debug.WriteLine(await content.ReadAsStringAsync());


            try
            {
                // URL del server (modifica l'indirizzo in base alla tua configurazione)
                var url = "http://localhost:8000/login";

                // Invia la richiesta POST
                var response = await _httpClient.PostAsync(url, content);

                // Log dello stato della risposta
                Console.WriteLine($"Response Status: {response.StatusCode}");

                // Analizza la risposta
                if (response.IsSuccessStatusCode)
                {
                    // Leggi il contenuto della risposta
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Content: {responseContent}");

                    // Naviga alla pagina principale
                    await Shell.Current.DisplayAlert("Successo", "Login effettuato con successo!", "OK");
                    await Shell.Current.GoToAsync("//MainPage"); 
                }
                else
                {
                    // Gestione degli errori di autenticazione
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error Response: {errorContent}");
                    await Shell.Current.DisplayAlert("Errore", "Nome utente o password non validi.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Gestione degli errori di rete o altre eccezioni
                Console.WriteLine($"Errore nella richiesta: {ex.Message}");
                await Shell.Current.DisplayAlert("Errore", $"Errore di connessione: {ex.Message}", "OK");
            }
        }



    }
}
