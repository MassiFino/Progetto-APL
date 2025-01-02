package main

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"progetto-go/database"
	"progetto-go/types"
)

var db *sql.DB // Variabile globale per la connessione al database

func initializeDatabase() *sql.DB {
	db, err := database.ConnectDB("root", "1234", "db", "3306", "bookroom_db")
	if err != nil {
		log.Fatalf("Errore durante la connessione al database: %v", err)
	}
	return db
}

func LogInHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.LogInRequest

	err := json.NewDecoder(r.Body).Decode(&req)
	if err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	fmt.Printf("Dati ricevuti: %+v\n", req)

	// Verifica le credenziali dell'utente
	valid, err, Role := database.CheckUserCredentials(db, req.Username, req.Password)
	if err != nil {
		// Gestisce l'errore interno del server
		http.Error(w, "Errore interno del server", http.StatusInternalServerError)
		return
	}

	if !valid {
		// Gestisce le credenziali non valide
		http.Error(w, "Credenziali non valide", http.StatusUnauthorized)
		return
	}
	fmt.Printf("Dati : %s\n", Role)

	// Login riuscito
	w.Header().Set("Content-Type", "application/json")
	response := fmt.Sprintf(`{"status": "success","message": "Login effettuato con successo!","role": "%s"}`, Role)
	fmt.Println(response) // Usa fmt.Println invece di fmt.Printf per stampare la risposta

	// Scrivi la risposta JSON con w.Write
	w.Write([]byte(response))
}

func SignUPHandler(w http.ResponseWriter, r *http.Request) {

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

	// Verifica se l'utente esiste già
	exists, err := database.CheckUserExists(db, req.Username, req.Email)
	if err != nil {
		http.Error(w, "Errore interno del server", http.StatusInternalServerError)
		return
	}

	if exists {
		http.Error(w, "Utente già esistente", http.StatusConflict)
		return
	}

	// Registra l'utente
	err = database.RegisterUser(db, req.Username, req.Password, req.Email, req.Role, req.PImage)
	if err != nil {
		http.Error(w, "Errore interno del server", http.StatusInternalServerError)
		return
	}

	// Registrazione riuscita
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(`{"status": "success", "message": "Registrazione effettuata con successo!"}`))
}

func getUserDataHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}
	// Definiamo una struttura per l'email che riceviamo nel corpo della richiesta
	var req types.UserRequest
	err := json.NewDecoder(r.Body).Decode(&req)
	if err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}
	fmt.Printf("Dati ricevuti: %+v\n", req)
	// Recupera i dati dell'utente dal database usando la funzione GetUser
	user, err := database.GetUser(db, req.Username)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore durante la ricerca dell'utente: %v", err), http.StatusInternalServerError)
		return
	}
	fmt.Println("Dati dell'utente recuperati:", user)

	// Se l'utente non viene trovato, restituisci un errore
	if user == nil {
		http.Error(w, "Utente non trovato", http.StatusNotFound)
		return
	}

	// Rispondi con i dati dell'utente in formato JSON
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusOK)
	err = json.NewEncoder(w).Encode(user)
	if err != nil {
		http.Error(w, "Errore durante la codifica della risposta JSON", http.StatusInternalServerError)
	}
}

func getHotelsHostHandler(w http.ResponseWriter, r *http.Request) {
	// Verifica che il metodo sia POST
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	// Definiamo una struttura per l'email che riceviamo nel corpo della richiesta
	var req types.UserRequest
	err := json.NewDecoder(r.Body).Decode(&req)
	if err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}
	fmt.Printf("email ricevuta: %+v\n", req.Username)

	// Recupera gli hotel dal database usando la funzione getHotelsByHost
	hotels, err := database.GetHotelsHost(db, req.Username)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore durante la ricerca degli hotel: %v", err), http.StatusInternalServerError)
		return
	}

	// Se non sono stati trovati hotel, restituisci un errore
	if len(hotels) == 0 {
		http.Error(w, "Nessun hotel trovato per questo utente", http.StatusNotFound)
		return
	}
	// Stampa degli hotel trovati per il debug
	for i, hotel := range hotels {
		fmt.Printf("Hotel #%d: %+v\n", i+1, hotel)
	}

	// Rispondi con i dati degli hotel in formato JSON
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusOK)
	err = json.NewEncoder(w).Encode(hotels)
	if err != nil {
		http.Error(w, "Errore durante la codifica della risposta JSON", http.StatusInternalServerError)
	}
}

func main() {
	db = initializeDatabase() // Inizializza la connessione al database
	defer db.Close()          // Chiude la connessione al termine del server

	http.HandleFunc("/login", LogInHandler)
	http.HandleFunc("/signup", SignUPHandler)
	http.HandleFunc("/getUserData", getUserDataHandler)
	http.HandleFunc("/getHotelsHost", getHotelsHostHandler)

	port := "8080"
	fmt.Printf("Server in ascolto su http://localhost:%s\n", port)

	if err := http.ListenAndServe(":"+port, nil); err != nil {
		log.Fatalf("Errore durante l'avvio del server: %v", err)
	}
}
