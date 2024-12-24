using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
        BindingContext = new RegisterPageViewModel();
    }

}
