package types

type LogInRequest struct {
	Username string `json:"Username"`
	Password string `json:"Password"`
}

type LogInResponse struct {
	Success bool   `json:"status"`
	Message string `json:"message"`
}

type SignUpRequest struct {
	Username string `json:"Username"`
	Password string `json:"Password"`
	Email    string `json:"Email"`
}

type SignUpResponse struct {
	Success bool   `json:"status"`
	Message string `json:"message"`
}
