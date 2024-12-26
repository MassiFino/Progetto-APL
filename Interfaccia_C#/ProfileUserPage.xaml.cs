using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class ProfileUserPage : ContentPage
{
    public ProfileUserPage()
    {
        InitializeComponent();
        BindingContext = new ProfileUserPageViewModel(); 
    }

}
