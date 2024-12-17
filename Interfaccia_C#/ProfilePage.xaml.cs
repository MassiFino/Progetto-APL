using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = new ProfilePageViewModel(); 
    }
}
