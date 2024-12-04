using System.Windows.Input;

namespace Interfaccia_C_.ViewModel;

public class RegisterPageViewModel
{
    public ICommand GoToLoginCommand { get; }
    public RegisterPageViewModel()
    {

        // Inizializza il comando con la logica di navigazione
        GoToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));

    }
}
