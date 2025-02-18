using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Interfaccia_C_.Model; // Importa il modello Hotel
using System.Net.Http.Headers; // Per AuthenticationHeaderValue
using Microsoft.Maui.Storage;   // Per SecureStorage
using System.Text;
using System.Windows.Input;



namespace Interfaccia_C_.ViewModel
{
    public class ProfilUserPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Booking> OwnedBookings { get; set; }
        public ObservableCollection<Interest> InterestedRooms { get; set; } = new ObservableCollection<Interest>();

        public class Interest
        {
            public int InterestID { get; set; }
            public int RoomID { get; set; }
            public string RoomName { get; set; }
            public string HotelName { get; set; }
            public double MonitorValue { get; set; }
        }


        // Costruttore del ViewModel
        public ProfilUserPageViewModel()
        {
            OwnedBookings = new ObservableCollection<Booking>();  // Inizializza la lista degli hotel

            // Inizializza il comando per aggiornare il grafico dei costi
            RefreshCostChartCommand = new Command(async () => await LoadCostChartAsync());
            _ = LoadCostChartAsync();

        }

        private string userName;
        private string email;
        private string role;
        private ImageSource _profileImage;
        private string message;
        // Classe per la risposta dell'API
        public class UserResponse
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string PImage { get; set; }

            public int Points { get; set; }

        }
        // Proprietà per il binding
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Role
        {
            get => role;
            set => SetProperty(ref role, value);
        }

        private int _puntiFedelta;
        public int PuntiFedelta
        {
            get => _puntiFedelta;
            set
            {
                if (_puntiFedelta != value)
                {
                    _puntiFedelta = value;
                    OnPropertyChanged();
                }
            }
        }

        public ImageSource ProfileImage
        {
            get => _profileImage;
            set
            {
                if (_profileImage != value)
                {
                    _profileImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private ImageSource _costChart;
        public ImageSource CostChart
        {
            get => _costChart;
            set
            {
                if (_costChart != value)
                {
                    _costChart = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _costDataSummary;
        public string CostDataSummary
        {
            get => _costDataSummary;
            set
            {
                if (_costDataSummary != value)
                {
                    _costDataSummary = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand RefreshCostChartCommand { get; }



        // Gestione delle modifiche alle proprietà
        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        // Eventi per il binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Command<Booking> lasciaRecensioneCommand;
        public Command<Booking> LasciaRecensioneCommand =>
            lasciaRecensioneCommand ??= new Command<Booking>(async (booking) => await OnLasciaRecensione(booking));

        private Command<Booking> inviaRecensioneCommand;
        public Command<Booking> InviaRecensioneCommand =>
            inviaRecensioneCommand ??= new Command<Booking>(async (booking) =>
            {
            // Prima esegui la funzione di invio recensione
            await OnInviaRecensione(booking);

            // Poi esegui la funzione FetchReviewData
            await FetchReviewData(booking);
            });

        private Command<Booking> eliminaPrenotazioneCommand;
        public Command<Booking> EliminaPrenotazioneCommand =>
            eliminaPrenotazioneCommand ??= new Command<Booking>(async (booking) => await OnEliminaPrenotazione(booking));

        // Comando per eliminare la recensione
        private Command<Booking> eliminaRecensioneCommand;
        public Command<Booking> EliminaRecensioneCommand =>
            eliminaRecensioneCommand ??= new Command<Booking>(async (booking) => await OnEliminaRecensione(booking));

        public ICommand DeleteInterestCommand => new Command<Interest>(async (interest) => await DeleteInterestAsync(interest));


        // Metodo per ottenere l'immagine del profilo
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

        // Metodo per caricare i dati dell'utente
        public async Task LoadUserData()
        {
            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    // Non hai il token, l'utente non è loggato
                    Debug.WriteLine("Token mancante! Reindirizzo al login?");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                var url = "http://localhost:9000/getUserData"; // Cambia l'URL se necessario
                using var client = new HttpClient();
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                // Se devi mandare un body JSON vuoto, ad es.:
                requestMessage.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                // 3) Imposta l'header Authorization: Bearer <token>
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // 4) Invii la richiesta
                var response = await client.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserializza la risposta in un oggetto UserResponse
                    var userData = JsonSerializer.Deserialize<UserResponse>(jsonResponse);
                    if (userData != null)
                    {
                        // Assegna i valori alle proprietà
                        UserName = userData.Username;
                        Email = userData.Email;
                        Role = userData.Role;

                        PuntiFedelta = userData.Points;

                        // Gestisci il percorso dell'immagine
                        string image = userData.PImage;
                        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                        var imagePath = Path.Combine(projectDirectory, image);

                        // Imposta l'immagine del profilo se il percorso è valido
                        ProfileImage = GetImageSource(imagePath);
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Errore", $"Caricamento dati fallito: {errorContent}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Errore", $"Si è verificato un errore: {ex.Message}", "OK");
            }
        }
        public async Task LoadBookingData()
        {
            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    // Non hai il token, l'utente non è loggato
                    Debug.WriteLine("Token mancante! Reindirizzo al login?");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                var url = "http://localhost:9000/getBookings"; 
                using var client = new HttpClient();

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

                // Se devi mandare un body JSON vuoto, ad es.:
                requestMessage.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                // 3) Imposta l'header Authorization: Bearer <token>
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // 4) Invii la richiesta
                var response = await client.SendAsync(requestMessage);

                var jsonR = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Questa è la risposta: " + jsonR);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Questa è la risposta: " + jsonResponse);

                    // Deserializza la risposta in una lista di prenotazioni
                    var bookings = JsonSerializer.Deserialize<List<Booking>>(jsonResponse);
                    if (bookings != null && bookings.Count > 0)
                    {
                        Debug.WriteLine("Questa è dopo la serializzazione: " + bookings);

                        OwnedBookings.Clear(); // Pulisce la lista corrente
                        Message = "Queste sono le tue prenotazioni";
                        foreach (var booking in bookings)
                        {
                           
                            // Debug per vedere i dettagli della prenotazione
                            Debug.WriteLine($"Prenotazione: {booking.roomName}");
                            Debug.WriteLine($"Check-in: {booking.checkInDate.ToShortDateString()}");
                            Debug.WriteLine($"Check-out: {booking.checkOutDate.ToShortDateString()}");
                            Debug.WriteLine($"Importo: €{booking.totalAmount:F2}");
                            Debug.WriteLine($"Stato: {booking.status}");
                            Debug.WriteLine($"RoomID: {booking.roomID}");

                            Debug.WriteLine($"RoomName: {booking.roomImage}");

                            Debug.WriteLine($"HotelName: {booking.hotelName}");

                            // Assegna l'immagine della stanza alla proprietà di binding
                            // Gestisci il percorso dell'immagine per l'hotel
                            // Gestisci le immagini: prendi solo la prima immagine dalla stringa separata da virgole
                            if (!string.IsNullOrEmpty(booking.roomImage?.Trim())) // Aggiungi Trim() per rimuovere spazi bianchi
                            {
                                var imageList = booking.roomImage.Split(','); // Dividi la stringa in un array di immagini
                                booking.roomImage = imageList[0]; // Prendi solo la prima immagine
                                Debug.WriteLine("Path immagine: " + booking.roomImage);
                            }

                            Debug.WriteLine($"RoomImages: {booking.roomImage}");
                            string image = booking.roomImage;
                            Debug.WriteLine("Path immagine hotel: " + image);

                            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                            var imagPath = Path.Combine(projectDirectory, image);
                            Debug.WriteLine("Path completo " + imagPath);
                            // Imposta l'immagine dell'hotel se il percorso è valido
                            booking.ImageSource = GetImageSource(imagPath); // Imposta la proprietà ImageSource
                            Debug.WriteLine("Immaginissima room: " + booking.ImageSource);

                            // Aggiungi la prenotazione alla lista
                            OwnedBookings.Add(booking);
                            FetchReviewData(booking);
                        }
                    }
                    else
                    {
                        // Se non ci sono prenotazioni, imposta il messaggio "Non hai prenotazioni"
                        Message = "Non hai prenotazioni";
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Errore", $"Caricamento dati fallito: {errorContent}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Errore", $"Si è verificato un errore: {ex.Message}", "OK");
            }
        }

        private async Task OnLasciaRecensione(Booking booking)
        {
            if (booking == null) return;

            if (DateTime.Now < booking.checkOutDate)
            {
                await Application.Current.MainPage.DisplayAlert("Attenzione", "Non puoi lasciare una recensione prima del check-out.", "OK");
                return;
            }

            if (!string.IsNullOrWhiteSpace(booking.review))
            {
                await Application.Current.MainPage.DisplayAlert("Attenzione", "Hai già inserito una recensione per questa prenotazione.", "OK");
                return;
            }

            // Mostra sezione recensione
            booking.IsReviewVisible = true;
        }

        private async Task OnInviaRecensione(Booking booking)
        {
            if (booking == null) return;

            int rating = booking.Rating;
            string comment = booking.Comment;

            var payload = new
            {
                Username = booking.username,
                RoomID = booking.roomID,
                Comment = booking.Comment,
                Rating = booking.Rating,
            };


            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/addReview");
            var json = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Successo", "Recensione inviata", "OK");
                // nascondo sezione e resetto
                booking.IsReviewVisible = false;
                booking.Rating = 0;
                booking.Comment = "";
                booking.IsDeleteReviewVisible = true;

            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Errore", errorContent, "OK");
            }
        }

        public class Review
        {
            public string createdAt { get; set; }  // Data della recensione
            public string review { get; set; }     // Commento della recensione
            public decimal rating { get; set; }     // Voto della recensione
        }




        // Metodo che recupera la recensione (simulato, può essere un'operazione asincrona)
        public async Task FetchReviewData(Booking booking)
        {
            try
            {
                // Recupera il token JWT dall'archivio sicuro
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    // Se non c'è il token, l'utente non è loggato
                    Debug.WriteLine("Token mancante! Reindirizzo al login?");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                // Crea il payload con i dati necessari
                var payload = new
                {
                    Username = booking.username,
                    RoomID = booking.roomID
                };

                Debug.WriteLine($"username: {booking.username}");
                Debug.WriteLine($"roomID: {booking.roomID}");

                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/getReviews");

                // Serializza il payload in formato JSON
                var json = JsonSerializer.Serialize(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                // Imposta l'header di autorizzazione
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Invia la richiesta e ottieni la risposta
                var response = await client.SendAsync(request);

                // Leggi la risposta
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Questa è la risposta: " + jsonResponse);

                // Verifica se la risposta è stata positiva
                if (response.IsSuccessStatusCode)
                {
                    // Deserializza la risposta in un oggetto Review
                    var reviewData = JsonSerializer.Deserialize<Review>(jsonResponse);

                    // Controlla se la recensione è vuota (commento, rating e data)
                    if (string.IsNullOrWhiteSpace(reviewData.review) &&
                             reviewData.rating == 0 &&
                             string.IsNullOrWhiteSpace(reviewData.createdAt))
                    {
                        booking.MessageReview = "Non hai ancora inserito nessuna recensione.";
                        Debug.WriteLine(" : " + booking.MessageReview);

                        // Nasconde la sezione recensione se non ci sono dati
                        booking.IsReviewSectionVisible = false;
                        booking.IsDeleteReviewVisible = false;
                        Debug.WriteLine("La recensione è vuota.");
                    }
                    else
                    {
                        booking.MessageReview = "Questa è la recensione che hai inserito.";
                        Debug.WriteLine(" : " + booking.MessageReview);

                        // Assegna i dati della recensione alle proprietà del ViewModel
                        booking.createdAt = reviewData.createdAt;
                        booking.review = reviewData.review;
                        booking.voto = reviewData.rating;

                        Debug.WriteLine($"createdAt: {booking.createdAt}");
                        Debug.WriteLine($"review: {booking.review}");
                        Debug.WriteLine($"rating: {booking.voto}");

                        // Mostra la sezione recensione
                        booking.IsReviewSectionVisible = true;
                        booking.IsDeleteReviewVisible = true;
                    }
                }
                else
                {
                    // Gestisci errori di risposta dal server
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await Application.Current.MainPage.DisplayAlert("Errore", $"Caricamento dati fallito: {errorContent}", "OK");
                    booking.IsReviewSectionVisible = false;
                }
            }
            catch (Exception ex)
            {
                // Gestisci eventuali errori di rete o eccezioni
                Debug.WriteLine("Errore durante la chiamata: " + ex.Message);
                booking.IsReviewSectionVisible = false; // Assicurati che la recensione sia nascosta in caso di errore
            }
        }



        private async Task OnEliminaPrenotazione(Booking booking)
        {
            if (booking == null) return;


            double giorniRimanenti = (booking.checkInDate - DateTime.Now).TotalDays;
            Debug.WriteLine($"Giorni rimanenti per il check-in: {giorniRimanenti}");

            if (giorniRimanenti < 5)
            {
                await Application.Current.MainPage.DisplayAlert("Attenzione", "Non puoi eliminare la prenotazione se mancano meno di 5 giorni dal check-in.", "OK");
                return;
            }


            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }
                using var client = new HttpClient();

                // Prepara il payload per eliminare la prenotazione
                var payload = new
                {
                    BookingID = booking.bookingID,  // Assicurati che questa proprietà esista nel modello Booking
                    Username = booking.username
                };
                var json = JsonSerializer.Serialize(payload);

                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/deleteBooking");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Successo", "Prenotazione eliminata", "OK");
                    // Rimuovi la prenotazione dalla lista
                    OwnedBookings.Remove(booking);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await Application.Current.MainPage.DisplayAlert("Errore", errorContent, "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", ex.Message, "OK");
            }
        }

        private async Task OnEliminaRecensione(Booking booking)
        {
            if (booking == null) return;


            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }
                using var client = new HttpClient();

                // Prepara il payload per eliminare la recensione
                var payload = new
                {
                    Username = booking.username,
                    RoomID = booking.roomID
                };
                var json = JsonSerializer.Serialize(payload);
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/deleteReview");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Successo", "Recensione eliminata", "OK");
                    // Aggiorna le proprietà relative alla recensione
                    booking.MessageReview = "Recensione eliminata";
                    booking.review = "";
                    booking.voto = 0;
                    booking.createdAt = "";
                    booking.IsReviewSectionVisible = false;

                    booking.IsDeleteReviewVisible = false; //scompare il pulsante elimina
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await Application.Current.MainPage.DisplayAlert("Errore", errorContent, "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", ex.Message, "OK");
            }
        }

        public async Task LoadCostChartAsync()
        {
            try
            {
                using var client = new HttpClient();
                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                string token = await SecureStorage.GetAsync("jwt_token");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.PostAsync("http://localhost:9000/getCostChart", content);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Response JSON: " + jsonResponse);
                    using var doc = JsonDocument.Parse(jsonResponse);
                    // Leggi il grafico
                    string base64 = doc.RootElement.GetProperty("chart").GetString();
                    if (!string.IsNullOrEmpty(base64))
                    {
                        byte[] imageBytes = Convert.FromBase64String(base64);
                        CostChart = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Attenzione", "Non sono disponibili dati di costo.", "OK");
                    }
                    // (Opzionale) Leggi la sintesi dei dati e aggiorna una proprietà CostDataSummary, se presente
                    if (doc.RootElement.TryGetProperty("summary", out JsonElement summaryElem))
                    {
                        double total = summaryElem.GetProperty("total").GetDouble();
                        string monthlyText = "";
                        if (summaryElem.TryGetProperty("monthly", out JsonElement monthlyElem))
                        {
                            foreach (var monthItem in monthlyElem.EnumerateArray())
                            {
                                string month = monthItem.GetProperty("month").GetString();
                                double cost = monthItem.GetProperty("cost").GetDouble();
                                monthlyText += $"{month}: {cost} €\n";
                            }
                        }
                        string summaryText = $"Totale: {total} €\n{monthlyText}";
                        CostDataSummary = summaryText;
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Errore", "Impossibile caricare il grafico dei costi", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", ex.Message, "OK");
            }
        }


        public async Task LoadInterestsAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent("{}", Encoding.UTF8, "application/json");

                // Supponiamo che l'endpoint sia /getInterests
                var response = await client.PostAsync("http://localhost:9000/getInterests", content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var interests = JsonSerializer.Deserialize<List<Interest>>(json);
                    InterestedRooms.Clear();
                    if (interests != null)
                    {
                        foreach (var interest in interests)
                            InterestedRooms.Add(interest);
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Errore", "Impossibile caricare gli interessi", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", ex.Message, "OK");
            }
        }

        private async Task DeleteInterestAsync(Interest interest)
        {
            var token = await SecureStorage.GetAsync("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new { InterestID = interest.InterestID };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://localhost:9000/deleteInterest", content);
            if (response.IsSuccessStatusCode)
            {
                InterestedRooms.Remove(interest);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                await Application.Current.MainPage.DisplayAlert("Errore", error, "OK");
            }
        }



    }




}
