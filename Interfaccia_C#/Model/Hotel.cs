namespace Interfaccia_C_.Model
{
    public class Hotel
    {
        public int HotelID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public List<string> Services { get; set; }
        public double Rating { get; set; }
        public ImageSource ImageSource { get; set; }

        public string ServiziStringa { get; set; }
        public string Images { get; set; }  // Stringa che contiene il path dell'immagine
        public float Prezzo { get; set; }
        public int NumeroPrenotazioni { get; set; }

    }


}