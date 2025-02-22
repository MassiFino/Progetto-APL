using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class AddHotelPage : ContentPage
{

    public AddHotelPage()
    {
        InitializeComponent();
        BindingContext = new AddHotelPageViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var token = await SecureStorage.GetAsync("jwt_token");
        if (string.IsNullOrEmpty(token))
        {
            // Se non c'� il token, reindirizza l'utente al login
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }
    }



}

