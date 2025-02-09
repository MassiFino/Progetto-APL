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
        public ICommand GoToSearchCommand { get; }

        private HttpClient _httpClient;

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
            GoToSearchCommand = new Command(async () => await Shell.Current.GoToAsync("SearchPage"));

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
                    if (!string.IsNullOrEmpty(meta.Images?.Trim()))
                    {
                        // Divido la stringa in più immagini
                        var imageList = meta.Images.Split(',');
                        // Prendo la prima
                        var firstImage = imageList[0].Trim();

                        Debug.WriteLine("Path immagine (prima del combine): " + firstImage);

                        // Costruisco path completo
                        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                        var imagePath = Path.Combine(projectDirectory, firstImage);
                        Debug.WriteLine("Path completo: " + imagePath);

                        // Converto in ImageSource
                        meta.ImageSource = GetImageSource(imagePath);
                        Debug.WriteLine("Immaginissima: " + meta.ImageSource);
                    }
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
                    if (!string.IsNullOrEmpty(offerta.Images?.Trim()))
                    {
                        // Divido la stringa in più immagini
                        var imageList = offerta.Images.Split(',');
                        // Prendo la prima
                        var firstImage = imageList[0].Trim();

                        Debug.WriteLine("Path immagine (prima del combine): " + firstImage);

                        // Costruisco path completo
                        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                        var imagePath = Path.Combine(projectDirectory, firstImage);
                        Debug.WriteLine("Path completo: " + imagePath);

                        // Converto in ImageSource
                        offerta.ImageSource = GetImageSource(imagePath);
                        Debug.WriteLine("Immaginissima: " + offerta.ImageSource);
                    }
                    OfferteImperdibili.Add(offerta);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore nel recupero delle offerte imperdibili: {ex.Message}");
            }
        }
        public ImageSource GetImageSource(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                Debug.WriteLine("Immagine trovata: " + imagePath);
                return ImageSource.FromFile(imagePath);  // Usa FileImageSource per caricare l'immagine

            }
            else
            {
                Debug.WriteLine("Immagine non trovata: " + imagePath);

                return null;  // Immagine non trovata
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
