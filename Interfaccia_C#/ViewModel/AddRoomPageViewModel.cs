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
    public class AddRoomPageViewModel
    {
        public string Name { get; }
        public string Location { get; }
        private string _roomImagePath;
        private string _roomImageNames;

        public string RoomImagePath
        {
            get => _roomImagePath;
            set
            {
                _roomImagePath = value;
                OnPropertyChanged();
            }
        }
        public string RoomImageNames
        {
            get => _roomImageNames;
            set
            {
                _roomImageNames = value;
                OnPropertyChanged();
            }
        }
        public string ServiziStringa { get; set; }
        public ICommand AddRoomCommand { get; set; }
        public ICommand UploadRoomImageCommand { get; }

        public AddRoomPageViewModel(Hotel hotel)
        {
            Name = hotel.Name;
            Location = hotel.Location;
            AddRoomCommand = new Command<Hotel>(AddRoom);
            UploadRoomImageCommand = new Command(async () => await OnUploadRoomImage());

        }


        private void AddRoom(Hotel selectedHotel)
        {

        }
        private async Task OnUploadRoomImage()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Seleziona un'immagine per la stanza",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                    var localPath = Path.Combine(projectDirectory, "Pictures", "RoomPictures");
                    Directory.CreateDirectory(localPath);

                    var filePath = Path.Combine(localPath, result.FileName);
                    var relativePath = Path.Combine("Pictures", "RoomPictures", result.FileName);

                    using (var stream = await result.OpenReadAsync())
                    using (var localFile = File.Create(filePath))
                    {
                        await stream.CopyToAsync(localFile);
                    }

                    RoomImagePath = relativePath;
                    RoomImageNames = $"Immagine caricata: {result.FileName}";
                }
            }
            catch (Exception ex)
            {
                RoomImagePath = null;
                RoomImageNames = $"Errore: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
