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
       
        public string Name { get; }
        public string Location { get; }
        public string Description { get; }
        public double Rating { get; }
        public string Images { get; set; }  // Stringa che contiene il path (o i path)
        public ImageSource ImageSource { get; set; }

        public string ServiziStringa { get; set; }

        // Costruttore che accetta un oggetto Hotel
        public HotelPageViewModel(Hotel hotel)
        {
            

            Name = hotel.Name;
            Location = hotel.Location;
            Description = hotel.Description;
            Rating = hotel.Rating;
            

            if (!string.IsNullOrEmpty(hotel.Images?.Trim()))
            {
                // Divido la stringa in più immagini
                var imageList = hotel.Images.Split(',');
                // Prendo la prima
                var firstImage = imageList[0].Trim();

                Debug.WriteLine("Path immagine (prima del combine): " + firstImage);

                // Costruisco path completo
                string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;
                var imagePath = Path.Combine(projectDirectory, firstImage);
                Debug.WriteLine("Path completo: " + imagePath);

                // Converto in ImageSource
                ImageSource = GetImageSource(imagePath);
                Debug.WriteLine("Immaginissima: " + hotel.ImageSource);
            }
            
        }


        public ImageSource GetImageSource(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                Debug.WriteLine("Immagine trovata: " + imagePath);
                return ImageSource.FromFile(imagePath);
            }
            else
            {
                return null;
            }
        }
    }


}