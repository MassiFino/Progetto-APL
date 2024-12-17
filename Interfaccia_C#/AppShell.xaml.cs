using Microsoft.Maui.Controls;
using Interfaccia_C_.ViewModels;  // Assicurati di usare il namespace giusto

namespace Interfaccia_C_
{
    public partial class AppShell : Shell
    {
        public AppShellViewModel ViewModel { get; set; }

        public AppShell()
        {
            InitializeComponent();
            ViewModel = new AppShellViewModel(this); // Inizializza il ViewModel passando l'istanza di Shell
            BindingContext = ViewModel;  // Imposta il BindingContext per l'AppShell
        }
    }
}