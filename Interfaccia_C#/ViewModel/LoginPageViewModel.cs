﻿using System;
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
using System.Data;
using Microsoft.Maui.Storage;


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

        public class LoginResponse
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string role { get; set; }
            public string token { get; set; }
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
                Username = this.Username,
                Password = this.Password
            };

            // Serializza l'oggetto in JSON

            var json = JsonSerializer.Serialize(loginRequest);

            Debug.WriteLine("Contenuto della richiesta: " + json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Debug.WriteLine(await content.ReadAsStringAsync());


            try
            {
                var url = "http://localhost:9000/login";

                // Invia la richiesta POST
                var response = await _httpClient.PostAsync(url, content);

                Console.WriteLine($"Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("ho fatto login" + responseContent);

                    // Deserializza la risposta JSON in un oggetto LoginResponse
                    var userData = JsonSerializer.Deserialize<LoginResponse>(responseContent);

                    if (!string.IsNullOrEmpty(userData.token))
                    {
                        // Salviamo il token in SecureStorage
                        await SecureStorage.SetAsync("jwt_token", userData.token);
                    }
                    string role = userData.role;

                    
                    Debug.WriteLine("stampa questo "+ role);


                    // Carica la Shell appropriata in base al ruolo
                    if (role == "host")
                    {

                        Debug.WriteLine("sono un: " + role);

                        var hostShell = new HostShell();

                        // Imposta la nuova Shell come finestra principale
                        Application.Current.MainPage = hostShell;

                        await hostShell.GoToAsync("//ProfilePage");

                    }
                    else
                    {
                        Debug.WriteLine("sono uno: r" + role);
                        var userShell = new UserShell();

                        Application.Current.MainPage = userShell;

                        // Naviga alla pagina MainPage
                        await userShell.GoToAsync("//MainPage");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error Response: {errorContent}");
                    await Shell.Current.DisplayAlert("Errore", "Nome utente o password non validi.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nella richiesta: {ex.Message}");
                await Shell.Current.DisplayAlert("Errore", $"Errore di connessione: {ex.Message}", "OK");
            }
        }



    }
}
