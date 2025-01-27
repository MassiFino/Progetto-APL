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
    public  class SearchPageViewModel
    {
        private string _city;
        private DateTime _checkInDate;
        private DateTime _checkOutDate;
        private bool _isInternetEnabled;
        private bool _isParkingEnabled;
        private bool _isGymEnabled;
        private bool _isSpaEnabled;

        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        public DateTime CheckInDate
        {
            get => _checkInDate;
            set => SetProperty(ref _checkInDate, value);
        }

        public DateTime CheckOutDate
        {
            get => _checkOutDate;
            set => SetProperty(ref _checkOutDate, value);
        }

        public bool IsInternetEnabled
        {
            get => _isInternetEnabled;
            set => SetProperty(ref _isInternetEnabled, value);
        }

        public bool IsParkingEnabled
        {
            get => _isParkingEnabled;
            set => SetProperty(ref _isParkingEnabled, value);
        }

        public bool IsGymEnabled
        {
            get => _isGymEnabled;
            set => SetProperty(ref _isGymEnabled, value);
        }

        public bool IsSpaEnabled
        {
            get => _isSpaEnabled;
            set => SetProperty(ref _isSpaEnabled, value);
        }

        // Metodo per eseguire la ricerca
        public ICommand SearchCommand { get; }

        public SearchPageViewModel()
        {
            // Inizializza il comando per la ricerca
            SearchCommand = new Command(OnSearch);
        }

        private void OnSearch()
        {
            // Qui raccogli i valori dalla proprietà e fai la ricerca
            string city = City;
            DateTime checkIn = CheckInDate;
            DateTime checkOut = CheckOutDate;
            bool isInternetEnabled = IsInternetEnabled;
            bool isParkingEnabled = IsParkingEnabled;
            bool isGymEnabled = IsGymEnabled;
            bool isSpaEnabled = IsSpaEnabled;

            // Esegui la logica della ricerca con questi valori
            // Esempio: chiamare un servizio per cercare gli hotel in base ai filtri
        }

        // Metodo di supporto per la notifica delle proprietà
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }
}

