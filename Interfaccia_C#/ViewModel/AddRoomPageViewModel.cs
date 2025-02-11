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



namespace Interfaccia_C_.ViewModel
{
    public class AddRoomPageViewModel
    {
        public string Name { get; }
        public string Location { get; }
        private string _roomImagePath;
        private string _roomImageNames;

        public string RoomImagePath
        {
            get => _roomImagePath;
            set
            {
                _roomImagePath = value;
                OnPropertyChanged();
            }
        }
        public string RoomImageNames
        {
            get => _roomImageNames;
            set
            {
                _roomImageNames = value;
                OnPropertyChanged();
            }
        }
        private string _roomName;
        private string _roomDescription;
        private double _pricePerNight;
        private int _maxGuests;
        private string _roomType;
        private bool _isRoomImageUploaded;

     
        public string RoomName
        {
            get => _roomName;
            set
            {
                if (_roomName != value)
                {
                    _roomName = value;
                    OnPropertyChanged(nameof(RoomName));
                }
            }
        }

        public string RoomDescription
        {
            get => _roomDescription;
            set
            {
                if (_roomDescription != value)
                {
                    _roomDescription = value;
                    OnPropertyChanged(nameof(RoomDescription));
                }
            }
        }

        public double PricePerNight
        {
            get => _pricePerNight;
            set
            {
                _pricePerNight = value;
                OnPropertyChanged();
            }
        }


        public int MaxGuests
        {
            get => _maxGuests;
            set
            {
                _maxGuests = value;
                OnPropertyChanged();
            }
        }

        public string RoomType
        {
            get => _roomType;
            set
            {
                if (_roomType != value)
                {
                    _roomType = value;
                    OnPropertyChanged(nameof(RoomType));
                }
            }
        }

        public bool IsRoomImageUploaded
        {
            get => _isRoomImageUploaded;
            set
            {
                if (_isRoomImageUploaded != value)
                {
                    _isRoomImageUploaded = value;
                    OnPropertyChanged(nameof(IsRoomImageUploaded));
                }
            }
        }

      

        public string ServiziStringa { get; set; }
        public ICommand AddRoomCommand { get; set; }
        public ICommand UploadRoomImageCommand { get; }

        public AddRoomPageViewModel(Hotel hotel)
        {
            Name = hotel.Name;
            Location = hotel.Location;
            AddRoomCommand = new Command(async () => await AddRoom());
            UploadRoomImageCommand = new Command(async () => await OnUploadRoomImage());

        }


        private async Task AddRoom()
        {
            var payload = new
            {
                HotelName = Name,
                RoomName = RoomName,
                RoomDescription = RoomDescription,
                PricePerNight = PricePerNight,
                MaxGuests = MaxGuests,
                RoomType = RoomType,
                RoomImagePath

            };
            // Converte il payload in JSON per migliorare la leggibilità
            var json = JsonSerializer.Serialize(payload);

            // Stampa il payload nella console di debug
            Debug.WriteLine("=== PAYLOAD DELLA STANZA ===");
            Debug.WriteLine(json);

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/addRoom");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
               // Svuota i campi
                RoomName = string.Empty;            // Svuota il nome della stanza
                RoomDescription = string.Empty;     // Svuota la descrizione della stanza
                PricePerNight = 0;                  // Svuota il prezzo per notte (imposta a 0)
                MaxGuests = 0;                           // Svuota il numero massimo di ospiti (imposta a 0)
                RoomType = string.Empty;            // Svuota la tipologia di stanza
                RoomImagePath = string.Empty;       // Svuota il percorso dell'immagine
                RoomImageNames= string.Empty;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Errore", errorContent, "OK");
            }
        }

        private async Task OnUploadRoomImage()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Seleziona un'immagine per la stanza",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                    var localPath = Path.Combine(projectDirectory, "Pictures", "RoomPictures");
                    Directory.CreateDirectory(localPath);

                    var filePath = Path.Combine(localPath, result.FileName);
                    var relativePath = Path.Combine("Pictures", "RoomPictures", result.FileName);

                    using (var stream = await result.OpenReadAsync())
                    using (var localFile = File.Create(filePath))
                    {
                        await stream.CopyToAsync(localFile);
                    }

                    RoomImagePath = relativePath;
                    RoomImageNames = $"Immagine caricata: {result.FileName}";
                }
            }
            catch (Exception ex)
            {
                RoomImagePath = null;
                RoomImageNames = $"Errore: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
