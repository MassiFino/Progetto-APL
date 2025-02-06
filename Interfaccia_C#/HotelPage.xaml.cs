using Interfaccia_C_.Model; // Importa il modello Hotel

using Interfaccia_C_.ViewModel;
namespace Interfaccia_C_;

public partial class HotelPage : ContentPage

{ 
    public HotelPage(Hotel hotel)
    {
        InitializeComponent();

        BindingContext = new HotelPageViewModel();
    }
}

