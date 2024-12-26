using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Text.Json.Serialization;

namespace Interfaccia_C_.ViewModel
{
    public class ProfileUserPageViewModel : INotifyPropertyChanged
    {
        // Costruttore del ViewModel
        public ProfileUserPageViewModel()
        {
            LoadUserData(); // Carica i dati dell'utente all'avvio
        }

        private string userName;
        private string email;
        private string role;
        private ImageSource _profileImage;

        // Proprietà per il binding
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Role
        {
            get => role;
            set => SetProperty(ref role, value);
        }

        public ImageSource ProfileImage
        {
            get => _profileImage;
            set
            {
                if (_profileImage != value)
                {
                    _profileImage = value;
                    OnPropertyChanged();
                }
            }
        }

        // Gestione delle modifiche alle proprietà
        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        // Eventi per il binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Classe per la risposta dell'API
        public class UserResponse
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string PImage { get; set; }
            public override string ToString()
            {
                return $"Username: {Username}, Email: {Email}, Role: {Role}, PImage: {PImage}";
            }
        }

        // Metodo per ottenere l'immagine del profilo
        public ImageSource GetImageSource(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                return ImageSource.FromFile(imagePath);  // Usa FileImageSource per caricare l'immagine
            }
            else
            {
                return null;  // Immagine non trovata
            }
        }

        // Metodo per caricare i dati dell'utente
        private async Task LoadUserData()
        {
            try
            {
                var url = "http://localhost:9000/getUserData"; // Cambia l'URL se necessario
                using var client = new HttpClient();
                var response = await client.PostAsync(url, new StringContent(""));
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserializza la risposta in un oggetto UserResponse
                    var userData = JsonSerializer.Deserialize<UserResponse>(jsonResponse);
                    if (userData != null)
                    {
                        // Assegna i valori alle proprietà
                        UserName = userData.Username;
                        Email = userData.Email;
                        Role = userData.Role;

                        // Gestisci il percorso dell'immagine
                        string image = userData.PImage;
                        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                        var imagePath = Path.Combine(projectDirectory, image);

                        // Imposta l'immagine del profilo se il percorso è valido
                        ProfileImage = GetImageSource(imagePath);
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await Application.Current.MainPage.DisplayAlert("Errore", $"Caricamento dati fallito: {errorContent}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", $"Si è verificato un errore: {ex.Message}", "OK");
            }
        }
    }
}
