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
        // ObservableCollection di Hotel
        public ObservableCollection<Hotel> OwnedHotels { get; set; }

        // Comando per visualizzare il singolo hotel
        public ICommand ViewHotelCommand { get; }

        // Comando per aggiungere un hotel
        public ICommand AddHotelCommand { get; }

        private string userName;
        private string email;
        private string role;
        private ImageSource _profileImage;
        private string message;
        public ICommand GuardaCommand { get; set; }
        public ICommand AddRoomCommand { get; set; }

        public ProfilHostPageViewModel()
        {
            // Inizializza la lista degli hotel
            OwnedHotels = new ObservableCollection<Hotel>();
            // Comando per 'Guarda Offerta'
            GuardaCommand = new Command<Hotel>(async (hotelSelezionato) =>
            {
                // Naviga con Shell, passando l'oggetto hotel
                var navParams = new Dictionary<string, object>
                {
                    ["hotel"] = hotelSelezionato
                };

                await Shell.Current.GoToAsync("HotelPage", navParams);
            });

            // Imposta il comando per il pulsante "Visualizza"
            ViewHotelCommand = new Command<Hotel>(OnViewHotel);

            // Comando che naviga a una pagina "AddHotelPage" (shell route)
            AddHotelCommand = new Command(async () => await Shell.Current.GoToAsync("//AddHotelPage"));
            AddRoomCommand = new Command<Hotel>(async (hotelSelezionato) =>
            {
                // Naviga con Shell, passando l'oggetto hotel
                var navParams = new Dictionary<string, object>
                {
                    ["hotel"] = hotelSelezionato
                };

                await Shell.Current.GoToAsync("AddRoomPage", navParams);
            });


            // Carica dati utente e dati hotel
            LoadUserData();
            LoadHotelData();
        }

        // ----------------
        // PROPRIETÀ BINDATE
        // ----------------

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

        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        // -----------
        // COMANDI
        // -----------

        // Quando l'utente clicca sul pulsante "Visualizza hotel"
        private async void OnViewHotel(Hotel selectedHotel)
        {
            if (selectedHotel != null)
            {
                Debug.WriteLine($"Hotel selezionato: {selectedHotel.Name}");

              
            }
        }

        // ----------------
        // CARICAMENTO DATI
        // ----------------

        public async Task LoadUserData()
        {
            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("Token mancante! Reindirizzo al login?");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                var url = "http://localhost:9000/getUserData";
                using var client = new HttpClient();

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("{}", Encoding.UTF8, "application/json");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Risposta dela richiesta" + jsonResponse);

                    var userData = JsonSerializer.Deserialize<UserResponse>(jsonResponse);
                    if (userData != null)
                    {
                        UserName = userData.Username;
                        Email = userData.Email;
                        Role = userData.Role;

                        // Gestisci il percorso dell'immagine
                        string image = userData.PImage;
                        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                        var imagePath = Path.Combine(projectDirectory, image);

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
                    Debug.WriteLine("Token non presente! Reindirizza o gestisci di conseguenza.");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                var url = "http://localhost:9000/getHotelsHost";
                using var client = new HttpClient();

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("{}", Encoding.UTF8, "application/json");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(requestMessage);

                var jsonR = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Questa è la risposta: " + jsonR);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Questa è la risposta: " + jsonR);

                    var hotels = JsonSerializer.Deserialize<List<Hotel>>(jsonR);
                    if (hotels != null && hotels.Count > 0)
                    {
                        Message = "Questi sono i tuoi Hotel : ";
                        OwnedHotels.Clear();

                        foreach (var hotel in hotels)
                        {
                            // Se la lista dei servizi è null, la inizializzi
                            if (hotel.Services == null)
                                hotel.Services = new List<string>();

                            // Debug per vedere i servizi
                            foreach (var service in hotel.Services)
                            {
                                Debug.WriteLine($"Servizio dell'hotel: {service}");
                            }
                            Debug.WriteLine($"Servizi tutti : {hotel.Name}");

                            hotel.ServiziStringa = string.Join(", ", hotel.Services);

                            Debug.WriteLine($"Servizi tutti : {hotel.ServiziStringa}");
                            // Se dal server arriva una proprietà "Images" con più percorsi separati da virgola
                            // Esempio: "Pictures\\Hotel1.png, Pictures\\Hotel2.png"
                            if (!string.IsNullOrEmpty(hotel.Images?.Trim()))
                            {
                                // Divido la stringa in più immagini usando il delimitatore corretto (;)
                                var imageList = hotel.Images.Split(';', StringSplitOptions.RemoveEmptyEntries);

                                // Verifico che ci sia almeno un elemento prima di accedere all'indice 0
                                var firstImage = imageList[0].Trim();

                                Debug.WriteLine("Path immagine (prima del combine): " + firstImage);

                                // Costruisco path completo
                                string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                                var imagePath = Path.Combine(projectDirectory, firstImage);
                                Debug.WriteLine("Path completo: " + imagePath);

                                // Converto in ImageSource
                                hotel.ImageSource = GetImageSource(imagePath);
                                Debug.WriteLine("Immaginissima: " + hotel.ImageSource);
                            }

                            OwnedHotels.Add(hotel);
                        }
                    }
                    else
                    {
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

        // ----------------
        // Classe di supporto
        // ----------------
        public class UserResponse
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string PImage { get; set; }
        }

        // ----------------
        // Metodi ausiliari
        // ----------------

        // Ritorna un ImageSource a partire da un percorso su disco
        public ImageSource GetImageSource(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                Debug.WriteLine("Immagine trovata: " + imagePath);
                return ImageSource.FromFile(imagePath);
            }
            else
            {
                return null;
            }
        }

        // ----------------
        // INotifyPropertyChanged
        // ----------------

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }
}
