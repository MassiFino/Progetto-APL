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
	Points   int    `json:"Points"`
}

type HotelRequest struct {
	Username string `json:"Username"`
}

type HotelResponse struct {
	HotelID     int      `json:"HotelID"`
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
	Comment   string  `json:"review"`
	Rating    float64 `json:"rating"`
	CreatedAt string  `json:"createdAt"`
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
	HotelID            int      `json:"HotelID"`
	Name               string   `json:"Name"`
	Location           string   `json:"Location"`
	Description        string   `json:"Description"`
	Services           []string `json:"Services"`
	Images             string   `json:"Images"`
	PrezzoMedio        float64  `json:"Prezzo"`
	NumeroPrenotazioni int      `json:"NumeroPrenotazioni"`
	MediaVoto          float64  `json:"Rating"`
}

type ResponseOffertaImperdibile struct {
	HotelID      int      `json:"HotelID"`
	Name         string   `json:"Name"`
	Description  string   `json:"Description"`
	Services     []string `json:"Services"`
	Location     string   `json:"Location"`
	Images       string   `json:"Images"`
	PrezzoMinimo float64  `json:"Prezzo"`
	MediaVoto    float64  `json:"Rating"`
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
	RoomImagePath   string  `json:"RoomImagePath"`
}

type SearchRequest struct {
	City         string   `json:"City"`
	CheckInDate  string   `json:"CheckInDate"`
	CheckOutDate string   `json:"CheckOutDate"`
	Guests       int      `json:"Guests"`
	Services     []string `json:"Services"`
	OrderBy      *string  `json:"OrderBy,omitempty"` // Campo opzionale
}

type SearchResponse struct {
	HotelID     int      `json:"HotelID"`
	Name        string   `json:"Name"`
	Location    string   `json:"Location"`
	Description string   `json:"Description"`
	Services    []string `json:"Services"`
	Rating      float64  `json:"Rating,omitempty"`
	Images      string   `json:"Images"`
	Price       float64  `json:"Prezzo"`
}

type Room struct {
	RoomID          int     `json:"RoomID"`
	RoomName        string  `json:"RoomName"`
	RoomDescription string  `json:"RoomDescription"`
	PricePerNight   float64 `json:"PricePerNight"`
	MaxGuests       int     `json:"MaxGuests"`
	RoomType        string  `json:"RoomType"`
	Images          string  `json:"Images"`
}

type Review struct {
	RoomID    int     `json:"RoomID"`
	RoomName  string  `json:"RoomName"`
	Username  string  `json:"Username"`
	Comment   string  `json:"Comment"`
	Rating    float64 `json:"Rating"`
	CreatedAt string  `json:"CreatedAt"`
}

type GetRoomsReviewsRequest struct {
	HotelID int `json:"HotelID"`
}

type GetRoomsUserRequest struct {
	HotelID  int    `json:"HotelID"`
	Username string `json:"Username"`
}

type AvailableRoomsRequest struct {
	HotelID      int    `json:"HotelID"`
	CheckInDate  string `json:"CheckInDate"`
	CheckOutDate string `json:"CheckOutDate"`
	Username     string `json:"Username"`
}

type RoomUser struct {
	RoomID          int     `json:"RoomID"`
	RoomName        string  `json:"RoomName"`
	RoomDescription string  `json:"RoomDescription"`
	PricePerNight   float64 `json:"PricePerNight"`
	MaxGuests       int     `json:"MaxGuests"`
	RoomType        string  `json:"RoomType"`
	Images          string  `json:"Images"`
	IsInterestSet   bool    `json:"IsInterestSet"` // Indica se l'utente ha già impostato l'interesse
}

type BookingRoomRequest struct {
	Username     string  `json:"Username"`
	RoomID       int     `json:"RoomID"`
	CheckInDate  string  `json:"CheckInDate"`
	CheckOutDate string  `json:"CheckOutDate"`
	TotalAmount  float64 `json:"TotalAmount"`
	Status       string  `json:"Status"`
}

type UpdatePointsRequest struct {
	Username    string `json:"Username"`
	PointsToAdd int    `json:"PointsToAdd"`
}

type SetInterestRequest struct {
	Username     string  `json:"Username"`
	RoomID       int     `json:"RoomID"`
	MonitorValue float64 `json:"MonitorValue"`
}

type SetInterestResponse struct {
	InterestID   int     `json:"InterestID"`
	Username     string  `json:"Username"`
	RoomName     string  `json:"RoomName"`  // Nome della stanza
	HotelName    string  `json:"HotelName"` // Nome dell'hotel
	MonitorValue float64 `json:"MonitorValue"`
}

type UpcomingBooking struct {
	BookingID   int
	Username    string
	RoomName    string
	RoomID      int
	CheckInDate string
	Email       string
}

type AveragePriceRequest struct {
	RoomType string `json:"RoomType"`
	Location string `json:"Location"`
}
type DeleteRoomRequest struct {
	RoomID   int    `json:"RoomID"`
	Username string `json:"Username"`
}

type UpdateHotelDescriptionRequest struct {
	HotelID        int    `json:"HotelID"`
	NewDescription string `json:"NewDescription"`
	Username       string `json:"Username"`
}

type CostData struct {
	Month string  `json:"month"`
	Cost  float64 `json:"cost"`
}

type GetInterestsRequest struct {
	Username string `json:"Username"`
}

type InterestResponse struct {
	InterestID   int     `json:"InterestID"`
	Username     string  `json:"Username"`
	RoomID       int     `json:"RoomID"`
	RoomName     string  `json:"RoomName"`
	HotelName    string  `json:"HotelName"`
	MonitorValue float64 `json:"MonitorValue"`
}

type DeleteInterestRequest struct {
	InterestID int    `json:"InterestID"`
	Username   string `json:"Username"`
}

type UpdateRoomPriceRequest struct {
	RoomID   int     `json:"RoomID"`
	NewPrice float64 `json:"NewPrice"`
	Username string  `json:"Username"`
}

type HotelBookingsRequest struct {
	HotelID int `json:"HotelID"`
}

type BookingHotelResponse struct {
	BookingID    int     `json:"bookingID"`
	RoomID       int     `json:"roomID"`
	Username     string  `json:"username"`
	CheckInDate  string  `json:"checkInDate"`
	CheckOutDate string  `json:"checkOutDate"`
	TotalAmount  float64 `json:"totalAmount"`
	RoomName     string  `json:"roomName"`
}
