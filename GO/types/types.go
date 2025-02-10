package types

type LogInRequest struct {
	Username string `json:"Username"`
	Password string `json:"Password"`
}

type LogInResponse struct {
	Success bool   `json:"status"`
	Message string `json:"message"`
	Role    string `json:"Role"`
}

type SignUpRequest struct {
	Username string  `json:"Username"`
	Password string  `json:"Password"`
	Email    string  `json:"Email"`
	PImage   *string `json:"PImage"`
	Role     string  `json:"Role"`
}

type SignUpResponse struct {
	Success bool   `json:"status"`
	Message string `json:"message"`
}

type UserRequest struct {
	Username string `json:"username"`
}

type UserResponse struct {
	Username string `json:"Username"`
	Email    string `json:"Email"`
	PImage   string `json:"PImage"`
	Role     string `json:"Role"`
}

type HotelRequest struct {
	Username string `json:"username"`
}

type HotelResponse struct {
	Name        string   `json:"Name"`             // Nome dell'hotel
	Location    string   `json:"Location"`         // Posizione dell'hotel
	Description string   `json:"Description"`      // Descrizione dettagliata dell'hotel
	Services    []string `json:"Services"`         // Elenco predefinito dei servizi
	Rating      float64  `json:"Rating,omitempty"` // Valutazione dell'hotel (opzionale)
	Images      string   `json:"Images"`           // URL delle immagini dell'hotel
}
type BookingRequest struct {
	Username string `json:"username"`
}
type BookingResponse struct {
	BookingID     int     `json:"bookingID"`
	RoomID        int     `json:"roomID"`
	Username      string  `json:"username"`
	CheckInDate   string  `json:"checkInDate"`
	CheckOutDate  string  `json:"checkOutDate"`
	TotalAmount   float64 `json:"totalAmount"`
	Status        string  `json:"status"`
	RoomName      string  `json:"roomName"`
	RoomImage     string  `json:"roomImage"` // URL delle immagini della stanza
	HotelName     string  `json:"hotelName"`
	HotelLocation string  `json:"hotelLocation"`
}

type ReviewRequest struct {
	RoomID   int    `json:"roomID"`
	Username string `json:"Username"`
	Comment  string `json:"comment"`
	Rating   int    `json:"rating"`
}

type ReviewResponse struct {
	Success bool   `json:"status"`
	Message string `json:"message"`
}

type ReviewReq struct {
	RoomID   int    `json:"roomID"`
	Username string `json:"Username"`
}
type ReviewResp struct {
	Comment   string  `json:"review"`    // Il campo "review" sarà trattato come stringa
	Rating    float64 `json:"rating"`    // Rating come numero decimale
	CreatedAt string  `json:"createdAt"` // Timestamp convertito come stringa
}

type DeleteBookingRequest struct {
	BookingID int    `json:"BookingID"`
	Username  string `json:"Username"`
}

type DeleteReviewRequest struct {
	RoomID   int    `json:"RoomID"`
	Username string `json:"Username"`
}

type ResponseMeta struct {
	NomeMeta           string  `json:"Name"`
	NumeroHotel        int     `json:"NumeroHotel"`
	PrezzoMedio        float64 `json:"Prezzo"`
	Immagine           string  `json:"Images"`
	NumeroPrenotazioni int     `json:"NumeroPrenotazioni"`
	MediaVoto          float64 `json:"Rating"`
}

type ResponseOffertaImperdibile struct {
	HotelID      int     `json:"HotelID"`
	NomeHotel    string  `json:"Name"`
	Immagine     string  `json:"Images"`
	PrezzoMinimo float64 `json:"Prezzo"`
	MediaVoto    float64 `json:"Rating"` // Nuovo campo per il voto medio
}

type RoomHotelRequest struct {
	HotelName       string   `json:"HotelName"`
	Location        string   `json:"Location"`
	Description     string   `json:"Description"`
	Services        []string `json:"Services"`
	HotelImagePath  string   `json:"HotelImagePath"`
	RoomName        string   `json:"RoomName"`
	RoomDescription string   `json:"RoomDescription"`
	PricePerNight   float64  `json:"PricePerNight"`
	MaxGuests       int      `json:"MaxGuests"`
	RoomType        string   `json:"RoomType"`
	RoomImagePath   string   `json:"RoomImagePath"`
	HostHotel       string   `json:"HostHotel"`
}

type RoomRequest struct {
	HotelName       string  `json:"HotelName"`
	RoomName        string  `json:"RoomName"`
	RoomDescription string  `json:"RoomDescription"`
	PricePerNight   float64 `json:"PricePerNight"`
	MaxGuests       int     `json:"MaxGuests"`
	RoomType        string  `json:"RoomType"`
	RoomImagePath   string  `json:"RoomImage"`
}

type SearchRequest struct {
	City         string   `json:"City"`
	CheckInDate  string   `json:"CheckInDate"`
	CheckOutDate string   `json:"CheckOutDate"`
	Guests       int      `json:"Guests"`
	Services     []string `json:"Services"`
	OrderBy      *string  `json:"OrderBy,omitempty"` // Campo opzionale
}
