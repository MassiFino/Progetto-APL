using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Collections.ObjectModel;
using Interfaccia_C_.Model;

namespace Interfaccia_C_.ViewModel
{
    public class SearchPageViewModel : INotifyPropertyChanged
    {
        private string _searchCity;
        private DateTime _checkInDate = DateTime.Today;
        private DateTime _checkOutDate = DateTime.Today.AddDays(1);
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
        private string message;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Proprietà per visualizzare i risultati della ricerca
        private ObservableCollection<Hotel> _researchHotels;
        public ObservableCollection<Hotel> ResearchHotels
        {
            get => _researchHotels;
            set
            {
                _researchHotels = value;
                OnPropertyChanged();
            }
        }

        public string SearchCity
        {
            get => _searchCity;
            set { _searchCity = value; OnPropertyChanged(); }
        }

        public DateTime CheckInDate
        {
            get => _checkInDate;
            set
            {
                if (_checkInDate != value)
                {
                    _checkInDate = value;
                    OnPropertyChanged();
                    ValidateDates();
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
                    OnPropertyChanged();
                    ValidateDates();
                }
            }
        }

        // Altre proprietà (switch, Picker, ecc.)...
        public bool IsWiFiEnabled { get => _isWiFiEnabled; set { _isWiFiEnabled = value; OnPropertyChanged(); } }
        public bool IsParkingEnabled { get => _isParkingEnabled; set { _isParkingEnabled = value; OnPropertyChanged(); } }
        public bool IsBreakfastEnabled { get => _isBreakfastEnabled; set { _isBreakfastEnabled = value; OnPropertyChanged(); } }
        public bool IsPoolEnabled { get => _isPoolEnabled; set { _isPoolEnabled = value; OnPropertyChanged(); } }
        public bool IsGymEnabled { get => _isGymEnabled; set { _isGymEnabled = value; OnPropertyChanged(); } }
        public bool IsSpaEnabled { get => _isSpaEnabled; set { _isSpaEnabled = value; OnPropertyChanged(); } }
        public bool IsRoomServiceEnabled { get => _isRoomServiceEnabled; set { _isRoomServiceEnabled = value; OnPropertyChanged(); } }
        public bool IsPetsAllowed { get => _isPetsAllowed; set { _isPetsAllowed = value; OnPropertyChanged(); } }
        public bool IsRestaurantEnabled { get => _isRestaurantEnabled; set { _isRestaurantEnabled = value; OnPropertyChanged(); } }
        public bool IsAirConditioningEnabled { get => _isAirConditioningEnabled; set { _isAirConditioningEnabled = value; OnPropertyChanged(); } }

        private string _selectedGuest;
        public string SelectedGuest
        {
            get => _selectedGuest;
            set { _selectedGuest = value; OnPropertyChanged(); }
        }

        private string _selectedOrderBy = "Order by";
        public string SelectedOrderBy
        {
            get => _selectedOrderBy;
            set { _selectedOrderBy = value; OnPropertyChanged(); }
        }

        public string Message
        {
            get => message;
            set { message = value; OnPropertyChanged(nameof(Message)); }
        }

        public ICommand SearchCommand { get; }
        public ICommand ViewHotelCommand { get; set; }

        public SearchPageViewModel()
        {
            ResearchHotels = new ObservableCollection<Hotel>();
            SearchCommand = new Command(async () => await ExecuteSearch());
            ViewHotelCommand = new Command<Hotel>(async (hotelSelezionato) =>
            {
                var navParams = new Dictionary<string, object>
                {
                    ["hotel"] = hotelSelezionato
                };
                await Shell.Current.GoToAsync("HotelPage", navParams);
            });
        }

        // Metodo per ottenere i servizi attivi in base agli switch
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

        private async Task ExecuteSearch()
        {
            // Verifica che siano state inserite le informazioni di base
            if (string.IsNullOrWhiteSpace(SearchCity))
            {
                await Application.Current.MainPage.DisplayAlert("Attenzione", "Inserisci la città", "OK");
                return;
            }
            if (!string.IsNullOrEmpty(Message))
            {
                await Application.Current.MainPage.DisplayAlert("Errore", Message, "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(SelectedGuest))
            {
                await Application.Current.MainPage.DisplayAlert("Attenzione", "Seleziona il numero di ospiti", "OK");
                return;
            }
            int guests;
            if (SelectedGuest == "5+")
            {
                guests = 5; 
            }
            else if (!int.TryParse(SelectedGuest, out guests))
            {
                await Application.Current.MainPage.DisplayAlert("Attenzione", "Seleziona un numero valido di ospiti", "OK");
                return;
            }

            List<string> activeServices = GetActiveServices();
            Debug.WriteLine($"Ricerca per: {SearchCity}, check-in: {CheckInDate:dd/MM/yyyy}, check-out: {CheckOutDate:dd/MM/yyyy}");
            var searchParameters = new Dictionary<string, object>
            {
                { "City", SearchCity },
                { "CheckInDate", CheckInDate },
                { "CheckOutDate", CheckOutDate },
                { "Guests", guests },
                { "Services", activeServices }
            };
            Debug.WriteLine($"Services: {string.Join(", ", activeServices)}");
            if (!string.IsNullOrWhiteSpace(SelectedOrderBy) && SelectedOrderBy != "Order by")
            {
                searchParameters.Add("OrderBy", SelectedOrderBy);
            }

            var json = JsonSerializer.Serialize(searchParameters);
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("http://localhost:9000/searchHotels", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var resultJson = await response.Content.ReadAsStringAsync();
                        var hotels = JsonSerializer.Deserialize<List<Hotel>>(resultJson);
                        ResearchHotels.Clear();
                        if (hotels != null && hotels.Count > 0)
                        {
                            foreach (var hotel in hotels)
                            {
                                hotel.ServiziStringa = hotel.Services != null ? string.Join(", ", hotel.Services) : "";
                                // Gestione delle immagini (come nel tuo codice precedente)
                                if (!string.IsNullOrEmpty(hotel.Images?.Trim()))
                                {
                                    var imageList = hotel.Images.Split(';');
                                    var firstImage = imageList[0].Trim();
                                    string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                                    var imagePath = System.IO.Path.Combine(projectDirectory, firstImage);
                                    hotel.ImageSource = GetImageSource(imagePath);
                                }
                                ResearchHotels.Add(hotel);
                            }
                        }
                        else
                        {
                            Message = "Non ho trovato hotel per le date selezionate";
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Errore nella ricerca: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Eccezione durante la ricerca: " + ex.Message);
            }
        }

        public ImageSource GetImageSource(string imagePath)
        {
            if (System.IO.File.Exists(imagePath))
            {
                Debug.WriteLine("Immagine trovata: " + imagePath);
                return ImageSource.FromFile(imagePath);
            }
            else
            {
                Debug.WriteLine("Immagine non trovata: " + imagePath);
                return null;
            }
        }

        // Metodo per validare le date
        private void ValidateDates()
        {
            if (CheckInDate < DateTime.Today)
            {
                Message = "La data di check-in non può essere nel passato.";
            }
            else if (CheckOutDate <= CheckInDate)
            {
                Message = "La data di check-out deve essere successiva a quella di check-in.";
            }
            else
            {
                Message = string.Empty;
            }
        }
    }
}
