using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
        InitializeComponent();
        BindingContext = new SearchPageViewModel();
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
    }
}
