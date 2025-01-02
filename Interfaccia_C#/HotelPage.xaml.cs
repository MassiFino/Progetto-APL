using Interfaccia_C_.Model; // Importa il modello Hotel

using Interfaccia_C_.ViewModel;
namespace Interfaccia_C_;

public partial class HotelPage : ContentPage

{ 
    public HotelPage(Hotel hotel)
    {
        InitializeComponent();

        // Imposta il ViewModel con i dati dell'hotel
        BindingContext = new HotelPageViewModel(hotel);
    }
}

