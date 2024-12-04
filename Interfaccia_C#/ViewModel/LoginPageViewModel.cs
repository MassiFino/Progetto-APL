using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Interfaccia_C_.ViewModel
{
    public partial class LoginPageViewModel
    {

        public ICommand GoToRegisterCommand { get; }

        public LoginPageViewModel()
        {
            // Inizializza il comando con la logica di navigazione
            GoToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//RegisterPage"));
        }

        // Aggiungi altre proprietà e logica se necessario
    }
}
