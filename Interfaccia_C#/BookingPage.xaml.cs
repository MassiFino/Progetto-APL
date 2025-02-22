using Interfaccia_C_.Model; // Importa il modello Hotel

using Interfaccia_C_.ViewModel;
namespace Interfaccia_C_;

public partial class BookingPage : ContentPage, IQueryAttributable
{
    public BookingPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Booking", out var bookingObj) && bookingObj is Booking booking)
        {

            BindingContext = new BookingPageViewModel(booking);
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var token = await SecureStorage.GetAsync("jwt_token");
        if (string.IsNullOrEmpty(token))
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

    }
}