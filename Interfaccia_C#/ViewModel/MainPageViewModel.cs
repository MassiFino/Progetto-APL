using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Interfaccia_C_.Model;  // Per usare la classe Hotel
using System.Collections.Generic;

namespace Interfaccia_C_.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private HttpClient _httpClient;

        // Ora usi 'Hotel' al posto di MeteGettonateModel / OffertaImperdibileModel
        private ObservableCollection<Hotel> _meteGettonate;
        private ObservableCollection<Hotel> _offerteImperdibili;

        public ObservableCollection<Hotel> MeteGettonate
        {
            get => _meteGettonate;
            set => SetProperty(ref _meteGettonate, value);
        }

        public ObservableCollection<Hotel> OfferteImperdibili
        {
            get => _offerteImperdibili;
            set => SetProperty(ref _offerteImperdibili, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand GuardaOffertaCommand { get; set; }

        public MainPageViewModel()
        {
            // Comando per 'Guarda Offerta'
            GuardaOffertaCommand = new Command<Hotel>(async (hotelSelezionato) =>
            {
                // Naviga con Shell, passando l'oggetto hotel
                var navParams = new Dictionary<string, object>
                {
                    ["hotel"] = hotelSelezionato
                };

                await Shell.Current.GoToAsync("HotelPage", navParams);
            });


            _httpClient = new HttpClient();
            MeteGettonate = new ObservableCollection<Hotel>();
            OfferteImperdibili = new ObservableCollection<Hotel>();

            RefreshCommand = new Command(async () => await CaricaDati());

            _ = CheckToken();  // Avvia il controllo del token
        }

        private async Task CheckToken()
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                // Se mancante, reindirizza al Login
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                Debug.WriteLine($"Token presente: {token.Substring(0, Math.Min(token.Length, 10))}...");
                await CaricaDati();
            }
        }

        private async Task CaricaDati()
        {
            await GetMeteGettonate();
            await GetOfferteImperdibili();
        }

        private async Task GetMeteGettonate()
        {
            try
            {
                Debug.WriteLine("Chiamata a GetMeteGettonate");
                var content = new StringContent("{}", Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("http://localhost:9000/getMeteGettonate", content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Errore nella risposta: {errorMsg}");
                    return;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"📩 Risposta JSON ricevuta: {jsonResponse}");

                // Deserializzi in List<Hotel>
                var mete = JsonSerializer.Deserialize<List<Hotel>>(jsonResponse);

                MeteGettonate.Clear();
                foreach (var meta in mete)
                {
                    MeteGettonate.Add(meta);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore nel recupero delle mete gettonate: {ex.Message}");
            }
        }

        private async Task GetOfferteImperdibili()
        {
            try
            {
                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:9000/getOfferteImperdibili", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Errore nella risposta: {errorMsg}");
                    return;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"📩 Risposta JSON ricevuta: {jsonResponse}");

                // Deserializzi in List<Hotel>
                var offerte = JsonSerializer.Deserialize<List<Hotel>>(jsonResponse);

                OfferteImperdibili.Clear();
                foreach (var offerta in offerte)
                {
                    OfferteImperdibili.Add(offerta);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore nel recupero delle offerte imperdibili: {ex.Message}");
            }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }
}
