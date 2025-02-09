using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;  // Per simulare chiamate asincrone
using System.Diagnostics;     // Per il debug
using System.Text.Json;
using System.Text;
using System.Collections.ObjectModel;
using Interfaccia_C_.Model;  // Per usare la classe Hotel


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
            set
            {
                _searchCity = value;
                OnPropertyChanged();
            }
        }
        public DateTime CheckInDate
        {
            get => _checkInDate;
            set
            {
                _checkInDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                _checkOutDate = value;
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
        private string _selectedGuest;

        // Proprietà per il valore selezionato
        public string SelectedGuest
        {
            get => _selectedGuest;
            set
            {
                _selectedGuest = value;
                OnPropertyChanged();
            }
        }

        private string _selectedOrderBy = "Order by"; // Valore di default
        public string SelectedOrderBy
        {
            get => _selectedOrderBy;
            set
            {
                _selectedOrderBy = value;
                OnPropertyChanged();
            }
        }

        // Comando per eseguire la ricerca
        public ICommand SearchCommand { get; }

        public ICommand ViewHotelCommand { get; set; }

        public SearchPageViewModel()
        {
            ResearchHotels = new ObservableCollection<Hotel>();

            SearchCommand = new Command(async () => await ExecuteSearch());

            ViewHotelCommand = new Command<Hotel>(async (hotelSelezionato) =>
            {
                // Naviga con Shell, passando l'oggetto hotel
                var navParams = new Dictionary<string, object>
                {
                    ["hotel"] = hotelSelezionato
                };

                await Shell.Current.GoToAsync("HotelPage", navParams);
            });
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
        // Metodo che simula una ricerca
        private async Task ExecuteSearch()
        {
            // Simuliamo un piccolo ritardo per vedere che il comando funziona
            var activeServices = GetActiveServices();

            // Stampa i parametri di ricerca nella console (puoi sostituirlo con la tua logica)
            Debug.WriteLine($"🔍 Ricerca avviata con i seguenti parametri:");
            Debug.WriteLine($"🔍 Ricerca avviata per la città: {SearchCity}");
            Debug.WriteLine($"Numero di ospiti: {SelectedGuest}");
            Debug.WriteLine($"Check-in: {CheckInDate:dd/MM/yyyy}");
            Debug.WriteLine($"Check-out: {CheckOutDate:dd/MM/yyyy}");
            Debug.WriteLine($"Wi-Fi: {IsWiFiEnabled}");
            Debug.WriteLine($"Parcheggio: {IsParkingEnabled}");
            Debug.WriteLine($"Colazione: {IsBreakfastEnabled}");
            Debug.WriteLine($"Piscina: {IsPoolEnabled}");
            Debug.WriteLine($"Palestra: {IsGymEnabled}");
            Debug.WriteLine($"Spa: {IsSpaEnabled}");
            Debug.WriteLine($"Servizio in Camera: {IsRoomServiceEnabled}");
            Debug.WriteLine($"Animali Ammessi: {IsPetsAllowed}");
            Debug.WriteLine($"Ristorante: {IsRestaurantEnabled}");
            Debug.WriteLine($"Aria Condizionata: {IsAirConditioningEnabled}");

            // Qui potresti fare una chiamata a un'API per ottenere i risultati


            var searchParameters = new Dictionary<string, object>
            {
                { "City", this.SearchCity },
                { "CheckInDate", this.CheckInDate },
                { "CheckOutDate", this.CheckOutDate },
                { "Guests", this.SelectedGuest },
                { "Services", activeServices }
            };

            if (!string.IsNullOrWhiteSpace(SelectedOrderBy) && SelectedOrderBy != "Order by")
            {
                searchParameters.Add("OrderBy", SelectedOrderBy);
            }

            // Serializza i parametri in JSON
            var json = JsonSerializer.Serialize(searchParameters);

            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    // Sostituisci con l'URL del tuo endpoint Python
                    var response = await client.PostAsync("http://localhost:9000/searchHotels", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Leggi e gestisci i risultati della ricerca (ad esempio, deserializza il JSON di risposta)
                        var resultJson = await response.Content.ReadAsStringAsync();

                        var hotels = JsonSerializer.Deserialize<List<Hotel>>(resultJson);

                        Debug.WriteLine("Risultati ricerca: " + resultJson);
                        // Qui potresti aggiornare una proprietà come OwnedHotels nel ViewModel
                        if (hotels != null)
                        {
                            // Pulisce la collezione esistente
                            ResearchHotels.Clear();
                            foreach (var hotel in hotels)
                            {
                                // Se la lista dei servizi è null, la inizializzi
                                if (hotel.Services == null)
                                    hotel.Services = new List<string>();

                                // Debug per vedere i servizi
                                foreach (var service in hotel.Services)
                                {
                                    Debug.WriteLine($"Servizio dell'hotel: {service}");
                                }
                                hotel.ServiziStringa = string.Join(", ", hotel.Services);
                                Debug.WriteLine($"Servizi tutti : {hotel.ServiziStringa}");

                                // Gestione delle immagini:
                                // Se il campo Images non è vuoto, esegui lo split per ottenere (ad esempio) la prima immagine
                                if (!string.IsNullOrEmpty(hotel.Images?.Trim()))
                                {
                                    var imageList = hotel.Images.Split(',');
                                    var firstImage = imageList[0].Trim();

                                    // Costruisci il percorso completo usando il project directory
                                    string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                                    var imagePath = Path.Combine(projectDirectory, firstImage);

                                    hotel.ImageSource = GetImageSource(imagePath);
                                }

                                // I servizi dovrebbero essere già una lista (se il backend li restituisce come array).
                                // Se invece sono una stringa, potresti fare:
                                // hotel.Services = hotel.ServicesString.Split(',').Select(s => s.Trim()).ToList();

                                ResearchHotels.Add(hotel);
                            }
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
            if (File.Exists(imagePath))
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


    }
}
