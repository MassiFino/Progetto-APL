using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class HotelPage : ContentPage
{
    public HotelPage()
    {
        InitializeComponent();
        BindingContext = new HotelPageViewModel();
    }

}
