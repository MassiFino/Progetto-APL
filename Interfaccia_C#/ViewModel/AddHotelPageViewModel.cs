using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using Microsoft.Maui.Storage;
using System.Text.Json;
using System.Diagnostics;

namespace Interfaccia_C_.ViewModel
{
    public class AddHotelPageViewModel : BindableObject
    {
        // Proprietà per l'hotel
        private string _hotelName;
        private string _location;
        private string _description;
        private bool _isWiFiEnabled;
        private bool _isParkingEnabled;
        private bool _isBreakfastEnabled;
        private bool _isPoolEnabled;
        private bool _isGymEnabled;
        private bool _isSpaEnabled;
        private bool _isRoomServiceEnabled;
        private bool _isPetsAllowed;
        private bool _isRestaurantEnabled;
        private bool _isAirConditioningEnabled;
   
        // Proprietà per la stanza iniziale
        private string _roomName;
        private string _roomDescription;
        private double _pricePerNight;
        private int _maxGuests;
        private string _roomType;

        // Proprietà per la seconda stanza
        private string _additionalRoomName;
        private string _additionalRoomDescription;
        private double _additionalPricePerNight;
        private int _additionalMaxGuests;
        private string _additionalRoomType;

        // Proprietà per la visibilità delle sezioni
        private bool _isAddHotelVisible = true;
        private bool _isAddRoomVisible = false;

        // Proprietà per la gestione immagini
        private string _hotelImagePath;
        private string _roomImagePath;
        private string _hotelImageNames;
        private string _roomImageNames;

        // Proprietà per la visibilità dei messaggi di successo/errore
        private bool _isErrorVisible;
        private bool _isSuccessVisible;

        public bool IsAddHotelVisible
        {
            get => _isAddHotelVisible;
            set
            {
                if (_isAddHotelVisible != value)
                {
                    _isAddHotelVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isHotelSaved;
        public bool IsHotelSaved
        {
            get => _isHotelSaved;
            set
            {
                if (_isHotelSaved != value)
                {
                    _isHotelSaved = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsAddRoomVisible
        {
            get => _isAddRoomVisible;
            set
            {
                if (_isAddRoomVisible != value)
                {
                    _isAddRoomVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public string HotelName
        {
            get => _hotelName;
            set
            {
                _hotelName = value;
                OnPropertyChanged();
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public bool IsWiFiEnabled
        {
            get => _isWiFiEnabled;
            set
            {
                _isWiFiEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsParkingEnabled
        {
            get => _isParkingEnabled;
            set
            {
                _isParkingEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsBreakfastEnabled
        {
            get => _isBreakfastEnabled;
            set
            {
                _isBreakfastEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsPoolEnabled
        {
            get => _isPoolEnabled;
            set
            {
                _isPoolEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsGymEnabled
        {
            get => _isGymEnabled;
            set
            {
                _isGymEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsSpaEnabled
        {
            get => _isSpaEnabled;
            set
            {
                _isSpaEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsRoomServiceEnabled
        {
            get => _isRoomServiceEnabled;
            set
            {
                _isRoomServiceEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsPetsAllowed
        {
            get => _isPetsAllowed;
            set
            {
                _isPetsAllowed = value;
                OnPropertyChanged();
            }
        }

        public bool IsRestaurantEnabled
        {
            get => _isRestaurantEnabled;
            set
            {
                _isRestaurantEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsAirConditioningEnabled
        {
            get => _isAirConditioningEnabled;
            set
            {
                _isAirConditioningEnabled = value;
                OnPropertyChanged();
            }
        }

        // Proprietà per la prima stanza
        public string RoomName
        {
            get => _roomName;
            set
            {
                _roomName = value;
                OnPropertyChanged();
            }
        }

        public string RoomDescription
        {
            get => _roomDescription;
            set
            {
                _roomDescription = value;
                OnPropertyChanged();
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
                _roomType = value;
                OnPropertyChanged();
            }
        }

        // Proprietà per la seconda stanza
        public string AdditionalRoomName
        {
            get => _additionalRoomName;
            set
            {
                _additionalRoomName = value;
                OnPropertyChanged();
            }
        }

        public string AdditionalRoomDescription
        {
            get => _additionalRoomDescription;
            set
            {
                _additionalRoomDescription = value;
                OnPropertyChanged();
            }
        }

        public double AdditionalPricePerNight
        {
            get => _additionalPricePerNight;
            set
            {
                _additionalPricePerNight = value;
                OnPropertyChanged();
            }
        }

        public int AdditionalMaxGuests
        {
            get => _additionalMaxGuests;
            set
            {
                _additionalMaxGuests = value;
                OnPropertyChanged();
            }
        }

        public string AdditionalRoomType
        {
            get => _additionalRoomType;
            set
            {
                _additionalRoomType = value;
                OnPropertyChanged();
            }
        }

        public string HotelImagePath
        {
            get => _hotelImagePath;
            set
            {
                _hotelImagePath = value;
                OnPropertyChanged();
            }
        }

        public string RoomImagePath
        {
            get => _roomImagePath;
            set
            {
                _roomImagePath = value;
                OnPropertyChanged();
            }
        }

        public string HotelImageNames
        {
            get => _hotelImageNames;
            set
            {
                _hotelImageNames = value;
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

        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set
            {
                _isErrorVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsSuccessVisible
        {
            get => _isSuccessVisible;
            set
            {
                _isSuccessVisible = value;
                OnPropertyChanged();
            }
        }

        // Comandi
        public ICommand AddHotelAndRoomCommand { get; }
        public ICommand ShowAddRoomCommand { get; }
        public ICommand UploadHotelImageCommand { get; }
        public ICommand UploadRoomImageCommand { get; }
        public ICommand AddRoomCommand { get; }

        public AddHotelPageViewModel()
        {
            AddHotelAndRoomCommand = new Command(async () => await OnAddHotelAndRoom());
            ShowAddRoomCommand = new Command(OnShowAddRoom);
            UploadHotelImageCommand = new Command(async () => await OnUploadHotelImage());
            UploadRoomImageCommand = new Command(async () => await OnUploadRoomImage());
            AddRoomCommand = new Command(async () => await OnAddSecondRoom());
        }

        private async Task OnUploadHotelImage()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Seleziona un'immagine per l'hotel",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                    var localPath = Path.Combine(projectDirectory, "Pictures", "HotelPictures");
                    Directory.CreateDirectory(localPath);

                    var filePath = Path.Combine(localPath, result.FileName);
                    var relativePath = Path.Combine("Pictures", "HotelPictures", result.FileName);

                    using (var stream = await result.OpenReadAsync())
                    using (var localFile = File.Create(filePath))
                    {
                        await stream.CopyToAsync(localFile);
                    }

                    HotelImagePath = relativePath;
                    HotelImageNames = $"Immagine caricata: {result.FileName}";
                }
            }
            catch (Exception ex)
            {
                HotelImagePath = null;
                HotelImageNames = $"Errore: {ex.Message}";
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


        private async Task OnAddHotelAndRoom()
        {// Controllo dei campi obbligatori
            if (string.IsNullOrWhiteSpace(HotelName) || string.IsNullOrWhiteSpace(Location) || string.IsNullOrWhiteSpace(RoomName) ||
                string.IsNullOrWhiteSpace(RoomDescription) || PricePerNight <= 0 || MaxGuests <= 0)
            {
                // Mostra un avviso all'utente se uno dei campi obbligatori è vuoto
                await Application.Current.MainPage.DisplayAlert("Attenzione", "Attenzione, non ha inserito tutti i campi richiesti.", "OK");

                // Impostiamo l'indicatore di errore e nascondiamo il successo
                IsErrorVisible = true;
                IsSuccessVisible = false;
                OnPropertyChanged(nameof(IsErrorVisible));
                return;
            }

            var activeServices = GetActiveServices();

            var payload = new
            {
                HotelName = this.HotelName,
                Location = this.Location,
                Description = this.Description,
                Services = activeServices,
                HotelImagePath,
                RoomName = this.RoomName,
                RoomDescription = this.RoomDescription,
                PricePerNight = this.PricePerNight,
                MaxGuests = this.MaxGuests,
                RoomType = this.RoomType,
                RoomImagePath
            };
            // Debug: stampiamo i valori finali
            Debug.WriteLine($"HotelName: {HotelName}");
            Debug.WriteLine($"Location: {Location}");
            Debug.WriteLine($"Description: {Description}");
            Debug.WriteLine($"Services: {string.Join(", ", activeServices)}");
            Debug.WriteLine($"RoomName: {RoomName}");
            Debug.WriteLine($"RoomDescription: {RoomDescription}");
            Debug.WriteLine($"PricePerNight: {PricePerNight}");
            Debug.WriteLine($"MaxGuests: {MaxGuests}");
            Debug.WriteLine($"RoomType: {RoomType}");
            Debug.WriteLine($"Hote: {HotelImagePath}");

            Debug.WriteLine($"Stampo tutto il paylod: {payload}");
           
            try
            {
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/addRoomHotel");
                var json = JsonSerializer.Serialize(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    IsHotelSaved = true;

                    IsSuccessVisible = true;
                    IsErrorVisible = false;
                    IsAddHotelVisible = false;
                    IsAddRoomVisible = true;
                }
                else
                {
                    IsHotelSaved = false;

                    IsSuccessVisible = false;
                    IsErrorVisible = true;
                }

                OnPropertyChanged(nameof(IsSuccessVisible));
                OnPropertyChanged(nameof(IsErrorVisible));
                OnPropertyChanged(nameof(IsAddHotelVisible));
                OnPropertyChanged(nameof(IsAddRoomVisible));
            }
            catch (Exception)
            {
                IsSuccessVisible = false;
                IsErrorVisible = true;
                OnPropertyChanged(nameof(IsSuccessVisible));
                OnPropertyChanged(nameof(IsErrorVisible));
            }
        }

        private async Task OnAddSecondRoom()
        {
            if (string.IsNullOrWhiteSpace(AdditionalRoomName) || string.IsNullOrWhiteSpace(AdditionalRoomDescription) ||
                AdditionalPricePerNight <= 0 || AdditionalMaxGuests <= 0)
            {
                IsErrorVisible = true;
                IsSuccessVisible = false;
                return;
            }

            var payload = new
            {
                HotelName = this.HotelName,
                RoomName = this.AdditionalRoomName,
                RoomDescription = this.AdditionalRoomDescription,
                PricePerNight = this.AdditionalPricePerNight,
                MaxGuests = this.AdditionalMaxGuests,
                RoomType = this.AdditionalRoomType
            };

            try
            {
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/addRoom");
                var json = JsonSerializer.Serialize(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    IsSuccessVisible = true;
                    IsErrorVisible = false;
                }
                else
                {
                    IsSuccessVisible = false;
                    IsErrorVisible = true;
                }

                OnPropertyChanged(nameof(IsSuccessVisible));
                OnPropertyChanged(nameof(IsErrorVisible));
            }
            catch (Exception)
            {
                IsSuccessVisible = false;
                IsErrorVisible = true;
            }
        }

        private List<string> GetActiveServices()
        {
            List<string> activeServices = new List<string>();
            if (IsWiFiEnabled) activeServices.Add("Free Wi-Fi");
            if (IsParkingEnabled) activeServices.Add("Free Parking");
            if (IsBreakfastEnabled) activeServices.Add("Free Breakfast");
            if (IsPoolEnabled) activeServices.Add("Pool");
            if (IsGymEnabled) activeServices.Add("Gym");
            if (IsSpaEnabled) activeServices.Add("Spa");
            if (IsRoomServiceEnabled) activeServices.Add("Room Service");
            if (IsPetsAllowed) activeServices.Add("Pets Allowed");
            if (IsRestaurantEnabled) activeServices.Add("Restaurant");
            if (IsAirConditioningEnabled) activeServices.Add("Air Conditioning");

            return activeServices;
        }

        private void OnShowAddRoom()
        {
            IsAddHotelVisible = false;
            IsAddRoomVisible = true;
        }
    }
}
