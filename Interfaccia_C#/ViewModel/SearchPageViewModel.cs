using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;  // Per simulare chiamate asincrone
using System.Diagnostics;     // Per il debug

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

        // Comando per eseguire la ricerca
        public ICommand SearchCommand { get; }

        public SearchPageViewModel()
        {
            SearchCommand = new Command(async () => await ExecuteSearch());
        }

        // Metodo che simula una ricerca
        private async Task ExecuteSearch()
        {
            // Simuliamo un piccolo ritardo per vedere che il comando funziona

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
        }
    }
}
