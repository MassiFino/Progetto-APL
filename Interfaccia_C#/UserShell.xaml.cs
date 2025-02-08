namespace Interfaccia_C_;
using Interfaccia_C_.ViewModel;

public partial class UserShell : Shell
    {
        public UserShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("HotelPage", typeof(HotelPage));
        Routing.RegisterRoute("SearchPage", typeof(SearchPage));

        BindingContext = new UserShellViewModel();
        }
    }
    



