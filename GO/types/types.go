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
	Name        string   `json:"name"`             // Nome dell'hotel
	Location    string   `json:"location"`         // Posizione dell'hotel
	Description string   `json:"description"`      // Descrizione dettagliata dell'hotel
	Services    []string `json:"services"`         // Elenco predefinito dei servizi
	Rating      float64  `json:"rating,omitempty"` // Valutazione dell'hotel (opzionale)
	Images      string   `json:"images"`           // URL delle immagini dell'hotel
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
