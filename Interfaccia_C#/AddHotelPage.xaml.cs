using Interfaccia_C_.ViewModel;
using System.Diagnostics;

namespace Interfaccia_C_;

public partial class AddHotelPage : ContentPage
{
    public AddHotelPageViewModel ViewModel { get; private set; }
            public string UserName { get; set; }

    public AddHotelPage()
    {
        InitializeComponent();
        BindingContext = new AddHotelPageViewModel(); 
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var queryParams = Shell.Current?.CurrentState?.Location?.OriginalString;

        if (!string.IsNullOrEmpty(queryParams))
        {
            Uri uri = new Uri(queryParams);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

            // Check for UserName
            string userName = query["UserName"];

            if (!string.IsNullOrEmpty(userName))
            {
                Debug.WriteLine($"Received userName: {userName}");
                // Optionally, set it to your ViewModel here
                ViewModel.UserName = userName;
            }
            else
            {
                Debug.WriteLine("UserName parameter not found.");
            }
        }
        else
        {
            Debug.WriteLine("No query parameters found.");
        }
    }
}
