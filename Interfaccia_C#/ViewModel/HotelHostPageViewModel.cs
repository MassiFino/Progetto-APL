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
    public class HotelHostPageViewModel : INotifyPropertyChanged
    {

        public int HotelID { get; }
        public string Name { get; }
        public string Location { get; }
        public string Description { get; set; }
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

        public class Room : INotifyPropertyChanged
        {

            private bool _canBook = true;

            public int RoomID { get; set; }
            public string RoomName { get; set; }
            public string RoomDescription { get; set; }

            private double _pricePerNight;
            public double PricePerNight
            {
                get => _pricePerNight;
                set
                {
                    if (_pricePerNight != value)
                    {
                        _pricePerNight = value;
                        OnPropertyChanged();
                    }
                }
            }
            public int MaxGuests { get; set; }
            public string RoomType { get; set; }
            public string Images { get; set; }

            public ImageSource ImageSource { get; set; }


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

            // Imposta true di default per mostrare i dettagli della recensione
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

        public ICommand DeleteRoomCommand { get; }

        public ICommand EditHotelDescriptionCommand { get; }

        public ICommand AddRoomCommand { get; }

        public ICommand UpdateRoomPriceCommand => new Command<Room>(async (room) => await OnUpdateRoomPrice(room));

        private ObservableCollection<Booking> _bookings = new ObservableCollection<Booking>();
        public ObservableCollection<Booking> Bookings
        {
            get => _bookings;
            set
            {
                _bookings = value;
                OnPropertyChanged();
            }
        }



        // Costruttore che accetta un oggetto Hotel
        public HotelHostPageViewModel(Hotel hotel)
        {
            HotelID = hotel.HotelID;
            Name = hotel.Name;
            Location = hotel.Location;
            Description = hotel.Description;
            Rating = hotel.Rating;
            Services = string.Join(", ", hotel.Services);

            Debug.WriteLine("Hotel id: " + HotelID);

            if (!string.IsNullOrEmpty(hotel.Images?.Trim()))
            {
                // Divido la stringa in pi� immagini
                var imageList = hotel.Images.Split(';');
                // Prendo la prima
                var firstImage = imageList[0].Trim();

                Debug.WriteLine("Path immagine (prima del combine): " + firstImage);

                // Costruisco path completo
                string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                var imagePath = Path.Combine(projectDirectory, firstImage);
                Debug.WriteLine("Path completo: " + imagePath);

                ImageSource = GetImageSource(imagePath);
                Debug.WriteLine("Immaginissima: " + hotel.ImageSource);
            }

            DeleteRoomCommand = new Command<Room>(async (room) => await OnDeleteRoom(room));
            EditHotelDescriptionCommand = new Command(async () => await OnEditHotelDescription());
            AddRoomCommand = new Command(async () => await OnAddRoom());

        }



        public async System.Threading.Tasks.Task LoadRoomsAsync()
        {
            try
            {
                using var client = new HttpClient();

                var json = JsonSerializer.Serialize(new { HotelID = HotelID });

                Debug.WriteLine($"Hotel ID: {HotelID}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:9000/getRooms", content);

                var responseStr = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Response JSON: " + responseStr);


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

                var responseStr = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Response JSON: " + responseStr);

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
        private async Task OnDeleteRoom(Room room)
        {
            var token = await SecureStorage.GetAsync("jwt_token");

            var payload = new { RoomID = room.RoomID};
            var json = JsonSerializer.Serialize(payload);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:9000/deleteRoom", content);
            if (response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Successo", "Stanza eliminata con successo", "OK");
                Rooms.Remove(room);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await Application.Current.MainPage.DisplayAlert("Errore", error, "OK");
            }
        }

        //Massi
        private async Task OnEditHotelDescription()
        {
            string newDescription = await Application.Current.MainPage.DisplayPromptAsync("Modifica Descrizione", "Inserisci la nuova descrizione dell'hotel:");
            if (string.IsNullOrWhiteSpace(newDescription))
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Descrizione non valida", "OK");
                return;
            }
            var token = await SecureStorage.GetAsync("jwt_token");
  
            var payload = new { HotelID = HotelID, NewDescription = newDescription};
            var json = JsonSerializer.Serialize(payload);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:9000/updateHotelDescription", content);
            if (response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Successo", "Descrizione aggiornata", "OK");
                Description = newDescription;
                OnPropertyChanged(nameof(Description));
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await Application.Current.MainPage.DisplayAlert("Errore", error, "OK");
            }
        }

        //Dario
        private async Task OnAddRoom()
        {
            var navParams = new Dictionary<string, object>
            {
                ["hotel"] = new Hotel
                {
                    HotelID = this.HotelID,
                    Name = this.Name,
                    Location = this.Location,
                    Description = this.Description,
                    Rating = this.Rating,
                    Services = this.Services.Split(",").ToList(),
                    Images = this.Images
                }
            };
            await Shell.Current.GoToAsync("AddRoomPage", navParams);
        }

        //Massi
        private async Task OnUpdateRoomPrice(Room room)
        {
            string input = await Application.Current.MainPage.DisplayPromptAsync(
                "Modifica Prezzo",
                $"Inserisci il nuovo prezzo per la stanza \"{room.RoomName}\":",
                placeholder: "Es. 120"
            );

            if (string.IsNullOrWhiteSpace(input) || !double.TryParse(input, out double newPrice))
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Valore non valido", "OK");
                return;
            }

            var token = await SecureStorage.GetAsync("jwt_token");

            // Prepara il payload con RoomID, NewPrice e Username (che verr� aggiunto nel backend)
            var payload = new { RoomID = room.RoomID, NewPrice = newPrice };
            var json = JsonSerializer.Serialize(payload);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://localhost:9000/updateRoomPrice", content);
            if (response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Successo", "Prezzo aggiornato con successo", "OK");
                room.PricePerNight = newPrice;
                // Se la classe Room implementa INotifyPropertyChanged, il binding verr� aggiornato.
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await Application.Current.MainPage.DisplayAlert("Errore", error, "OK");
            }
        }


        public async Task LoadBookingsAsync()
        {
            try
            {
                using var client = new HttpClient();
                var payload = JsonSerializer.Serialize(new { HotelID = this.HotelID });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://localhost:9000/getHotelBookings", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var bookings = JsonSerializer.Deserialize<List<Booking>>(responseJson);
                    if (bookings != null)
                    {
                        Bookings.Clear();
                        foreach (var booking in bookings)
                        {
                            Bookings.Add(booking);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Errore nel caricamento delle prenotazioni: " + response.StatusCode);
                }
               
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Eccezione durante il caricamento delle prenotazioni: " + ex.Message);
            }
        }


    }
}