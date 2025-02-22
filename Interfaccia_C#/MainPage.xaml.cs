using Microsoft.Maui.Storage;
using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Controlla se il token esiste
            var token = await SecureStorage.GetAsync("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                // Se il token manca, reindirizza alla pagina di Login
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            // Se il token è presente, carica i dati tramite il ViewModel
            if (BindingContext is MainPageViewModel viewModel)
            {
                await viewModel.LoadDataAsync();
            }
        }
    }
}
