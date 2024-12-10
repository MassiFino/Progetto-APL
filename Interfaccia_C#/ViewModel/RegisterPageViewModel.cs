using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Text.Json;
using System.Diagnostics;

namespace Interfaccia_C_.ViewModel
{

    public class RegisterPageViewModel
    {
        // Proprietà per il comando
        public ICommand GoToLoginCommand { get; }

        // Costruttore
        public RegisterPageViewModel()
        {
            // Inizializza il comando con la logica di navigazione
            GoToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("//LoginPage"));
        }
    }
}
