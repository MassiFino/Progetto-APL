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
                }
            }
        }
        // Comando per il bottone "Prenota stanza"
        public ICommand CercaStanzaCommand { get; }
        public class Room
        {
            public int RoomID { get; set; }
            public string RoomName { get; set; }
            public string RoomDescription { get; set; }
            public double PricePerNight { get; set; }
            public int MaxGuests { get; set; }
            public string RoomType { get; set; }
            public string Images { get; set; }
            public ImageSource ImageSource { get; set; }
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

            CercaStanzaCommand = new Command(CercaStanza);

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

            var payload = new
            {
                CheckIn = CheckInDate,
                CheckOut = CheckOutDate
            };

            // Esegui la logica per il payload, ad esempio inviare i dati al server o visualizzare un messaggio
            // Qui possiamo fare qualcosa con il payload, come inviarlo a un servizio, eseguire una prenotazione, etc.
            await Application.Current.MainPage.DisplayAlert("Prenotazione",
                $"Check-in: {payload.CheckIn.ToShortDateString()} - Check-out: {payload.CheckOut.ToShortDateString()}",
                "OK");
        }
        private async Task PrenotaStanza(Room room)
        {
            //Inserire la logica per prenotare le stanza query inserimento
            // Creare un oggetto che rappresenta la prenotazione (ad esempio con le date)
            var payload = new
            {
                Room = room,
                CheckIn = CheckInDate,
                CheckOut = CheckOutDate
            };

            // Qui puoi inviare la prenotazione al server, salvarla localmente, etc.
            // Per il momento, mostriamo solo un messaggio di conferma
            await Application.Current.MainPage.DisplayAlert("Prenotazione",
                $"Hai prenotato la stanza: {room.RoomName}\n" +
                $"Check-in: {payload.CheckIn.ToShortDateString()}\n" +
                $"Check-out: {payload.CheckOut.ToShortDateString()}",
                "OK");
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

    }
}