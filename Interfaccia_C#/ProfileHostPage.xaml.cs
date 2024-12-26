using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class ProfileHostPage : ContentPage
{
    public ProfileHostPage()
    {
        InitializeComponent();
        BindingContext = new ProfileHostPageViewModel();
    }

    
}
