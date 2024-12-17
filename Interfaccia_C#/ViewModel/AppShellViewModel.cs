using Microsoft.Maui.Controls;

namespace Interfaccia_C_.ViewModels
{
    /// <summary>
    /// ViewModel per gestire la visibilità dinamica della barra laterale (Flyout) 
    /// nella navigazione dell'applicazione. La barra laterale viene nascosta 
    /// nelle pagine di Login e Registrazione e mostrata solo quando necessario.
    /// </summary>
    public class AppShellViewModel
    {
        
        private readonly Shell _shell;

        /// <summary>
        /// Costruttore del ViewModel. Viene passata un'istanza di Shell 
        /// per gestire la logica della navigazione dinamica.
        /// </summary>
        /// <param name="shell">L'istanza di Shell principale dell'applicazione.</param>
        public AppShellViewModel(Shell shell)
        {/*
            _shell = shell;

            // Registriamo l'evento Navigated per rilevare ogni cambio di navigazione
            _shell.Navigated += HandleNavigation;
        }

        /// <summary>
        /// Metodo chiamato ogni volta che l'utente naviga verso una nuova pagina.
        /// Determina se la barra laterale deve essere mostrata o nascosta 
        /// basandosi sulla pagina corrente.
        /// </summary>
        /// <param name="sender">L'oggetto che ha scatenato l'evento.</param>
        /// <param name="e">Contiene informazioni sulla pagina corrente navigata.</param>
        private void HandleNavigation(object sender, ShellNavigatedEventArgs e)
        {
            // Controlla se l'utente è nelle pagine di Login o Registrazione
            if (e.Current.Location.ToString().Contains("LoginPage") ||
                e.Current.Location.ToString().Contains("RegisterPage"))
            {
                // Se l'utente si trova in LoginPage o RegisterPage, disabilita la barra laterale
                _shell.FlyoutBehavior = FlyoutBehavior.Disabled;
            }
            else
            {
                // Altrimenti, abilita la barra laterale
                _shell.FlyoutBehavior = FlyoutBehavior.Flyout;
            }*/
        }
    }
}
