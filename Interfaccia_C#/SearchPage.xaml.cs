using Interfaccia_C_.ViewModel;

namespace Interfaccia_C_;

public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
        InitializeComponent();
        BindingContext = new SearchPageViewModel(); 
    }
}
