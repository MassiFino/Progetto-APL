namespace Interfaccia_C_;
using Interfaccia_C_.ViewModel; 

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginPageViewModel();
    }

}