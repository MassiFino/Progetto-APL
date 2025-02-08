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
        private string _pricePerNight;
        private string _maxGuests;
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

        public string PricePerNight
        {
            get => _pricePerNight;
            set
            {
                if (_pricePerNight != value)
                {
                    _pricePerNight = value;
                    OnPropertyChanged(nameof(PricePerNight));
                }
            }
        }

        public string MaxGuests
        {
            get => _maxGuests;
            set
            {
                if (_maxGuests != value)
                {
                    _maxGuests = value;
                    OnPropertyChanged(nameof(MaxGuests));
                }
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
                RoomName = RoomName,
                RoomDescription = RoomDescription,
                PricePerNight = PricePerNight,
                MaxGuests = MaxGuests,
                RoomType = RoomType,
                RoomImage = RoomImagePath

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
