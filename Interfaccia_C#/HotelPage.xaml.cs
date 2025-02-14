using Interfaccia_C_.Model; // Importa il modello Hotel

using Interfaccia_C_.ViewModel;
namespace Interfaccia_C_;

public partial class HotelPage : ContentPage, IQueryAttributable
{
    public HotelPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("hotel", out var hotelObj) && hotelObj is Hotel hotel)
        {
            BindingContext = new HotelPageViewModel(hotel);
        }
    }

 

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var token = await SecureStorage.GetAsync("jwt_token");
        if (string.IsNullOrEmpty(token))
        {
            // Se non c'è il token, reindirizza l'utente al login
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        if (BindingContext is HotelPageViewModel viewModel)
        {
            await viewModel.LoadRoomsAsync();
            await viewModel.LoadReviewsAsync();
        }
    }
}

