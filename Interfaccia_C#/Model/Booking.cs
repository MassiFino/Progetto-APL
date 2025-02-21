using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Interfaccia_C_.Model
{


    public class Booking: INotifyPropertyChanged
    {
        public int bookingID { get; set; }  
        public int roomID { get; set; }
        public string username { get; set; }  // Nome utente del cliente
        public DateTime checkInDate { get; set; }  
        public DateTime checkOutDate { get; set; }  
        public double totalAmount { get; set; }  // Importo totale della prenotazione
        public string status { get; set; }  // Stato della prenotazione
        public string roomName { get; set; } 
        public string roomImage { get; set; }

        public string hotelName { get; set; }  // URL o percorso dell'immagine della stanza
        public string hotelLocation { get; set; }
        public ImageSource ImageSource { get; set; }

        public int hotelID { get; set; }
        
        public string roomType { get; set; }

        public int guests { get; set; }

        public int nights { get; set; }
        private bool isReviewVisible;
        public bool IsReviewVisible
        {
            get => isReviewVisible;
            set
            {
                if (isReviewVisible != value)
                {
                    isReviewVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private int rating;
        public int Rating
        {
            get => rating;
            set
            {
                if (rating != value)
                {
                    rating = value;
                    OnPropertyChanged();
                }
            }
        }

        private string comment;
        public string Comment
        {
            get => comment;
            set
            {
                if (comment != value)
                {
                    comment = value;
                    OnPropertyChanged();
                }
            }
        }
     

        private bool _isReviewSectionVisible;
        public bool IsReviewSectionVisible
        {
            get { return _isReviewSectionVisible; }
            set
            {
                if (_isReviewSectionVisible != value)
                {
                    _isReviewSectionVisible = value;
                    OnPropertyChanged();  
                }
            }
        }
        private string _createdAt;
        public string createdAt
        {
            get { return _createdAt; }
            set
            {
                _createdAt = value;
                OnPropertyChanged(); 
            }
        }

        private string _review;
        public string review
        {
            get { return _review; }
            set
            {
                _review = value;
                OnPropertyChanged(); 
            }
        }

        private decimal _rating;
        public decimal voto
        {
            get { return _rating; }
            set
            {
                _rating = value;
                OnPropertyChanged(); 
            }
        }


        private string _messageReview;

        public string MessageReview
        {
            get => _messageReview;
            set
            {
                if (_messageReview != value)
                {
                    _messageReview = value;
                    OnPropertyChanged(nameof(MessageReview));

                    Debug.WriteLine($"MessageReview aggiornato: {_messageReview}");
                }
            }
        }


        private bool _isDeleteReviewVisible = false; // default false: se non esiste la recensione, il pulsante è nascosto
        public bool IsDeleteReviewVisible
        {
            get => _isDeleteReviewVisible;
            set
            {
                if (_isDeleteReviewVisible != value)
                {
                    _isDeleteReviewVisible = value;
                    OnPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}