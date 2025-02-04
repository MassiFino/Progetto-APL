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
using CommunityToolkit.Mvvm.ComponentModel;


namespace Interfaccia_C_.ViewModel
{
   public class AddHotelPageViewModel : INotifyPropertyChanged
    {
        private string userName;

        public string UserName
        {
            get => userName;
            set
            {
                if (userName != value)
                {
                    Debug.WriteLine($"UserName changing from {userName} to {value}"); // Debugging line
                    userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
