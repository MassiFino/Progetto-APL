using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Interfaccia_C_.Model; // Importa il modello Hotel
using System.Net.Http.Headers; // Per AuthenticationHeaderValue
using Microsoft.Maui.Storage;   // Per SecureStorage
using System.Text;


namespace Interfaccia_C_.ViewModel
{
    public class ProfileUserPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Booking> OwnedBookings { get; set; }
        // Costruttore del ViewModel
        public ProfileUserPageViewModel()
        {
            OwnedBookings = new ObservableCollection<Booking>();  // Inizializza la lista degli hotel
            LoadUserData(); // Carica i dati dell'utente all'avvio
            LoadBookingData();
        }

        private string userName;
        private string email;
        private string role;
        private ImageSource _profileImage;
        private string message;
        // Classe per la risposta dell'API
        public class UserResponse
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string PImage { get; set; }

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

        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
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
                Debug.WriteLine("Immagine non trovata: " + imagePath);

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
        public async Task LoadBookingData()
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

                var url = "http://localhost:9000/getBookings"; 
                using var client = new HttpClient();

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                // Se devi mandare un body JSON vuoto, ad es.:
                requestMessage.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                // 3) Imposta l'header Authorization: Bearer <token>
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // 4) Invii la richiesta
                var response = await client.SendAsync(requestMessage);

                var jsonR = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Questa è la risposta: " + jsonR);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Questa è la risposta: " + jsonResponse);

                    // Deserializza la risposta in una lista di prenotazioni
                    var bookings = JsonSerializer.Deserialize<List<Booking>>(jsonResponse);
                    if (bookings != null && bookings.Count > 0)
                    {
                        Debug.WriteLine("Questa è dopo la serializzazione: " + bookings);

                        OwnedBookings.Clear(); // Pulisce la lista corrente
                        Message = "Queste sono le tue prenotazioni";
                        foreach (var booking in bookings)
                        {
                           
                            // Debug per vedere i dettagli della prenotazione
                            Debug.WriteLine($"Prenotazione: {booking.roomName}");
                            Debug.WriteLine($"Check-in: {booking.checkInDate.ToShortDateString()}");
                            Debug.WriteLine($"Check-out: {booking.checkOutDate.ToShortDateString()}");
                            Debug.WriteLine($"Importo: €{booking.totalAmount:F2}");
                            Debug.WriteLine($"Stato: {booking.status}");
                            
                            Debug.WriteLine($"RoomName: {booking.roomImage}");

                            Debug.WriteLine($"HotelName: {booking.hotelName}");

                            // Assegna l'immagine della stanza alla proprietà di binding
                            // Gestisci il percorso dell'immagine per l'hotel
                            // Gestisci le immagini: prendi solo la prima immagine dalla stringa separata da virgole
                            if (!string.IsNullOrEmpty(booking.roomImage?.Trim())) // Aggiungi Trim() per rimuovere spazi bianchi
                            {
                                var imageList = booking.roomImage.Split(','); // Dividi la stringa in un array di immagini
                                booking.roomImage = imageList[0]; // Prendi solo la prima immagine
                                Debug.WriteLine("Path immagine: " + booking.roomImage);
                            }

                            Debug.WriteLine($"RoomImages: {booking.roomImage}");
                            string image = booking.roomImage;
                            Debug.WriteLine("Path immagine hotel: " + image);

                            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                            var imagPath = Path.Combine(projectDirectory, image);
                            Debug.WriteLine("Path completo " + imagPath);
                            // Imposta l'immagine dell'hotel se il percorso è valido
                            booking.ImageSource = GetImageSource(imagPath); // Imposta la proprietà ImageSource
                            Debug.WriteLine("Immaginissima room: " + booking.ImageSource);

                            // Aggiungi la prenotazione alla lista
                            OwnedBookings.Add(booking);
                        }
                    }
                    else
                    {
                        // Se non ci sono prenotazioni, imposta il messaggio "Non hai prenotazioni"
                        Message = "Non hai prenotazioni";
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
