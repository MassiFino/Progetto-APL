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
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel?.LoadUserData();  // Richiama la funzione di caricamento dei dati ogni volta che la pagina appare
    }
    // Carica i dati dell'utente all'avvio

}
