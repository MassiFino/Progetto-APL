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
