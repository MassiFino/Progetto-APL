package main

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"net/http"
	"progetto-go/types"
	"progetto-go/database"
)

var db *sql.DB // Variabile globale per la connessione al database

func SignUpHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.SignUpRequest

	err := json.NewDecoder(r.Body).Decode(&req)
	if err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	fmt.Printf("Dati ricevuti: %+v\n", req)

	// Connessione al database (se non già connesso)
	if db == nil {
		db, err = database.ConnectDB("root", "1234", "db", "3306", "bookroom_db")
		if err != nil {
			http.Error(w, "Errore di connessione al database", http.StatusInternalServerError)
			return
		}
	}

	// Verifica le credenziali dell'utente
	valid, err := database.CheckUserCredentials(db, req.Username, req.Password)
	if err != nil {
		http.Error(w, "Errore interno del server", http.StatusInternalServerError)
		return
	}

	if !valid {
		http.Error(w, "Credenziali non valide", http.StatusUnauthorized)
		return
	}

	// Login riuscito
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(`{"status": "success", "message": "Login effettuato con successo!"}`))
}

func main() {
	http.HandleFunc("/Signup", SignUpHandler)

	// Avvia il server sulla porta 8080
	port := "8080"
	fmt.Printf("Server in ascolto su http://localhost:%s\n", port)

	err := http.ListenAndServe(":"+port, nil)
	if err != nil {
		fmt.Println("Errore durante l'avvio del server:", err)
	}
}
