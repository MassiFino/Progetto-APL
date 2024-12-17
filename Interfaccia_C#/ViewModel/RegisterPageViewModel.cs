using System;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Interfaccia_C_.ViewModel
{
    public class RegisterPageViewModel : INotifyPropertyChanged
    {
        // Evento per la notifica delle modifiche delle proprietà
        public event PropertyChangedEventHandler PropertyChanged;

        // Metodo per notificare la modifica delle proprietà
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Proprietà per il comando di navigazione alla LoginPage
        public ICommand GoToLoginCommand { get; }

        // Proprietà per la gestione dell'immagine del profilo
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

        // Proprietà per il controllo della visibilità del messaggio di conferma
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

        // Proprietà per il comando di caricamento immagine
        public ICommand UploadImageCommand { get; }

        // Costruttore
        public RegisterPageViewModel()
        {
            // Inizializza il comando di navigazione
            GoToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));

            // Inizializza il comando per caricare l'immagine
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

                // Se l'utente ha selezionato un file
                if (result != null)
                {
                    // Imposta il percorso dell'immagine selezionata nella proprietà
                    ProfileImage = result.FullPath;

                    // Imposta il nome del file caricato come messaggio di conferma
                    UploadStatusMessage = $"File Uploaded: {result.FileName}";

                    // Imposta la visibilità del messaggio di conferma a true
                    IsUploadComplete = true;
                }
                else
                {
                    // Se l'utente non ha selezionato nulla
                    ProfileImage = null;
                    UploadStatusMessage = null;
                    IsUploadComplete = false;
                }
            }
            catch (Exception ex)
            {
                // Gestione degli errori, se qualcosa va storto
                ProfileImage = null; // Azzera l'immagine in caso di errore
                UploadStatusMessage = $"Error: {ex.Message}";
                IsUploadComplete = false; // Nasconde il messaggio di conferma in caso di errore
                Console.WriteLine($"Error selecting image: {ex.Message}");
            }
        }
    }
}
