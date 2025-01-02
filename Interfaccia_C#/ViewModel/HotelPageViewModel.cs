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
using Interfaccia_C_.Model;


namespace Interfaccia_C_.ViewModel
{
    public class HotelPageViewModel
    { 
        
        public string name { get; }
        public string location { get; }
        public string description { get; }
        public List<string> services { get; }
        public double rating { get; }
        public string image { get; }

        // Costruttore che accetta un oggetto Hotel
        public HotelPageViewModel(Hotel hotel)
        {
            name = hotel.name;
            location = hotel.location;
            description = hotel.description;
            services = hotel.services;
            rating = hotel.rating;
            image = hotel.images;
        }
    }
}