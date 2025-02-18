using Interfaccia_C_.Model;
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
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace Interfaccia_C_.ViewModel
{
    public class BookingPageViewModel : INotifyPropertyChanged
{
        public int HotelID { get; set; }
        public string HotelName { get; set; }
        public int BookingID { get; set; }
        public int RoomID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        private double _totalAmount;

        public double TotalAmount
        {
            get => _totalAmount;
            set
            {
                if (_totalAmount != value)
                {
                    _totalAmount = value;

                    Debug.WriteLine($"TotalAmount updated to: {value}");

                    OnPropertyChanged(); // Notifica la UI del cambiamento
                }
            }
        }
        public string Status { get; set; }
        public string RoomName { get; set; }
        public string RoomImage { get; set; }
        public ImageSource ImageSource { get; set; }



        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Metodo per invocare l'evento PropertyChanged
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ICommand ConfermaPrenotazioneCommand { get; }
        public ICommand ApplicaScontoCommand { get; }

        private int _pointsSpent;
        public int PointsSpent
        {
            get => _pointsSpent;
            set
            {
                if (_pointsSpent != value)
                {
                    _pointsSpent = value;
                    OnPropertyChanged();
                }
            }
        }


        public BookingPageViewModel(Booking booking)
        {
            HotelName = booking.hotelName;
            HotelID = booking.hotelID;
            BookingID = booking.bookingID;
            RoomID = booking.roomID;
            CheckInDate = booking.checkInDate;
            CheckOutDate = booking.checkOutDate;
            TotalAmount = booking.totalAmount;
            Status = booking.status;
            RoomName = booking.roomName;
            RoomImage = booking.roomImage;

            Debug.WriteLine($"Booking Details: {booking}");
            // Stampa tutti i valori della prenotazione nella finestra di output del debugger
            Debug.WriteLine($"HotelID: {HotelID}");
            Debug.WriteLine($"HotelName: {HotelName}");

            Debug.WriteLine($"Booking ID: {booking.bookingID}");
            Debug.WriteLine($"Room ID: {booking.roomID}");
            Debug.WriteLine($"Check-in Date: {booking.checkInDate}");
            Debug.WriteLine($"Check-out Date: {booking.checkOutDate}");
            Debug.WriteLine($"Total Amount: {booking.totalAmount:C}");
            Debug.WriteLine($"Status: {booking.status}");
            Debug.WriteLine($"Room Name: {booking.roomName}");
            Debug.WriteLine($"Room Image: {booking.roomImage}");

            if (!string.IsNullOrEmpty(booking.roomImage?.Trim()))
            {
                // Divido la stringa in pi� immagini
                var imageList = booking.roomImage.Split(';');
                // Prendo la prima
                var firstImage = imageList[0].Trim();

                Debug.WriteLine("Path immagine (prima del combine): " + firstImage);

                // Costruisco path completo
                string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                var imagePath = Path.Combine(projectDirectory, firstImage);
                Debug.WriteLine("Path completo: " + imagePath);

                // Converto in ImageSource
                ImageSource = GetImageSource(imagePath);
                Debug.WriteLine("Immaginissima: " + booking.ImageSource);
            }


            ConfermaPrenotazioneCommand = new Command(() => PrenotaStanza(booking));
            ApplicaScontoCommand = new Command(async () => await ApplicaSconto());
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
                return null;
            }
        }

     

        private async Task ApplicaSconto()
        {
            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    await Application.Current.MainPage.DisplayAlert("Errore", "Token mancante", "OK");
                    return;
                }

                // Costruisci payload
                var requestBody = new
                {
                    TotalAmount = this.TotalAmount
                };

                var json = JsonSerializer.Serialize(requestBody);

                Debug.WriteLine(json);

                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://localhost:9000/previewDiscount", content);

                var responseStr = await response.Content.ReadAsStringAsync();

                Debug.WriteLine(responseStr);

                if (response.IsSuccessStatusCode)
                {
                    // Deserializzi la risposta
                    var result = JsonSerializer.Deserialize<PreviewDiscountResponse>(responseStr);

                    // Adesso hai:
                    double discountedPrice = result.discounted_price;
                    int pointsUsed = result.points_used;

                    // Qui controlli se i punti usati sono 0 => "Non hai abbastanza punti..."
                    if (pointsUsed == 0)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Nessuno sconto applicabile",
                            "Non hai abbastanza punti per ottenere uno sconto.",
                            "OK"
                        );
                        return;
                    }

                    // Altrimenti mostri la preview con sconto > 0
                    await Application.Current.MainPage.DisplayAlert(
                        "Anteprima Sconto",
                        $"Prezzo originale: {this.TotalAmount}\n" +
                        $"Prezzo scontato: {discountedPrice}\n" +
                        $"Punti usati: {pointsUsed}",
                        "OK"
                    );
                    await Microsoft.Maui.ApplicationModel.MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        this.TotalAmount = discountedPrice;
                        this.PointsSpent = pointsUsed;
                    });
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Errore", responseStr, "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", ex.Message, "OK");
            }

        }

        private async Task PrenotaStanza(Booking booking)
        {
            // Controllo se la sessione (token) � attiva
            var token = await SecureStorage.GetAsync("jwt_token");

            // Calcola il numero di notti
            int nights = (int)(booking.checkOutDate.Date - booking.checkInDate.Date).TotalDays;
            if (nights <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Il numero di notti deve essere maggiore di zero.", "OK");
                return;
            }

            // Calcola l'importo totale lo faccio nella pagina dell'hotel quando mando i dati
            // double totalAmount = room.PricePerNight * nights;

            // Recupera l'username dal token oppure da una propriet� del ViewModel (se lo hai gi� estratto in precedenza)
            // Qui presumo di avere un metodo GetLoggedUsername() che lo estrae dal token
            // Crea il payload per la prenotazione

            var payload = new
            {
                RoomID = booking.roomID,
                CheckInDate = booking.checkInDate.ToString("yyyy-MM-dd"),
                CheckOutDate = booking.checkOutDate.ToString("yyyy-MM-dd"),
                TotalAmount = this.TotalAmount,
                Status = "Confirmed",
                RoomType = booking.roomType,
                Guests = booking.guests,
                Nights = booking.nights,

                PointsSpent = this.PointsSpent
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsync("http://localhost:9000/addBooking", content);
                if (response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Prenotazione Effettuata",
                        $"Hai prenotato la stanza: {booking.roomName}\n" +
                        $"Check-in: {booking.checkInDate.ToShortDateString()}\n" +
                        $"Check-out: {booking.checkOutDate.ToShortDateString()}\n" +
                        $"Totale: {this.TotalAmount:C}",
                        "OK");

                    await Shell.Current.GoToAsync("//MainPage");

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Errore", "Si � verificato un errore durante la prenotazione. Riprova.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Eccezione in PrenotaStanza: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Errore", ex.Message, "OK");
            }
        }

        public class PreviewDiscountResponse
        {
            public string status { get; set; }
            public double discounted_price { get; set; }
            public int points_used { get; set; }
            public double original_price { get; set; }
        }


    }



}
