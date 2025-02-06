using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class AddHotelPage : ContentPage
{

    public AddHotelPage()
    {
        InitializeComponent();
        BindingContext = new AddHotelPageViewModel();
    }



}

