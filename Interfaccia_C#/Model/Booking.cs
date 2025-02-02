using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Interfaccia_C_.Model
{


    public class Booking: INotifyPropertyChanged
    {
        public int bookingID { get; set; }  // Identificativo univoco della prenotazione
        public int roomID { get; set; }
        public string username { get; set; }  // Nome utente del cliente
        public DateTime checkInDate { get; set; }  // Data di check-in
        public DateTime checkOutDate { get; set; }  // Data di check-out
        public decimal totalAmount { get; set; }  // Importo totale della prenotazione
        public string status { get; set; }  // Stato della prenotazione
        public string roomName { get; set; }  // Nome della stanza
        public string roomImage { get; set; }  // Nome della stanza

        public string hotelName { get; set; }  // URL o percorso dell'immagine della stanza
        public string hotelLocation { get; set; }
        public ImageSource ImageSource { get; set; }

        // Proprietà UI: Sezione “Lascia Recensione”
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
        public class Review
        {
            public string createdAt { get; set; }  // Data della recensione
            public string review { get; set; }     // Commento della recensione
            public decimal rating { get; set; }     // Voto della recensione
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
                    OnPropertyChanged();  // Assicurati che la UI venga aggiornata
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
                OnPropertyChanged(); // Assicurati che la UI venga aggiornata
            }
        }

        private string _review;
        public string review
        {
            get { return _review; }
            set
            {
                _review = value;
                OnPropertyChanged(); // Assicurati che la UI venga aggiornata
            }
        }

        private decimal _rating;
        public decimal voto
        {
            get { return _rating; }
            set
            {
                _rating = value;
                OnPropertyChanged(); // Assicurati che la UI venga aggiornata
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

                    // Debug per vedere se il valore cambia
                    Debug.WriteLine($"MessageReview aggiornato: {_messageReview}");
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