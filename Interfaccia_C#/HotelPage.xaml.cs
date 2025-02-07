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
}

