package main

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"progetto-go/database"
	"progetto-go/types"
	"progetto-go/utils"
	"time"

	"gopkg.in/gomail.v2"
)

var db *sql.DB // Variabile globale per la connessione al database

var emailDialer *gomail.Dialer

func initializeDatabase() *sql.DB {
	db, err := database.ConnectDB("root", "1234", "db", "3306", "bookroom_db")
	if err != nil {
		log.Fatalf("Errore durante la connessione al database: %v", err)
	}
	return db
}
func initializeEmail() {
	emailDialer = gomail.NewDialer("smtp.gmail.com", 587, "universita012@gmail.com", "hksk dmnn vgxq wewm")
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
	//fmt.Printf("Dati ricevuti: %+v\n", req)
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

	fmt.Printf("Username ricevuta: %+v\n", req.Username)

	// Recupera gli hotel dal database usando la funzione getHotelsByHost
	hotels, err := database.GetHotelsHost(db, req.Username)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore durante la ricerca degli hotel: %v", err), http.StatusInternalServerError)
		return
	}

	// Se non sono stati trovati hotel, restituisci comunque una risposta di successo con lista vuota
	if len(hotels) == 0 {
		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusOK)             // Risposta con successo
		err := json.NewEncoder(w).Encode(hotels) // Rispondi con la lista vuota di hotel
		if err != nil {
			http.Error(w, "Errore durante la codifica della risposta JSON", http.StatusInternalServerError)
		}
		return
	}

	fmt.Println("Hotel trovati: ", hotels)

	// Rispondi con i dati degli hotel in formato JSON
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusOK)
	err = json.NewEncoder(w).Encode(hotels)
	if err != nil {
		http.Error(w, "Errore durante la codifica della risposta JSON", http.StatusInternalServerError)
	}
}
func getBookingsHandler(w http.ResponseWriter, r *http.Request) {
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

	// Recupera le prenotazioni dal database usando la funzione getBookings
	bookings, err := database.GetBookings(db, req.Username)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore durante la ricerca delle prenotazioni: %v", err), http.StatusInternalServerError)
		return
	}

	// Se non sono state trovate prenotazioni, restituisci comunque una risposta di successo con lista vuota
	if len(bookings) == 0 {
		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusOK)               // Risposta con successo
		err := json.NewEncoder(w).Encode(bookings) // Rispondi con la lista vuota di prenotazioni
		if err != nil {
			http.Error(w, "Errore durante la codifica della risposta JSON", http.StatusInternalServerError)
		}
		return
	}

	// Stampa delle prenotazioni trovate per il debug
	for i, booking := range bookings {
		fmt.Printf("Prenotazione #%d: %+v\n", i+1, booking)
	}

	// Rispondi con i dati delle prenotazioni in formato JSON
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusOK)
	err = json.NewEncoder(w).Encode(bookings)
	if err != nil {
		http.Error(w, "Errore durante la codifica della risposta JSON", http.StatusInternalServerError)
	}
}

func addReviewHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.ReviewRequest

	err := json.NewDecoder(r.Body).Decode(&req)
	if err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	fmt.Printf("Dati ricevuti: %+v\n", req)

	// Aggiungi la recensione
	err = database.AddReview(db, req.RoomID, req.Username, req.Comment, req.Rating)
	if err != nil {
		http.Error(w, "Errore interno del server", http.StatusInternalServerError)
		return
	}

	// Aggiunta della recensione riuscita
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(`{"status": "success", "message": "Recensione aggiunta con successo!"}`))

}

func getReviewsHandler(w http.ResponseWriter, r *http.Request) {
	// Verifica che la richiesta sia di tipo POST
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	// Decodifica il corpo della richiesta in una struttura ReviewRequest
	var req types.ReviewRequest
	err := json.NewDecoder(r.Body).Decode(&req)
	if err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	fmt.Printf("Dati ricevuti: %+v\n", req)

	// Chiama la funzione GetReview per ottenere la recensione dalla base di dati
	review, err := database.GetReview(db, req.RoomID, req.Username)
	if err != nil {
		http.Error(w, "Errore interno del server", http.StatusInternalServerError)
		return
	}

	// Se non viene trovata la recensione
	if review == nil {
		// Restituisci una risposta JSON con la recensione non trovata
		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusOK) // Status 200 OK anche se non sono state trovate recensioni
		w.Write([]byte(`{"status": "success", "message": "Nessuna recensione trovata"}`))
		return
	}

	// Se la recensione è trovata, restituisci i dettagli della recensione
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusOK)
	err = json.NewEncoder(w).Encode(review) // Restituisce la recensione
	if err != nil {
		http.Error(w, "Errore nella codifica della risposta JSON", http.StatusInternalServerError)
	}
}

func deleteBookingHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.DeleteBookingRequest

	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	// Esegui la query di cancellazione
	err := database.DeleteBooking(db, req.BookingID, req.Username)

	if err != nil {
		http.Error(w, "Errore interno del server", http.StatusInternalServerError)
		return
	}

	// Risposta JSON per la cancellazione della prenotazione

	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(`{"status": "success", "message": "Prenotazione eliminata con successo"}`))
}

// Handler per eliminare una recensione
func deleteReviewHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.DeleteReviewRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	// Esegui la query di cancellazione
	err := database.DeleteReview(db, req.RoomID, req.Username)

	if err != nil {
		http.Error(w, "Errore interno del server", http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(`{"status": "success", "message": "Recensione eliminata con successo"}`))
}

func getMeteGettonateHandler(w http.ResponseWriter, r *http.Request) {

	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	// Recupera le mete dal database
	mete, err := database.GetMeteFromDB(db)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore nel recupero delle mete: %v", err), http.StatusInternalServerError)
		return
	}

	// Calcola il punteggio per ciascuna meta
	meteConPunteggio := utils.CalcolaPunteggi(mete)

	w.Header().Set("Content-Type", "application/json")
	if err := json.NewEncoder(w).Encode(meteConPunteggio); err != nil {
		http.Error(w, "Errore durante la codifica della risposta JSON", http.StatusInternalServerError)
	}
}

func getOfferteImperdibiliHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	offerte, err := database.GetOfferteImperdibili(db)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore nel recupero delle offerte: %v", err), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	if err := json.NewEncoder(w).Encode(offerte); err != nil {
		http.Error(w, "Errore durante la codifica della risposta JSON", http.StatusInternalServerError)
	}
}

func addRoomHotelHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.RoomHotelRequest

	err := json.NewDecoder(r.Body).Decode(&req)
	if err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	fmt.Printf("Dati ricevuti: %+v\n", req)

	//devo passare l'host
	// Aggiungi la stanza all'hotel
	err = database.AddRoomHotel(db, req.HotelName, req.Location, req.Description, req.Services, req.HostHotel, req.HotelImagePath, req.RoomName, req.RoomDescription, req.PricePerNight, req.MaxGuests, req.RoomType, req.RoomImagePath)
	if err != nil {
		http.Error(w, "Errore interno del server", http.StatusInternalServerError)
		return
	}

	// Aggiunta della stanza riuscita
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(`{"status": "success", "message": "Stanza aggiunta con successo!"}`))
}

func addRoomHadler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.RoomRequest

	err := json.NewDecoder(r.Body).Decode(&req)
	if err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	fmt.Printf("Dati ricevuti: %+v\n", req)

	// Aggiungi la stanza all'hotel
	err = database.AddRoom(db, req.HotelName, req.RoomName, req.RoomDescription, req.PricePerNight, req.MaxGuests, req.RoomType, req.RoomImagePath)
	if err != nil {
		http.Error(w, "Errore interno del server", http.StatusInternalServerError)
		return
	}

	// Aggiunta della stanza riuscita
	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(`{"status": "success", "message": "Stanza aggiunta con successo!"}`))
}

func searchHotelsHandler(w http.ResponseWriter, r *http.Request) {
	var req types.SearchRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Bad request", http.StatusBadRequest)
		return
	}

	//da implementare l'ordinamento se richiesto
	var orderBy string
	if req.OrderBy != nil {
		orderBy = *req.OrderBy
		fmt.Println("Ordinamento richiesto:", orderBy)
	} else {
		orderBy = "default"
		fmt.Println("Ordinamento di default")
	}

	fmt.Printf("Dati ricevuti: %+v\n", req)

	results, err := database.SearchHotels(db, req.City, req.CheckInDate, req.CheckOutDate, req.Guests, req.Services)

	fmt.Printf("Risultati della ricerca: %+v\n", results)

	if err != nil {
		http.Error(w, "Errore interno del server", http.StatusInternalServerError)
		return
	}

	// Ordina esternamente i risultati in base al criterio selezionato
	results = utils.OrderResults(results, orderBy)

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(results)
}

func getRoomsHandler(w http.ResponseWriter, r *http.Request) {
	var req types.GetRoomsReviewsRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	rooms, err := database.GetRooms(db, req.HotelID)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore in GetRooms: %v", err), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(rooms)
}

// Handler per ottenere le recensioni per l’hotel
func getHotelReviewsHandler(w http.ResponseWriter, r *http.Request) {
	var req types.GetRoomsReviewsRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	fmt.Printf("Dati ricevuti: %+v\n", req)

	reviews, err := database.GetHotelReviews(db, req.HotelID)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore in GetHotelReviews: %v", err), http.StatusInternalServerError)
		return
	}

	fmt.Printf("Recensioni dell'hotel #%d: %+v\n", req.HotelID, reviews)

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(reviews)
}

func getAvailableRoomsHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.AvailableRoomsRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	// Chiama la funzione del package database per ottenere le stanze disponibili
	rooms, err := database.GetAvailableRooms(db, req.HotelID, req.CheckOutDate, req.CheckInDate)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore nel recupero delle stanze disponibili: %v", err), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	if err := json.NewEncoder(w).Encode(rooms); err != nil {
		http.Error(w, "Errore durante la codifica della risposta JSON", http.StatusInternalServerError)
	}
}

func addBookingHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.BookingRoomRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	// Inserisce la prenotazione nel database
	if err := database.AddBooking(db, req.Username, req.RoomID, req.CheckInDate, req.CheckOutDate, req.TotalAmount, req.Status); err != nil {
		http.Error(w, fmt.Sprintf("Errore durante l'inserimento della prenotazione: %v", err), http.StatusInternalServerError)
		return
	}

	// Recupera l'email dell'host
	hostEmail, err := database.GetHostEmailByRoomID(db, req.RoomID)
	if err != nil {
		http.Error(w, "Errore nel recupero dell'email dell'host", http.StatusInternalServerError)
		return
	}

	// Recupera i dettagli della stanza (nome della stanza e dell'hotel)
	roomName, hotelName, err := database.GetRoomDetails(db, req.RoomID)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore nel recupero dei dettagli della stanza: %v", err), http.StatusInternalServerError)
		return
	}

	// Invia email di notifica all'host con i dettagli della prenotazione
	subject := "Nuova prenotazione ricevuta"
	body := fmt.Sprintf("Hai ricevuto una nuova prenotazione da %s per la stanza '%s' dell'hotel '%s'.", req.Username, roomName, hotelName)
	if err := utils.SendEmail(hostEmail, subject, body); err != nil {
		log.Printf("Errore durante l'invio dell'email: %v", err)
	}

	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(`{"status": "success", "message": "Prenotazione effettuata con successo!"}`))
}

func updateUserPointsHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.UpdatePointsRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	err := database.UpdateUserPoints(db, req.Username, req.PointsToAdd)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore nell'aggiornamento punti: %v", err), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(`{"status": "success", "message": "Punti aggiornati correttamente"}`))
}

func SetInterestHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}
	fmt.Printf("inseisco interessi")

	var req types.SetInterestRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}
	fmt.Printf("Richiesta ricevuta per gli interessi: %+v\n", req)

	// Inserisci l'interesse nel database (senza PriceInitial)
	interestID, err := database.InsertInterest(db, req.Username, req.RoomID, req.MonitorValue)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore durante l'inserimento dell'interesse: %v", err), http.StatusInternalServerError)
		return
	}

	// Prepara la risposta
	response := map[string]interface{}{
		"status":     "ok",
		"message":    "Interesse impostato con successo",
		"InterestID": interestID,
	}
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(response)
}

func deleteRoomHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.DeleteRoomRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	// Chiama la funzione per eliminare la stanza
	err := database.DeleteRoom(db, req.RoomID, req.Username)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore durante l'eliminazione della stanza: %v", err), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(`{"status": "success", "message": "Stanza eliminata con successo"}`))
}

func updateHotelDescriptionHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.UpdateHotelDescriptionRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	// Aggiorna la descrizione dell'hotel (verificando che il proprietario sia corretto)
	err := database.UpdateHotelDescription(db, req.HotelID, req.NewDescription, req.Username)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore durante l'aggiornamento della descrizione: %v", err), http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	w.Write([]byte(`{"status": "success", "message": "Descrizione aggiornata con successo"}`))
}

func getCostDataHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Metodo non supportato", http.StatusMethodNotAllowed)
		return
	}

	var req types.UserRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Errore nel parsing JSON", http.StatusBadRequest)
		return
	}

	// Recupera i dati di costo dal database usando il package database
	costs, err := database.GetCostData(db, req.Username)
	if err != nil {
		http.Error(w, fmt.Sprintf("Errore nel recupero dei dati di costo: %v", err), http.StatusInternalServerError)
		return
	}

	// Imposta l'header e restituisce i dati in formato JSON
	w.Header().Set("Content-Type", "application/json")
	if err := json.NewEncoder(w).Encode(costs); err != nil {
		http.Error(w, "Errore nella codifica della risposta JSON", http.StatusInternalServerError)
	}
}

func main() {
	db = initializeDatabase()            // Inizializza la connessione al database
	defer db.Close()                     // Chiude la connessione al termine del server
	utils.InitializeEmail()              // Inizializza il sistema di email
	utils.StartNotificationScheduler(db) // Avvia il sistema di notifiche
	utils.StartPeriodicRatingUpdate(db, 10*time.Minute)

	http.HandleFunc("/login", LogInHandler)
	http.HandleFunc("/signup", SignUPHandler)
	http.HandleFunc("/getUserData", getUserDataHandler)
	http.HandleFunc("/getHotelsHost", getHotelsHostHandler)
	http.HandleFunc("/getBookings", getBookingsHandler)
	http.HandleFunc("/addReview", addReviewHandler)
	http.HandleFunc("/getReviews", getReviewsHandler)
	http.HandleFunc("/deleteBooking", deleteBookingHandler)
	http.HandleFunc("/deleteReview", deleteReviewHandler)
	http.HandleFunc("/getMeteGettonate", getMeteGettonateHandler)
	http.HandleFunc("/getOfferteImperdibili", getOfferteImperdibiliHandler)
	http.HandleFunc("/addRoomHotel", addRoomHotelHandler)
	http.HandleFunc("/addRoom", addRoomHadler)
	http.HandleFunc("/searchHotels", searchHotelsHandler)
	http.HandleFunc("/getRooms", getRoomsHandler)
	http.HandleFunc("/getHotelReviews", getHotelReviewsHandler)
	http.HandleFunc("/getAvailableRooms", getAvailableRoomsHandler)
	http.HandleFunc("/addBooking", addBookingHandler)
	http.HandleFunc("/updateUserPoints", updateUserPointsHandler)
	http.HandleFunc("/setInterest", SetInterestHandler)
	http.HandleFunc("/deleteRoom", deleteRoomHandler)
	http.HandleFunc("/updateHotelDescription", updateHotelDescriptionHandler)
	http.HandleFunc("/getCostData", getCostDataHandler)

	port := "8080"
	fmt.Printf("Server in ascolto su http://localhost:%s\n", port)

	if err := http.ListenAndServe(":"+port, nil); err != nil {
		log.Fatalf("Errore durante l'avvio del server: %v", err)
	}
}
