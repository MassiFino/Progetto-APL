package types


type SignUpRequest struct {
	Username string `json:"Username"`
	Password string `json:"Password"`
}

type SignUpResponse struct {
	Success bool `json:"status"`
	Message string `json:"message"`
}
