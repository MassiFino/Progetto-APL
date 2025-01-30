namespace Interfaccia_C_.Model
{
    public class Hotel
    {
        public string name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public List<string> services { get; set; }
        public double rating { get; set; }
        public string images { get; set; }
        public ImageSource ImageSource { get; set; } // Aggiungi questa proprietà

    }


    }