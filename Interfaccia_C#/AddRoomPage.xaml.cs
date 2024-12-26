using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class AddRoomPage : ContentPage
{
    public AddRoomPage()
    {
        InitializeComponent();
        BindingContext = new AddRoomPageViewModel(); 
    }
}
