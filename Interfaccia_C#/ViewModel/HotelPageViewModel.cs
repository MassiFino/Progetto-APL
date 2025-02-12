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
using Interfaccia_C_.Model;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;


namespace Interfaccia_C_.ViewModel
{
    public class HotelPageViewModel : INotifyPropertyChanged
    {

        public int HotelID { get; }
        public string Name { get; }
        public string Location { get; }
        public string Description { get; }
        public double Rating { get; }
        public string Services { get; set; }
        public string Images { get; set; }  // Stringa che contiene il path (o i path)
        public ImageSource ImageSource { get; set; }

        public string ServiziStringa { get; set; }

        private ObservableCollection<ImageSource> _imageList;
        public ObservableCollection<ImageSource> ImageList
        {
            get => _imageList;
            set
            {
                _imageList = value;
                OnPropertyChanged();
            }
        }
        private DateTime _checkInDate;
        private DateTime _checkOutDate;


        public DateTime CheckInDate
        {
            get => _checkInDate;
            set
            {
                if (_checkInDate != value)
                {
                    _checkInDate = value;
                    OnPropertyChanged(nameof(CheckInDate)); // Notifica la modifica
                    ValidateSearchDates();
                }
            }
        }

        public DateTime CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                if (_checkOutDate != value)
                {
                    _checkOutDate = value;
                    OnPropertyChanged(nameof(CheckOutDate)); // Notifica la modifica
                    ValidateSearchDates();
                }
            }
        }
        // Comando per il bottone "Prenota stanza"
        public ICommand CercaStanzaCommand { get; }
        public class Room : INotifyPropertyChanged
        {

            private bool _canBook = true;

            public int RoomID { get; set; }
            public string RoomName { get; set; }
            public string RoomDescription { get; set; }
            public double PricePerNight { get; set; }
            public int MaxGuests { get; set; }
            public string RoomType { get; set; }
            public string Images { get; set; }
            public ImageSource ImageSource { get; set; }


            public bool CanBook
            {
                get => _canBook;
                set
                {
                    if (_canBook != value)
                    {
                        _canBook = value;
                        OnPropertyChanged();
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public class Review
        {
            public string ReviewerName { get; set; }
            public string Comment { get; set; }
            public double Rating { get; set; }
        }


        // Nuove proprietà per le stanze e le recensioni
        public ObservableCollection<Room> Rooms { get; set; } = new ObservableCollection<Room>();
        public ObservableCollection<Review> Reviews { get; set; } = new ObservableCollection<Review>();
        public ICommand PrenotaStanzaCommand { get; }

        private bool _isSearchMode;
        public bool IsSearchMode
        {
            get => _isSearchMode;
            set
            {
                if (_isSearchMode != value)
                {
                    _isSearchMode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NoRoomsAvailable));
                }
            }
        }

        private bool _isSearchEnabled;
        private string _searchErrorMessage;

        public bool IsSearchEnabled
        {
            get => _isSearchEnabled;
            set
            {
                if (_isSearchEnabled != value)
                {
                    _isSearchEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SearchErrorMessage
        {
            get => _searchErrorMessage;
            set
            {
                if (_searchErrorMessage != value)
                {
                    _searchErrorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        // Proprietà calcolata per mostrare il messaggio "Nessuna stanza disponibile..."
        public bool NoRoomsAvailable => IsSearchMode && Rooms.Count == 0;

        // Costruttore che accetta un oggetto Hotel
        public HotelPageViewModel(Hotel hotel)
        {
            //Se è possibile cercare di mettere meno poccibile nel costruttore e creiamo un'altra funzione che carica i dati
            PrenotaStanzaCommand ??= new Command<Room>(async (Room) => await PrenotaStanza(Room));

            HotelID = hotel.HotelID;
            Name = hotel.Name;
            Location = hotel.Location;
            Description = hotel.Description;
            Rating = hotel.Rating;
            Services = string.Join(", ", hotel.Services);

            CheckInDate = DateTime.Today;
            CheckOutDate = DateTime.Today.AddDays(1);

            ValidateSearchDates(); // Imposta il valore iniziale

            // Inizialmente non siamo in modalità ricerca

            IsSearchMode = false;
            CercaStanzaCommand = new Command(CercaStanza, () => IsSearchEnabled);

            this.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(IsSearchEnabled))
                {
                    ((Command)CercaStanzaCommand).ChangeCanExecute();
                }
            };

            if (!string.IsNullOrEmpty(hotel.Images?.Trim()))
            {
                // Stampa il valore di hotel.Images

                // Dividi la stringa delle immagini in base alla virgola
                var imageList = hotel.Images.Split(',').Select(img => img.Trim()).ToList();


                // Recupera il percorso della cartella principale del progetto
                string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;


                // Crea l'ObservableCollection di ImageSource
                ImageList = new ObservableCollection<ImageSource>(
                    imageList.Select(img =>
                    {
                        // Costruisci il percorso completo per ogni immagine
                        var imagePath = Path.Combine(projectDirectory, img);

                        // Stampa il percorso completo di ciascuna immagine
                        Debug.WriteLine("Percorso immagine: " + imagePath);

                        // Usa il metodo GetImageSource per ottenere l'immagine
                        var imageSource = GetImageSource(imagePath);

                        return imageSource;
                    }).Where(img => img != null)  // Rimuovi gli oggetti nulli dalla lista
                );

                // Stampa la lista finale delle immagini caricate
                Debug.WriteLine("Numero di immagini caricate: " + ImageList.Count);
            }
            else
            {
                // Se la stringa delle immagini è vuota o nulla, stampa un messaggio
                Debug.WriteLine("La proprietà 'Images' è vuota o nulla.");
            }

        }
        private async void CercaStanza()
        {                 // Si deve inserire quello fatto nel load Room

            try
            {
                using var client = new HttpClient();

                // Costruisci il payload con HotelID, CheckInDate e CheckOutDate formattate (ISO "yyyy-MM-dd")
                var payload = new
                {
                    HotelID = this.HotelID,
                    CheckInDate = this.CheckInDate.ToString("yyyy-MM-dd"),
                    CheckOutDate = this.CheckOutDate.ToString("yyyy-MM-dd")
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Rooms.Clear();

                // Chiama l'endpoint dedicato alle stanze disponibili
                var response = await client.PostAsync("http://localhost:9000/getAvailableRooms", content);

                Debug.WriteLine($"Response: {response}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    // Assumiamo che l'endpoint restituisca una lista di Room
                    var availableRooms = JsonSerializer.Deserialize<List<Room>>(jsonResponse);
                    if (availableRooms != null)
                    {
                        foreach (var room in availableRooms)
                        {
                            // Se il campo Images contiene più percorsi, prendi la prima immagine
                            if (!string.IsNullOrWhiteSpace(room.Images))
                            {
                                var imageList = room.Images.Split(',');
                                var firstImage = imageList.First().Trim();
                                string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)
                                    ?.Parent?.Parent?.Parent?.FullName;
                                var imagePath = Path.Combine(projectDirectory, firstImage);
                                room.ImageSource = GetImageSource(imagePath);
                            }
                            Rooms.Add(room);
                        }
                    }
                    IsSearchMode = true;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Errore", "Impossibile recuperare le stanze disponibili", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Eccezione in CercaStanza: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Errore", ex.Message, "OK");
            }
        }
        private async Task PrenotaStanza(Room room)
        {
            // Controllo se la sessione (token) è attiva
            var token = await SecureStorage.GetAsync("jwt_token");

            // Calcola il numero di notti
            int nights = (int)(CheckOutDate.Date - CheckInDate.Date).TotalDays;
            if (nights <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Il numero di notti deve essere maggiore di zero.", "OK");
                return;
            }

            // Calcola l'importo totale
            double totalAmount = room.PricePerNight * nights;

            // Recupera l'username dal token oppure da una proprietà del ViewModel (se lo hai già estratto in precedenza)
            // Qui presumo di avere un metodo GetLoggedUsername() che lo estrae dal token
            // Crea il payload per la prenotazione
            var payload = new
            {
                RoomID = room.RoomID,
                CheckInDate = CheckInDate.ToString("yyyy-MM-dd"),
                CheckOutDate = CheckOutDate.ToString("yyyy-MM-dd"),
                TotalAmount = totalAmount,
                Status = "Confirmed"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync("http://localhost:9000/addBooking", content);
                if (response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Prenotazione Effettuata",
                        $"Hai prenotato la stanza: {room.RoomName}\n" +
                        $"Check-in: {CheckInDate.ToShortDateString()}\n" +
                        $"Check-out: {CheckOutDate.ToShortDateString()}\n" +
                        $"Totale: {totalAmount:C}",
                        "OK");

                    room.CanBook = false;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Errore", "Si è verificato un errore durante la prenotazione. Riprova.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Eccezione in PrenotaStanza: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Errore", ex.Message, "OK");
            }
        }


        public async System.Threading.Tasks.Task LoadRoomsAsync()
        {
            try
            {
                using var client = new HttpClient();

                var json = JsonSerializer.Serialize(new { HotelID = HotelID });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:9000/getRooms", content);


                if (response.IsSuccessStatusCode)
                {
                    var jsonR = await response.Content.ReadAsStringAsync();
                    var rooms = JsonSerializer.Deserialize<List<Room>>(jsonR);
                    if (rooms != null)
                    {
                        Rooms.Clear();
                        foreach (var room in rooms)
                        {

                            // Se il campo Images contiene più percorsi, puoi gestirlo in modo simile
                            if (!string.IsNullOrEmpty(room.Images?.Trim()))
                            {

                                Debug.WriteLine($"Immagine: {room.Images}");
                                var imageList = room.Images.Split(',');
                                var firstImage = imageList[0].Trim();
                                string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                                var imagePath = System.IO.Path.Combine(projectDirectory, firstImage);
                                room.ImageSource = GetImageSource(imagePath);
                            }
                            Rooms.Add(room);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"Errore nel recupero delle stanze: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Eccezione durante il caricamento delle stanze: {ex.Message}");
            }
        }


        // Funzione per recuperare le recensioni per l'hotel (o per una delle sue stanze)
        public async System.Threading.Tasks.Task LoadReviewsAsync()
        {
            try
            {
                using var client = new HttpClient();

                var payload = JsonSerializer.Serialize(new { HotelID = HotelID });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://localhost:9000/getHotelReviews", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var reviews = JsonSerializer.Deserialize<List<Review>>(json);
                    if (reviews != null)
                    {
                        Reviews.Clear();
                        foreach (var review in reviews)
                        {
                            Reviews.Add(review);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"Errore nel recupero delle recensioni: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Eccezione durante il caricamento delle recensioni: {ex.Message}");
            }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Metodo per invocare l'evento PropertyChanged
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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


        private void ValidateSearchDates()
        {

            if (CheckInDate.Date < DateTime.Today)
            {
                IsSearchEnabled = false;
                SearchErrorMessage = "La data di check-in non può essere nel passato.";
                return;
            }

            // Se le date sono predefinite o non coerenti, disabilita la ricerca
            if (CheckOutDate <= CheckInDate)
            {
                IsSearchEnabled = false;
                SearchErrorMessage = "La data di check-out deve essere successiva a quella di check-in.";
            }
            else
            {
                IsSearchEnabled = true;
                SearchErrorMessage = string.Empty;
            }
        }

    }
}