using Interfaccia_C_.ViewModels;

namespace Interfaccia_C_
{
    public partial class AppShell : Shell
    {
        private readonly AppShellViewModel _viewModel;

        public AppShell()
        {
            InitializeComponent();
            _viewModel = new AppShellViewModel(this);
        }
    }
}
