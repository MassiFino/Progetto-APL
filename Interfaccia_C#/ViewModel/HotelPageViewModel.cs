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
using Interfaccia_C_.Model;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;


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
        public string Images { get; set; }  
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
        public ICommand CercaStanzaCommand { get; }
        public class Room : INotifyPropertyChanged
        {

            public int RoomID { get; set; }
            public string RoomName { get; set; }
            public string RoomDescription { get; set; }
            public double PricePerNight { get; set; }
            public int MaxGuests { get; set; }
            public string RoomType { get; set; }
            public string Images { get; set; }

            public ImageSource ImageSource { get; set; }



            private bool _isInterestSet = false;
            public bool IsInterestSet
            {
                get => _isInterestSet;
                set
                {
                    if (_isInterestSet != value)
                    {
                        _isInterestSet = value;
                        OnPropertyChanged();
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }


        public class Review : INotifyPropertyChanged
        {
            private string _reviewerName;
            public string ReviewerName
            {
                get => _reviewerName;
                set { _reviewerName = value; OnPropertyChanged(); }
            }

            private string _username;
            public string Username
            {
                get => _username;
                set { _username = value; OnPropertyChanged(); }
            }

            private string _comment;
            public string Comment
            {
                get => _comment;
                set { _comment = value; OnPropertyChanged(); }
            }

            private double _rating;
            public double Rating
            {
                get => _rating;
                set { _rating = value; OnPropertyChanged(); }
            }

            private string _createdAt;
            public string CreatedAt
            {
                get => _createdAt;
                set { _createdAt = value; OnPropertyChanged(); }
            }

            private bool _isReviewSectionVisible = true;
            public bool IsReviewSectionVisible
            {
                get => _isReviewSectionVisible;
                set { _isReviewSectionVisible = value; OnPropertyChanged(); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


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
                    OnPropertyChanged(); //?
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

        public bool NoRoomsAvailable => IsSearchMode && Rooms.Count == 0;

        public ICommand SetInterestCommand { get; }

        //Dario
        public HotelPageViewModel(Hotel hotel)
        {
            PrenotaStanzaCommand = new Command<Room>(async (roomSelezionata) =>
            {
                // Naviga con Shell, passando l'oggetto hotel
                var booking = new Booking
                {
                    roomID = roomSelezionata.RoomID,
                    roomName = roomSelezionata.RoomName,
                    roomImage = roomSelezionata.Images,
                    checkInDate = CheckInDate,
                    checkOutDate = CheckOutDate,
                    totalAmount = roomSelezionata.PricePerNight * (CheckOutDate - CheckInDate).Days,
                    status = "Confirmed", // Imposta lo stato come "Confirmed" 
                    hotelID = hotel.HotelID, 
                    hotelName = hotel.Name, 
                    hotelLocation = this.Location, // Posizione dell'hotel
                    roomType = roomSelezionata.RoomType,
                    guests = roomSelezionata.MaxGuests,
                    nights = (CheckOutDate - CheckInDate).Days,
                };
                // Naviga alla pagina di prenotazione e passa l'oggetto Booking come parametro
                var navParams = new Dictionary<string, object>
                {
                    ["Booking"] = booking
                };


                await Shell.Current.GoToAsync("BookingPage", navParams);
            });



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
                var imageList = hotel.Images.Split(';');

                var firstImage = imageList[0].Trim();

                Debug.WriteLine("Path immagine (prima del combine): " + firstImage);

                // Costruisco path completo
                string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                var imagePath = Path.Combine(projectDirectory, firstImage);
                Debug.WriteLine("Path completo: " + imagePath);

                ImageSource = GetImageSource(imagePath);
                Debug.WriteLine("Immaginissima: " + hotel.ImageSource);
            }

            SetInterestCommand = new Command<Room>(async (selectedRoom) =>
            {
                if (selectedRoom.IsInterestSet)
                {
                    await Shell.Current.DisplayAlert("Info", "Hai già impostato l'interesse per questa stanza", "OK");
                }
                else
                {
                    await OnSetInterest(selectedRoom);
                }
            });

        }
        private async void CercaStanza()
        {

            try
            {
                using var client = new HttpClient();

                var payload = new
                {
                    HotelID = this.HotelID,
                    CheckInDate = this.CheckInDate.ToString("yyyy-MM-dd"),
                    CheckOutDate = this.CheckOutDate.ToString("yyyy-MM-dd")
                };

                var token = await SecureStorage.GetAsync("jwt_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Rooms.Clear();

                var response = await client.PostAsync("http://localhost:9000/getAvailableRooms", content);

                Debug.WriteLine($"Response: {response}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var availableRooms = JsonSerializer.Deserialize<List<Room>>(jsonResponse);
                    if (availableRooms != null)
                    {
                        foreach (var room in availableRooms)
                        {
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



        public async System.Threading.Tasks.Task LoadRoomsAsync()
        {
            try
            {
                using var client = new HttpClient();

                var token = await SecureStorage.GetAsync("jwt_token");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonSerializer.Serialize(new { HotelID = HotelID });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://localhost:9000/getRoomsUser", content);


                if (response.IsSuccessStatusCode)
                {
                    var jsonR = await response.Content.ReadAsStringAsync();
                    var rooms = JsonSerializer.Deserialize<List<Room>>(jsonR);
                    if (rooms != null)
                    {
                        Rooms.Clear();
                        foreach (var room in rooms)
                        {

                            if (!string.IsNullOrEmpty(room.Images?.Trim()))
                            {

                                Debug.WriteLine($"Immagine: {room.Images}");
                                var imageList = room.Images.Split(';');
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
                    else
                    {
                        Message = "Non ho trovato nessuna recensione";

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

        //Massi
        private void ValidateSearchDates()
        {

            if (CheckInDate.Date < DateTime.Today)
            {
                IsSearchEnabled = false;
                SearchErrorMessage = "La data di check-in non può essere nel passato.";
                return;
            }

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
        //Massi
        private async Task OnSetInterest(Room selectedRoom)
        {
            var token = await SecureStorage.GetAsync("jwt_token");

            // (prezzo minimo) che vuole monitorare
            string input = await Application.Current.MainPage.DisplayPromptAsync(
                "Monitoraggio Prezzo",
                "Inserisci il valore (es. prezzo target) da monitorare:",
                placeholder: "Es. 150"
            );

            if (string.IsNullOrWhiteSpace(input) || !double.TryParse(input, out double monitorValue))
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Valore non valido", "OK");
                return;
            }

            if (monitorValue >= selectedRoom.PricePerNight)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Il valore di interesse deve essere inferiore al prezzo corrente della stanza.", "OK");
                return;
            }

            // Prepara il payload includendo il valore monitorato
            var payload = new
            {
                RoomID = selectedRoom.RoomID,
                MonitorValue = monitorValue  //valore inserito dall'utente
            };
            Debug.WriteLine("Paylod interessi: " + payload);

            var json = JsonSerializer.Serialize(payload);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://localhost:9000/setInterest", content);
            if (response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Interesse salvato",
                    $"Monitorerai le variazioni rispetto a {monitorValue}. Riceverai notifiche se il prezzo o la disponibilità variano.",
                    "OK");

                // Aggiorna lo stato della camera per indicare che l'interesse è impostato
                selectedRoom.IsInterestSet = true;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await Application.Current.MainPage.DisplayAlert("Errore", error, "OK");
            }
        }



    }
}