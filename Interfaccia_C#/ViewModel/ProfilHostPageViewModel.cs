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
using Interfaccia_C_.Model; // Importa il modello Hotel
using System.Net.Http.Headers; // Per AuthenticationHeaderValue
using Microsoft.Maui.Storage;   // Per SecureStorage
using System.Text;

namespace Interfaccia_C_.ViewModel
{
    public class ProfilHostPageViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<Hotel> OwnedHotels { get; set; }

public ICommand ViewHotelCommand { get; }

        public ICommand AddHotelCommand { get; }

        // Costruttore del ViewModel
        public ProfilHostPageViewModel()
{
            OwnedHotels = new ObservableCollection<Hotel>();  // Inizializza la lista degli hotel
            ViewHotelCommand = new Command(async () => await Shell.Current.GoToAsync("//HotelPage"));// Imposta il comando per il pulsante "Visualizza"
            AddHotelCommand = new Command(async () => await Shell.Current.GoToAsync("//AddHotelPage"));
            LoadUserData(); // Carica i dati dell'utente all'avvio
            LoadHotelData();   // Carica i dati degli hotel
}

     
        private string userName;
        private string email;
        private string role;
        private ImageSource _profileImage;
        private string message;

        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }
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
        // La proprietà ImageHotel per il binding
        private ImageSource _imageHotel;
        public ImageSource ImageHotel
        {
            get => _imageHotel;
            set
            {
                if (_imageHotel != value)
                {
                    _imageHotel = value;
                    OnPropertyChanged();  // Se la classe Hotel implementa INotifyPropertyChanged
                }
            }
        }
        // Classe per la risposta dell'API
        public class UserResponse
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string PImage { get; set; }

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

      


        // Metodo per ottenere l'immagine del profilo
        public ImageSource GetImageSource(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                Debug.WriteLine("Immagine trovata: " + imagePath);
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
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    // Non hai il token, l'utente non è loggato
                    Debug.WriteLine("Token mancante! Reindirizzo al login?");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                var url = "http://localhost:9000/getUserData"; // Cambia l'URL se necessario
                using var client = new HttpClient();

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                // Se devi mandare un body JSON vuoto, ad es.:
                requestMessage.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                // 3) Imposta l'header Authorization: Bearer <token>
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // 4) Invii la richiesta
                var response = await client.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Risposta dela richiesta" + jsonResponse);

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
                        Debug.WriteLine("Immaginissima profilo: " + ProfileImage);

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
       
       


        public async Task LoadHotelData()
        {
            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    // Se il token non esiste, significa che l'utente non è loggato
                    Debug.WriteLine("Token non presente! Reindirizza o gestisci di conseguenza.");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                var url = "http://localhost:9000/getHotelsHost"; // Cambia l'URL se necessario
                using var client = new HttpClient();

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                // Se il server si aspetta un body, anche fosse vuoto:
                // StringContent per un body "{}"
                requestMessage.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                // 4) Imposta l'header Authorization: Bearer <token>
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // 5) Invia la richiesta
                var response = await client.SendAsync(requestMessage);

                var jsonR = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Questa è la risposta: " + jsonR);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Questa è la risposta: " + jsonResponse);

                    // Deserializza la risposta in una lista di Hotel
                    var hotels = JsonSerializer.Deserialize<List<Hotel>>(jsonResponse);
                    if (hotels != null && hotels.Count > 0)
                    {
                        Message = "Questi sono i tuoi Hotel";

                        OwnedHotels.Clear(); // Pulisce la lista corrente
                        foreach (var hotel in hotels)
                        {// Assicurati che i servizi siano popolati correttamente

                            // Verifica che la lista dei servizi non sia null
                            if (hotel.services == null)
                            {
                                hotel.services = new List<string>(); // Se null, inizializza come lista vuota
                            }

                            // Debug per vedere i servizi
                            foreach (var service in hotel.services)
                            {
                                Debug.WriteLine($"Servizio dell'hotel: {service}");
                            }
                            // Debug per vedere il contenuto di Images
                            Debug.WriteLine("Contenuto di hotel.Images: '" + hotel.images + "'");

                            // Gestisci le immagini: prendi solo la prima immagine dalla stringa separata da virgole
                            if (!string.IsNullOrEmpty(hotel.images?.Trim())) // Aggiungi Trim() per rimuovere spazi bianchi
                            {
                                var imageList = hotel.images.Split(','); // Dividi la stringa in un array di immagini
                                hotel.images = imageList[0]; // Prendi solo la prima immagine
                                Debug.WriteLine("Path immagine: " + hotel.images);
                            }

                            // Gestisci il percorso dell'immagine per l'hotel
                            string image = hotel.images;
                            Debug.WriteLine("Path immagine hotel: " + image);

                            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                            var imagePath = Path.Combine(projectDirectory, image);
                            Debug.WriteLine("Path completo " + imagePath);
                            // Imposta l'immagine dell'hotel se il percorso è valido
                            hotel.ImageSource = GetImageSource(imagePath); // Imposta la proprietà ImageSource
                            Debug.WriteLine("Immaginissima: " + hotel.ImageSource);

                            // Aggiungi l'hotel alla lista
                            OwnedHotels.Add(hotel);
                        }
                    }
                    else
                    {
                        // Se non ci sono hotel, imposta il messaggio "Non ho trovato hotel"
                        Message = "Non ho trovato hotel";
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
