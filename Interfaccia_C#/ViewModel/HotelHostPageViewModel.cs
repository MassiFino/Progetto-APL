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


        public class Review
        {
            public string ReviewerName { get; set; }
            public string Comment { get; set; }
            public double Rating { get; set; }
        }


        // Nuove proprietà per le stanze e le recensioni
        public ObservableCollection<Room> Rooms { get; set; } = new ObservableCollection<Room>();
        public ObservableCollection<Review> Reviews { get; set; } = new ObservableCollection<Review>();

        // Costruttore che accetta un oggetto Hotel
        public HotelHostPageViewModel(Hotel hotel)
        {
            HotelID = hotel.HotelID;
            Name = hotel.Name;
            Location = hotel.Location;
            Description = hotel.Description;
            Rating = hotel.Rating;
            Services = string.Join(", ", hotel.Services);

            if (!string.IsNullOrEmpty(hotel.Images?.Trim()))
            {
                // Divido la stringa in più immagini
                var imageList = hotel.Images.Split(';');
                // Prendo la prima
                var firstImage = imageList[0].Trim();

                Debug.WriteLine("Path immagine (prima del combine): " + firstImage);

                // Costruisco path completo
                string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                var imagePath = Path.Combine(projectDirectory, firstImage);
                Debug.WriteLine("Path completo: " + imagePath);

                // Converto in ImageSource
                ImageSource = GetImageSource(imagePath);
                Debug.WriteLine("Immaginissima: " + hotel.ImageSource);
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

    }
}