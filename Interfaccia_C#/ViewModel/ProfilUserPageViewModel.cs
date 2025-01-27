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

        public class Booking
        {
            public int bookingID { get; set; }  // Identificativo univoco della prenotazione
            public string username { get; set; }  // Nome utente del cliente
            public DateTime checkInDate { get; set; }  // Data di check-in
            public DateTime checkOutDate { get; set; }  // Data di check-out
            public decimal totalAmount { get; set; }  // Importo totale della prenotazione
            public string status { get; set; }  // Stato della prenotazione
            public string roomName { get; set; }  // Nome della stanza
            public string hotelName { get; set; }  // URL o percorso dell'immagine della stanza
            public string hotelLocation { get; set; }
            
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
        public async Task LoadBookingData()
        {
            try
            {
                var url = "http://localhost:9000/getBookings"; // Cambia l'URL se necessario
                using var client = new HttpClient();

                // Esegui la richiesta all'API
                var response = await client.PostAsync(url, new StringContent(""));
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

                        foreach (var booking in bookings)
                        {
                            Message = "";

                            // Debug per vedere i dettagli della prenotazione
                            Debug.WriteLine($"Prenotazione: {booking.roomName}");
                            Debug.WriteLine($"Check-in: {booking.checkInDate.ToShortDateString()}");
                            Debug.WriteLine($"Check-out: {booking.checkOutDate.ToShortDateString()}");
                            Debug.WriteLine($"Importo: €{booking.totalAmount:F2}");
                            Debug.WriteLine($"Stato: {booking.status}");
                            Debug.WriteLine($"RoomName: {booking.roomName}");
                            Debug.WriteLine($"HotelName: {booking.hotelName}");

                            // Assegna l'immagine della stanza alla proprietà di binding

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
