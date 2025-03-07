using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Diagnostics;
using System.Data;
using Microsoft.Maui.Storage;


namespace Interfaccia_C_.ViewModel
{
    public class RegisterPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Metodo per notificare la modifica delle proprietà
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }
        private bool isUserSelected;
        private bool isHostSelected;
        private string selectedUserType;

        // Proprietà per "User"
        public bool IsUserSelected
        {
            get => isUserSelected;
            set
            {
                if (isUserSelected != value)
                {
                    isUserSelected = value;
                    OnPropertyChanged(nameof(IsUserSelected));

                    if (value)  // Quando "User" è selezionato
                    {
                        IsHostSelected = false;  // Deseleziona "Host"
                        SelectedUserType = "User";  // Imposta "User"
                    }

                    // Debug: Stampa il valore selezionato
                    Debug.WriteLine("User Selected: " + (IsUserSelected ? "User" : "None"));
                }
            }
        }

        // Proprietà per "Host"
        public bool IsHostSelected
        {
            get => isHostSelected;
            set
            {
                if (isHostSelected != value)
                {
                    isHostSelected = value;
                    OnPropertyChanged(nameof(IsHostSelected));

                    if (value)  // Quando "Host" è selezionato
                    {
                        IsUserSelected = false;  // Deseleziona "User"
                        SelectedUserType = "Host";  // Imposta "Host"
                    }

                    // Debug: Stampa il valore selezionato
                    Debug.WriteLine("Host Selected: " + (IsHostSelected ? "Host" : "None"));
                }
            }
        }

        // Proprietà per memorizzare il tipo di utente selezionato
        public string SelectedUserType
        {
            get => selectedUserType;
            set
            {
                if (selectedUserType != value)
                {
                    selectedUserType = value;
                    OnPropertyChanged(nameof(SelectedUserType));

                    // Debug: Stampa il valore della variabile selezionata
                    Debug.WriteLine("Selected User Type: " + selectedUserType);
                }
            }
        }

        public class RegisterResponse
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string Role { get; set; }
            public string Token { get; set; } 
        }

        public ICommand GoToLoginCommand { get; }

        public ICommand RegisterCommand { get; }


        // immagine del profilo
        private string _profileImage;
        public string ProfileImage
        {
            get => _profileImage;
            set
            {
                _profileImage = value;
                OnPropertyChanged();  // Notifica il cambiamento
            }
        }

        // Proprietà per il nome del file caricato (messaggio di conferma)
        private string _uploadStatusMessage;
        public string UploadStatusMessage
        {
            get => _uploadStatusMessage;
            set
            {
                _uploadStatusMessage = value;
                OnPropertyChanged();  // Notifica il cambiamento
            }
        }

        // controllo della visibilità del messaggio di conferma
        private bool _isUploadComplete;
        public bool IsUploadComplete
        {
            get => _isUploadComplete;
            set
            {
                _isUploadComplete = value;
                OnPropertyChanged();  // Notifica il cambiamento
            }
        }

        public ICommand UploadImageCommand { get; }

        // Costruttore
        public RegisterPageViewModel()
        {

            RegisterCommand = new Command(async () => await Register());

            GoToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));

            UploadImageCommand = new Command(async () => await OnUploadImage());
        }

        // Metodo che apre il selettore di file per caricare l'immagine
        private async System.Threading.Tasks.Task OnUploadImage()
        {
            try
            {
                // Apri il selettore di file per scegliere un'immagine
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a Profile Picture", // Titolo del selettore
                    FileTypes = FilePickerFileType.Images // Filtro per immagini
                });

                if (result != null)
                {

                    // cartella principale del progetto
                    string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

                    // Percorso della cartella "Progetto-APL" dove vuoi salvare i file
                    var localPath = Path.Combine(projectDirectory, "Pictures", "ProfilePictures");

                    // Crea la directory "ProfilePictures" se non esiste
                    Directory.CreateDirectory(localPath);

                    // percorso completo del file
                    var filePath = Path.Combine(localPath, result.FileName);

                    var Pat= Path.Combine("Pictures", "ProfilePictures", result.FileName);

                    // Copia il file nella cartella locale
                    using (var stream = await result.OpenReadAsync())
                    using (var localFile = File.Create(filePath))
                    {
                        await stream.CopyToAsync(localFile);
                    }
                    ProfileImage = Pat;

                    UploadStatusMessage = $"File Uploaded: {result.FileName}";

                    IsUploadComplete = true;

                }
                else
                {
                    ProfileImage = null;
                    UploadStatusMessage = null;
                    IsUploadComplete = false;
                }
            }
            catch (Exception ex)
            {
                ProfileImage = null; // Azzera l'immagine in caso di errore
                UploadStatusMessage = $"Error: {ex.Message}";
                IsUploadComplete = false; // Nasconde il messaggio di conferma in caso di errore
                Console.WriteLine($"Error selecting image: {ex.Message}");
            }
        }

        private async Task Register()
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Shell.Current.DisplayAlert("Errore", "Tutti i campi sono obbligatori.", "OK");
                return;
            }
            if (Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Errore", "Le password non coincidono.", "OK");
                return;
            }

            var payload = new
            {
                Username = this.Name,
                Email = this.Email,
                Password = this.Password,
                PImage = ProfileImage,
                Role = selectedUserType
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            Debug.WriteLine("Contenuto della richiesta: " + jsonPayload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                using var client = new HttpClient();
                var url = "http://localhost:9000/signup";

                var response = await client.PostAsync(url, content);
                Debug.WriteLine("dopo signup " + selectedUserType);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Risposta JSON: " + responseContent);

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var registerResponse = JsonSerializer.Deserialize<RegisterResponse>(responseContent, options);

                    if (registerResponse != null && registerResponse.Status == "ok")
                    {
                        if (!string.IsNullOrEmpty(registerResponse.Token))
                        {
                            await SecureStorage.SetAsync("jwt_token", registerResponse.Token);
                        }

                        if (selectedUserType == "Host")
                        {
                            Debug.WriteLine("sono un Host " + selectedUserType);

                            var hostShell = new HostShell();
                            Application.Current.MainPage = hostShell;
                            await hostShell.GoToAsync("//ProfilePage");
                        }
                        else
                        {
                            Debug.WriteLine("sono un User " + selectedUserType);

                            var userShell = new UserShell();
                            Application.Current.MainPage = userShell;
                            await userShell.GoToAsync("//MainPage");
                        }

                        await Shell.Current.DisplayAlert("Successo", "Registrazione completata con successo!", "OK");
                    }
                    else
                    {

                        var errorContent = registerResponse?.Message ?? "Risposta sconosciuta dal server.";
                        await Shell.Current.DisplayAlert("Errore", $"Registrazione fallita: {errorContent}", "OK");
                    }
                }
                else
                {
                    //Se lo stato non è success
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Errore", $"Registrazione fallita: {errorContent}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Errore", $"Si è verificato un errore: {ex.Message}", "OK");
            }
        }


    }
}
