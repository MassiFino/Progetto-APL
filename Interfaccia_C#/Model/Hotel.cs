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
    }


    // Classe per la risposta dell'API
    public class UserResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string PImage { get; set; }

    }
    }