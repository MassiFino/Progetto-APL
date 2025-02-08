namespace Interfaccia_C_;
using Interfaccia_C_.ViewModel;

public partial class HostShell : Shell
    {

        public HostShell()
        {
        InitializeComponent();
        BindingContext = new HostShellViewModel();
        Routing.RegisterRoute("HotelPage", typeof(HotelPage));
        Routing.RegisterRoute("AddHotelPage", typeof(AddHotelPage));
        Routing.RegisterRoute("AddRoomPage", typeof(AddRoomPage));


    }

}
         