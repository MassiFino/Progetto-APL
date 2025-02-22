using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class ProfileUserPage : ContentPage
{
    private ProfilUserPageViewModel _viewModel;

    public ProfileUserPage()
    {
        InitializeComponent();
        _viewModel = new ProfilUserPageViewModel(); // Crei un'istanza del ViewModel
        BindingContext = _viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var token = await SecureStorage.GetAsync("jwt_token");
        if (string.IsNullOrEmpty(token))
        {
            // Se il token manca, reindirizza alla pagina di Login
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        await _viewModel.LoadUserData();
        await _viewModel.LoadBookingData();
        await _viewModel.LoadInterestsAsync();
    }

}
