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
using System.Net.Http.Headers;



namespace Interfaccia_C_.ViewModel
{
    public class AddRoomPageViewModel: INotifyPropertyChanged
    {
        public string Name { get; }
        public string Location { get; set; }
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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

        private async Task<double> GetAveragePrice(string RoomType, string Location)
        {   // Controlla se RoomType o Location sono nulli o vuoti
           
            var token = await SecureStorage.GetAsync("jwt_token");

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/getAveragePrice");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new { RoomType, Location };
            var json = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Errore nella richiesta: {response.StatusCode}, {errorContent}");
                return 0; // Se la richiesta fallisce, restituisce 0
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseBody);
            if (responseJson.RootElement.TryGetProperty("avgPrice", out var avgPriceElement) && avgPriceElement.TryGetDouble(out double avgPrice))
            {
                return avgPrice;
            }
            return 0;
        }
        private async Task AddRoom()
        {
           Debug.WriteLine($"Location: {Location}, Room ID: {RoomType}");

            // 🔍 Controllo del prezzo medio prima dell'inserimento
            double avgPrice = await GetAveragePrice(RoomType, Location);

            if (avgPrice == 0)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Informazione",
                    $"Non è stato trovato alcun valore medio corrispondente per le stanze di tipo {RoomType} in {Location}. Procedi pure con l'inserimento.",
                    "OK");
            }
            else if (avgPrice > 0 && PricePerNight != avgPrice)
            {
                string suggestion = PricePerNight > avgPrice ? "abbassare" : "alzare";
                string message = $"📊 Il prezzo inserito **{PricePerNight:C}** è diverso dal **prezzo medio** per le stanze di tipo **{RoomType}** in **{Location}**.\n\n" +
                                $"💰 **Prezzo medio attuale**: {avgPrice:C}\n" +
                                $"🔍 Ti suggeriamo di **{suggestion} il prezzo per essere più competitivo**.\n\nVuoi modificarlo?";

                bool adjustPrice = await Application.Current.MainPage.DisplayAlert(
                    "Suggerimento Prezzo",
                    message,
                    "Sì, lo cambio",
                    "No, mantieni");

                if (adjustPrice)
                {
                    return; // L'utente deve reinserire il prezzo
                }
            }
            var payload = new
            {
                HotelName = Name,
                RoomName = this.RoomName,
                RoomDescription = this.RoomDescription,
                PricePerNight = this.PricePerNight,
                MaxGuests = this.MaxGuests,
                RoomType = this.RoomType,
                RoomImagePath

            };
            Debug.WriteLine($"Immagine Stanza: {RoomImagePath}");

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
            {                await Application.Current.MainPage.DisplayAlert("Successo", "Stanza inserita con successo!", "OK");

               
                // Chiede all'utente se vuole inserire un'altra stanza
                string previewMessage = "Stanza e hotel aggiunti con successo";
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Conferma Selezione",
                    previewMessage + "\n\nVuoi inserire un'altra stanza?",
                    "Sì",
                    "No");

                if (confirm)
                {
                    RoomName = string.Empty;            // Svuota il nome della stanza
                    RoomDescription = string.Empty;     // Svuota la descrizione della stanza
                    PricePerNight = 0;                  // Svuota il prezzo per notte (imposta a 0)
                    MaxGuests = 0;                           // Svuota il numero massimo di ospiti (imposta a 0)
                    RoomType = string.Empty;            // Svuota la tipologia di stanza
                    RoomImagePath = string.Empty;       // Svuota il percorso dell'immagine
                    RoomImageNames = string.Empty;
                }
                else
                {
                    await Shell.Current.GoToAsync("//ProfilePage");
                }
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
