using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel(); 
    }
}
