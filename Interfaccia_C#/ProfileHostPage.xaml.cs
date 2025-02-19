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
        // Carica i dati dell'host all'avvio della pagina
        await _viewModel.LoadUserData();
        await _viewModel.LoadHotelData();
    }
    
}
