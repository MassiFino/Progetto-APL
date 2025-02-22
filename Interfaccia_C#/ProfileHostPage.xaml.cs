using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class ProfileHostPage : ContentPage
{
    private ProfilHostPageViewModel _viewModel;

    public ProfileHostPage()
    {
        InitializeComponent();
        _viewModel = new ProfilHostPageViewModel();
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
        await _viewModel.LoadHotelData();
    }

}
