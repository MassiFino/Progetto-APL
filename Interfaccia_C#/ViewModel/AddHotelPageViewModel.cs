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
using System.Net.Http.Headers;

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
                if (_roomType != value)
                {
                    _roomType = value;

                    // Logica per aggiornare MaxGuests in base alla selezione di RoomType
                    switch (_roomType)
                    {
                        case "Singola":
                            MaxGuests = 1;
                            break;
                        case "Doppia":
                            MaxGuests = 2;
                            break;
                        case "Tripla":
                            MaxGuests = 3;
                            break;
                        case "Quadrupla":
                            MaxGuests = 4;
                            break;
                        case "Suite":
                            MaxGuests = 5;
                            break;
                        case "Deluxe":
                            MaxGuests = 6;
                            break;
                        default:
                            MaxGuests = 0; // O un altro valore di default
                            break;
                    }

                    OnPropertyChanged();
                }
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
                if (_additionalRoomType != value)
                {
                    _additionalRoomType = value;

                    // Logica per aggiornare AdditionalMaxGuests in base alla selezione di AdditionalRoomType
                    switch (_additionalRoomType)
                    {
                        case "Singola":
                            AdditionalMaxGuests = 1;
                            break;
                        case "Doppia":
                            AdditionalMaxGuests = 2;
                            break;
                        case "Tripla":
                            AdditionalMaxGuests = 3;
                            break;
                        case "Quadrupla":
                            AdditionalMaxGuests = 4;
                            break;
                        case "Suite":
                            AdditionalMaxGuests = 5;
                            break;
                        case "Deluxe":
                            AdditionalMaxGuests = 6;
                            break;
                        default:
                            AdditionalMaxGuests = 0; // O un altro valore di default
                            break;
                    }

                    OnPropertyChanged();
                }
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

        private string _additionalRoomImagePath;
        public string AdditionalRoomImagePath
        {
            get => _additionalRoomImagePath;
            set
            {
                _additionalRoomImagePath = value;
                OnPropertyChanged();
            }
        }

        private string _additionalRoomImageNames;
        public string AdditionalRoomImageNames
        {
            get => _additionalRoomImageNames;
            set
            {
                _additionalRoomImageNames = value;
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
        public ICommand UploadAdditionalRoomImageCommand { get; }
        public ICommand AddRoomCommand { get; }

        public AddHotelPageViewModel()
        {
            AddHotelAndRoomCommand = new Command(async () => await OnAddHotelAndRoom());
            ShowAddRoomCommand = new Command(OnShowAddRoom);
            UploadHotelImageCommand = new Command(async () => await OnUploadHotelImage());
            UploadRoomImageCommand = new Command(async () => await OnUploadRoomImage());
            AddRoomCommand = new Command(async () => await OnAddSecondRoom());
            UploadAdditionalRoomImageCommand = new Command(async () => await OnUploadAdditionalRoomImage());
        }
        private async Task OnUploadHotelImage()
        {
            try
            {
                List<string> imagePaths = new List<string>();
                List<string> imageNames = new List<string>();
                int maxImages = 5;

                while (imagePaths.Count < maxImages)
                {
                    var result = await FilePicker.PickAsync(new PickOptions
                    {
                        PickerTitle = $"Seleziona un'immagine ({imagePaths.Count}/{maxImages})",
                        FileTypes = FilePickerFileType.Images
                    });

                    if (result != null)
                    {
                        // Aggiungi l'immagine selezionata alla lista
                        imagePaths.Add(result.FullPath);
                        imageNames.Add(result.FileName);
                    }
                    else
                    {
                        break; // L'utente ha annullato
                    }

                    // Se ha raggiunto il massimo, mostra un avviso
                    if (imagePaths.Count == maxImages)
                    {
                        await Application.Current.MainPage.DisplayAlert("Limite raggiunto",
                            "Hai selezionato il numero massimo di 5 immagini.", "OK");
                    }
                }

                // Se ci sono immagini selezionate, chiedi conferma
                if (imagePaths.Any())
                {
                    // Mostra anteprima delle immagini
                    string previewMessage = "Immagini selezionate:\n" + string.Join("\n", imageNames);
                    bool confirm = await Application.Current.MainPage.DisplayAlert(
                        "Conferma Selezione",
                        previewMessage + "\n\nVai avanti con il salvataggio?",
                        "Sì",
                        "Riprova");

                    if (confirm)
                    {
                        // Salva le immagini localmente
                        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                        var localPath = Path.Combine(projectDirectory, "Pictures", "HotelPictures");
                        Directory.CreateDirectory(localPath);

                        foreach (var result in imagePaths)
                        {
                            var fileName = Path.GetFileName(result);
                            var filePath = Path.Combine(localPath, fileName);
                            var Pat = Path.Combine("Pictures", "HotelPictures", fileName);

                            // Aggiungi il percorso relativo a HotelImagePath, separato da ";" se ci sono più immagini
                            if (!string.IsNullOrEmpty(HotelImagePath))
                            {
                                HotelImagePath += ";"; // Aggiungi un separatore se c'è già un percorso
                            }
                            HotelImagePath += Pat; // Concatenazione del percorso relativo
                            using (var stream = File.OpenRead(result))
                            using (var localFile = File.Create(filePath))
                            {
                                await stream.CopyToAsync(localFile);
                            }
                        }

                        HotelImageNames = $"Immagini caricate: {string.Join(", ", imageNames)}";
                    }
                    else
                    {
                        // Se l'utente rifiuta, consenti una nuova selezione
                        await Application.Current.MainPage.DisplayAlert("Riprova", "Puoi selezionare nuovamente le immagini.", "OK");
                        imagePaths.Clear();
                        imageNames.Clear();
                        HotelImagePath = string.Empty;
                        await OnUploadHotelImage();  // Chiama di nuovo il metodo per selezionare le immagini
                    }
                }
                else
                {
                    HotelImageNames = "Nessuna immagine selezionata.";
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
            var (imagePath, imageName) = await UploadImageAsync("RoomPictures", "Seleziona un'immagine per la stanza");
            if (imagePath != null)
            {
                RoomImagePath = imagePath;
                RoomImageNames = $"Immagine caricata: {imageName}";
            }
            else
            {
                RoomImagePath = null;
                RoomImageNames = $"Errore: {imageName}";
            }
        }
        public string ImageHotel;

        private async Task OnAddHotelAndRoom()
        {// Controllo dei campi obbligatori
            if (string.IsNullOrWhiteSpace(HotelName) || string.IsNullOrWhiteSpace(Location) || string.IsNullOrWhiteSpace(RoomName) ||
                string.IsNullOrWhiteSpace(RoomDescription) || PricePerNight <= 0 ||  MaxGuests <= 0 ||
        string.IsNullOrWhiteSpace(RoomType))  // Verifica che RoomType non sia null o vuoto
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

            var token = await SecureStorage.GetAsync("jwt_token");

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
            //Manca User Host
            Debug.WriteLine($"HotelName: {HotelName}");
            Debug.WriteLine($"Location: {Location}");
            Debug.WriteLine($"Description: {Description}");
            Debug.WriteLine($"Immagini dell'hotel: {ImageHotel}");

            Debug.WriteLine($"Services: {string.Join(", ", activeServices)}");
            Debug.WriteLine($"RoomName: {RoomName}");
            Debug.WriteLine($"RoomDescription: {RoomDescription}");
            Debug.WriteLine($"PricePerNight: {PricePerNight}");
            Debug.WriteLine($"MaxGuests: {MaxGuests}");
            Debug.WriteLine($"RoomType: {RoomType}");

            Debug.WriteLine($"Stampo tutto il paylod: {payload}");
           
            try
            {
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/addRoomHotel");
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var json = JsonSerializer.Serialize(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string previewMessage = "Stanza e hotel aggiunto con successo";

                    bool confirm = await Application.Current.MainPage.DisplayAlert(
                        "Conferma Selezione",
                        previewMessage + "\n\nVuoi inserire un'altra stanza?",
                        "Sì",
                        "No");
                    if (confirm)
                    {
                        IsHotelSaved = true;
                        IsSuccessVisible = true;
                        IsErrorVisible = false;
                        IsAddHotelVisible = false;
                        IsAddRoomVisible = true;
                        // Svuota tutti i campi
                        HotelName = string.Empty;
                        Location = string.Empty;
                        Description = string.Empty;
                        ImageHotel = null; // Supponendo che ImageHotel sia un oggetto o un percorso dell'immagine
                        HotelImageNames = string.Empty;
                    }
                    else
                    {
                        // Svuota tutti i campi
                        HotelName = string.Empty;
                        Location = string.Empty;
                        Description = string.Empty;
                        ImageHotel = null; // Supponendo che ImageHotel sia un oggetto o un percorso dell'immagine
                        HotelImageNames = string.Empty;
                        activeServices.Clear(); // Svuota la lista dei servizi
                        RoomName = string.Empty;
                        RoomDescription = string.Empty;
                        PricePerNight = 0; // Imposta un valore predefinito per il prezzo
                        MaxGuests = 0; // Imposta il valore predefinito per il numero di ospiti
                        RoomType = string.Empty;
                        RoomImageNames= string.Empty;


                        // Mostra un altro messaggio
                        await Application.Current.MainPage.DisplayAlert(
                            "Nuovo Hotel",
                            "Se vuoi, puoi inserire un nuovo hotel.",
                            "OK");

                        // Reset delle visibilità per la schermata dell'hotel
                        IsHotelSaved = false;
                        IsSuccessVisible = false;
                        IsErrorVisible = false;
                        IsAddHotelVisible = true;
                        IsAddRoomVisible = false;
                    }
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
            if (string.IsNullOrWhiteSpace(AdditionalRoomName) ||
                string.IsNullOrWhiteSpace(AdditionalRoomDescription) ||
                AdditionalPricePerNight <= 0 ||
                AdditionalMaxGuests <= 0     || string.IsNullOrWhiteSpace(AdditionalRoomType))  // Verifica che RoomType non sia null o vuoto

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
                RoomType = this.AdditionalRoomType,
                RoomImagePath = this.RoomImagePath // Qui era mancante l'assegnazione corretta
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

                    string previewMessage = "Stanza e hotel aggiunto con successo";
                    bool confirm = await Application.Current.MainPage.DisplayAlert(
                        "Conferma Selezione",
                        previewMessage + "\n\nVuoi inserire un'altra stanza?",
                        "Sì",
                        "No");

                    if (confirm)
                    {
                        // Svuotamento dei campi
                        AdditionalRoomName = string.Empty;
                        AdditionalRoomDescription = string.Empty;
                        AdditionalPricePerNight = 0;
                        AdditionalMaxGuests = 0;
                        AdditionalRoomType = string.Empty;
                        RoomImagePath = string.Empty;
                        AdditionalRoomImageNames = string.Empty;

                    }
                    else
                    {
                        AdditionalRoomName = string.Empty;
                        AdditionalRoomDescription = string.Empty;
                        AdditionalPricePerNight = 0;
                        AdditionalMaxGuests = 0;
                        AdditionalRoomType = string.Empty;
                        RoomImagePath = string.Empty;
                        AdditionalRoomImageNames = string.Empty;
                        // Reset delle visibilità per la schermata dell'hotel
                        IsHotelSaved = false;
                        IsSuccessVisible = false;
                        IsErrorVisible = false;
                        IsAddHotelVisible = true;
                        IsAddRoomVisible = false;
                    }
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


        private async Task<(string imagePath, string imageName)> UploadImageAsync(string folder, string pickerTitle)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = pickerTitle,
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    // Calcola il percorso del progetto (modifica in base alla tua struttura)
                    string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                    var localPath = Path.Combine(projectDirectory, "Pictures", folder);
                    Directory.CreateDirectory(localPath);

                    var filePath = Path.Combine(localPath, result.FileName);
                    var relativePath = Path.Combine("Pictures", folder, result.FileName);

                    using (var stream = await result.OpenReadAsync())
                    using (var localFile = File.Create(filePath))
                    {
                        await stream.CopyToAsync(localFile);
                    }

                    return (relativePath, result.FileName);
                }
            }
            catch (Exception ex)
            {
                // In caso di errore, restituisci un messaggio nell'imageName
                return (null, $"Errore: {ex.Message}");
            }
            return (null, null);
        }


        private async Task OnUploadAdditionalRoomImage()
        {
            var (imagePath, imageName) = await UploadImageAsync("RoomPictures", "Seleziona un'immagine per la seconda stanza");
            if (imagePath != null)
            {
                AdditionalRoomImagePath = imagePath;
                AdditionalRoomImageNames = $"Immagine caricata: {imageName}";
            }
            else
            {
                AdditionalRoomImagePath = null;
                AdditionalRoomImageNames = $"Errore: {imageName}";
            }
        }

    }
}
