namespace Interfaccia_C_.Model
{


    public class Booking
    {
        public int bookingID { get; set; }  // Identificativo univoco della prenotazione
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


    }
}