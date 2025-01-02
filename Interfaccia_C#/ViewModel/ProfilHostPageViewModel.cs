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

namespace Interfaccia_C_.ViewModel
{
    public class ProfileHostPageViewModel : INotifyPropertyChanged
    {public ObservableCollection<Hotel> OwnedHotels { get; set; }

public ICommand ViewHotelCommand { get; }

// Costruttore del ViewModel
public ProfileHostPageViewModel()
{
    OwnedHotels = new ObservableCollection<Hotel>();  // Inizializza la lista degli hotel
    ViewHotelCommand = new Command<Hotel>(OnViewHotel);  // Imposta il comando per il pulsante "Visualizza"
    
    LoadUserData(); // Carica i dati dell'utente all'avvio
    LoadHotelData();   // Carica i dati degli hotel
}

private async void OnViewHotel(Hotel selectedHotel)
{
    if (selectedHotel != null)
    {
        // Debug per confermare l'hotel selezionato
        Debug.WriteLine($"Hotel selezionato: {selectedHotel.name}");

        // Naviga alla pagina dell'hotel, passando l'hotel selezionato
        await Application.Current.MainPage.Navigation.PushAsync(new HotelPage(selectedHotel));
    }
}

        private string userName;
        private string email;
        private string role;
        private ImageSource _profileImage;
        private ImageSource _hotelImage;
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
       
        public string name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public List<string> services { get; set; }
        public double rating { get; set; }
        public string images { get; set; }

        // La proprietà ImageHotel per il binding
        public ImageSource ImageHotel
        {
            get => _hotelImage;
            set
            {
                if (_hotelImage != value)
                {
                    _hotelImage = value;
                    OnPropertyChanged();
                }
            }
        }
        public async Task LoadHotelData()
        {
            try
            {
                var url = "http://localhost:9000/getHotelsHost"; // Cambia l'URL se necessario
                using var client = new HttpClient();

                // Esegui la richiesta all'API
                var response = await client.PostAsync(url, new StringContent(""));

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Questa è la risposta: " + jsonResponse);

                    // Deserializza la risposta in una lista di Hotel
                    var hotels = JsonSerializer.Deserialize<List<Hotel>>(jsonResponse);
                    if (hotels != null)
                    {

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
                            ImageHotel = GetImageSource(imagePath);

                            // Aggiungi l'hotel alla lista
                            OwnedHotels.Add(hotel);
                        }
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
