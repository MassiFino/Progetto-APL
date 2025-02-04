package database

import (
	"database/sql"
	"fmt"
	"log"
	"progetto-go/types"
	"strings"

	_ "github.com/go-sql-driver/mysql"
)

func ConnectDB(username, password, host, port, dbName string) (*sql.DB, error) {
	// Inizializzazione della gestione del database per il driver go-sql-driver/mysql
	dataSourceName := fmt.Sprintf("%s:%s@tcp(%s:%s)/%s", username, password, host, port, dbName)
	db, err := sql.Open("mysql", dataSourceName)
	if err != nil {
		log.Println(err)
		return nil, err
	}

	// Poiché open non ci dice se la connessione con il db è avvenuta effettivamente, testiamo la connessione con il metodo Ping() della struct db:
	err = db.Ping()
	if err != nil {
		log.Println(err)
		return nil, err
	}

	fmt.Println("Connected to MySQL!")

	return db, nil
}

func CheckUserCredentials(db *sql.DB, username, password string) (bool, error, string) {
	var storedPassword, Role string

	// Query per verificare le credenziali
	query := "SELECT Password, role FROM users WHERE Username = ?"
	err := db.QueryRow(query, username).Scan(&storedPassword, &Role)
	if err != nil {
		if err == sql.ErrNoRows {
			// Nessun utente trovato
			return false, nil, ""
		}
		log.Println("Errore nella query:", err)
		return false, err, ""
	}

	// Controllo della password (non crittografata)
	if storedPassword == password {
		return true, nil, Role
	}

	return false, nil, ""
}

func CheckUserExists(db *sql.DB, username, email string) (bool, error) {
	query := "SELECT Username FROM users WHERE Username = ? OR Email = ?"
	var existingUsername string
	err := db.QueryRow(query, username, email).Scan(&existingUsername)
	if err != nil {
		if err == sql.ErrNoRows {
			// Nessun utente trovato
			return false, nil
		}
		log.Println("Errore nella query:", err)
		return false, err
	}

	return true, nil
}

func RegisterUser(db *sql.DB, username, password, email, role string, pImage *string) error {
	var query string
	var err error

	if pImage == nil {
		// Query senza il campo ProfileImage
		query = "INSERT INTO users (Username, Password, Email, Role) VALUES (?, ?, ?, ?)"
		_, err = db.Exec(query, username, password, email, role)
	} else {
		// Query con il campo ProfileImage
		query = "INSERT INTO users (Username, Password, Email, ProfileImage, Role) VALUES (?, ?, ?, ?, ?)"
		_, err = db.Exec(query, username, password, email, *pImage, role)
	}

	if err != nil {
		log.Printf("Errore durante l'inserimento dell'utente: %v", err)
		return fmt.Errorf("errore durante la registrazione dell'utente: %w", err)
	}

	return nil
}

// Funzione che recupera i dati dell'utente dal database in base all'email
func GetUser(db *sql.DB, username string) (*types.UserResponse, error) {
	var user types.UserResponse

	query := "SELECT Username, Email, ProfileImage,role FROM users WHERE Username = ?"
	row := db.QueryRow(query, username)

	err := row.Scan(&user.Username, &user.Email, &user.PImage, &user.Role)
	if err != nil {
		if err == sql.ErrNoRows {
			// Utente non trovato
			return nil, nil
		}
		return nil, fmt.Errorf("errore durante la ricerca dell'utente: %v", err)
	}
	// Log dei dati dell'utente recuperati
	fmt.Println("Dati dell'utente recuperati:", user)
	return &user, nil
}

func GetHotelsHost(db *sql.DB, username string) ([]types.HotelResponse, error) {
	fmt.Printf("Cerco hotel per l'utente: %s\n", username)

	query := "SELECT Name, Location, Description, Services, Rating, Images FROM hotels WHERE UserHost = ?"
	rows, err := db.Query(query, username)
	if err != nil {
		return nil, fmt.Errorf("errore durante l'esecuzione della query: %v", err)
	}
	defer rows.Close()

	fmt.Println("Esecuzione query per l'utente:", username)

	var hotels []types.HotelResponse

	// Itera su ogni riga restituita dalla query
	for rows.Next() {
		fmt.Println("Iterazione su una nuova riga dell'hotel")

		var hotel types.HotelResponse
		var services string
		var rating float64 // tipo float64 per il Rating

		// Scansione dei dati dalla query
		err := rows.Scan(&hotel.Name, &hotel.Location, &hotel.Description, &services, &rating, &hotel.Images)
		if err != nil {
			return nil, fmt.Errorf("errore durante la scansione delle righe: %v", err)
		}

		// Assegna il valore di rating direttamente alla proprietà dell'hotel
		hotel.Rating = rating

		fmt.Printf("Services prima della divisione: %s\n", services)
		hotel.Services = strings.Split(services, ",")
		fmt.Printf("Hotel dopo scansione: %+v\n", hotel)

		// Aggiungi l'hotel alla lista
		hotels = append(hotels, hotel)
	}

	// Gestione degli errori delle righe
	if err := rows.Err(); err != nil {
		fmt.Printf("Errore durante l'iterazione delle righe: %v\n", err)
		return nil, fmt.Errorf("errore durante l'iterazione delle righe: %v", err)
	}

	fmt.Printf("Gli hotel trovati sono: %d\n", len(hotels))
	return hotels, nil
}
func GetBookings(db *sql.DB, username string) ([]types.BookingResponse, error) {
	fmt.Printf("Cerco prenotazioni per l'utente: %s\n", username)

	// Query per ottenere i dettagli della prenotazione, incluso il percorso dell'immagine
	query := `
    SELECT 
        b.BookingID,
        b.Username,
		b.RoomID,
        b.CheckInDate,
        b.CheckOutDate,
        b.TotalAmount,
        b.Status,
        r.Name AS RoomName,
        r.Images AS RoomImage,  -- Nome della stanza e immagine
        h.Name AS HotelName,  -- Nome dell'hotel
        h.Location AS HotelLocation  -- Posizione dell'hotel
    FROM bookings b
    JOIN rooms r ON b.RoomID = r.RoomID  -- Collega la prenotazione alla stanza
    JOIN hotels h ON r.HotelID = h.HotelID  -- Collega la stanza all'hotel
    WHERE b.Username = ?;  -- Filtro per l'utente
    `

	// Esegui la query
	rows, err := db.Query(query, username)
	if err != nil {
		return nil, fmt.Errorf("errore durante l'esecuzione della query: %v", err)
	}
	defer rows.Close()

	fmt.Println("Esecuzione query per l'utente:", username)

	var bookings []types.BookingResponse

	// Itera su ogni riga restituita dalla query
	for rows.Next() {
		fmt.Println("Iterazione su una nuova riga di prenotazione")

		var booking types.BookingResponse
		var roomName, roomImage, hotelName, hotelLocation string

		// Scansione dei dati dalla query
		err := rows.Scan(&booking.BookingID, &booking.Username, &booking.RoomID, &booking.CheckInDate, &booking.CheckOutDate,
			&booking.TotalAmount, &booking.Status, &roomName, &roomImage, &hotelName, &hotelLocation)
		if err != nil {
			return nil, fmt.Errorf("errore durante la scansione delle righe: %v", err)
		}

		// Assegna i dati
		booking.RoomName = roomName
		booking.HotelName = hotelName
		booking.HotelLocation = hotelLocation
		booking.RoomImage = roomImage // Assegna l'immagine della stanza

		// Aggiungi la prenotazione alla lista
		bookings = append(bookings, booking)
	}

	// Gestione degli errori delle righe
	if err := rows.Err(); err != nil {
		fmt.Printf("Errore durante l'iterazione delle righe: %v\n", err)
		return nil, fmt.Errorf("errore durante l'iterazione delle righe: %v", err)
	}

	fmt.Printf("Le prenotazioni trovate sono: %d\n", len(bookings))
	return bookings, nil
}

func AddReview(db *sql.DB, roomID int, username, review string, rating int) error {
	query := "INSERT INTO reviews (roomID, Username, review, rating) VALUES (?, ?, ?, ?)"
	_, err := db.Exec(query, roomID, username, review, rating)
	if err != nil {
		log.Printf("Errore durante l'inserimento della recensione: %v", err)
		return fmt.Errorf("errore durante l'inserimento della recensione: %w", err)
	}

	return nil
}

func GetReview(db *sql.DB, roomID int, username string) (*types.ReviewResp, error) {
	// Query per selezionare il commento, rating e il CreatedAt (timestamp) per la roomID e username specificati
	query := "SELECT review, rating, CreatedAt FROM reviews WHERE roomID = ? AND Username = ? LIMIT 1"
	fmt.Printf("Sto cercando le recensioni")

	// Esegui la query
	row := db.QueryRow(query, roomID, username)

	// Crea una struttura per memorizzare il commento, il rating e la data di creazione
	var review types.ReviewResp
	var createdAt string // Variabile temporanea per memorizzare la data come stringa

	// Scansiona il risultato nella struttura
	err := row.Scan(&review.Comment, &review.Rating, &createdAt)
	if err != nil {
		if err == sql.ErrNoRows {
			// Nessuna recensione trovata, restituisci una struttura vuota
			return &types.ReviewResp{}, nil
		}
		log.Printf("Errore durante la scansione della recensione: %v", err)
		return nil, fmt.Errorf("errore durante la scansione della recensione: %w", err)
	}

	// Assegna la data di creazione (CreatedAt) alla struttura ReviewResp
	review.CreatedAt = createdAt

	// Restituisci la recensione trovata con il commento, il rating e la data di creazione
	return &review, nil
}

func DeleteBooking(db *sql.DB, bookingID int, username string) error {
	query := "DELETE FROM bookings WHERE BookingID = ? AND Username = ?"
	_, err := db.Exec(query, bookingID, username)
	if err != nil {
		log.Printf("Errore durante l'eliminazione della prenotazione: %v", err)
		return fmt.Errorf("errore durante l'eliminazione della prenotazione: %w", err)
	}

	return nil
}

func DeleteReview(db *sql.DB, roomID int, username string) error {
	query := "DELETE FROM reviews WHERE roomID = ? AND Username = ?"
	_, err := db.Exec(query, roomID, username)
	if err != nil {
		log.Printf("Errore durante l'eliminazione della recensione: %v", err)
		return fmt.Errorf("errore durante l'eliminazione della recensione: %w", err)
	}

	return nil
}

func GetMeteFromDB(db *sql.DB) ([]types.ResponseMeta, error) {
	query := `
        SELECT 
            h.Location AS NomeMeta,
            COUNT(DISTINCT h.HotelID) AS NumeroHotel,
            AVG(r.PricePerNight) AS PrezzoMedio,
            COUNT(b.BookingID) AS NumeroPrenotazioni,
            AVG(rv.Rating) AS MediaVoto,
            MAX(h.Images) AS Immagine
        FROM hotels h
        JOIN rooms r ON h.HotelID = r.HotelID
        LEFT JOIN bookings b ON r.RoomID = b.RoomID
        LEFT JOIN reviews rv ON r.RoomID = rv.RoomID
        GROUP BY h.Location
        ORDER BY NumeroHotel DESC
        LIMIT 10;
    `
	rows, err := db.Query(query)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var mete []types.ResponseMeta
	for rows.Next() {
		var m types.ResponseMeta
		if err := rows.Scan(&m.NomeMeta, &m.NumeroHotel, &m.PrezzoMedio, &m.NumeroPrenotazioni, &m.MediaVoto, &m.Immagine); err != nil {
			return nil, err
		}
		mete = append(mete, m)
	}
	if err := rows.Err(); err != nil {
		return nil, err
	}
	return mete, nil
}

func GetOfferteImperdibili(db *sql.DB) ([]types.ResponseOffertaImperdibile, error) {
	query := `
        SELECT 
            h.HotelID,
            h.Name AS NomeHotel,
            h.Images AS Immagine,
            MIN(r.PricePerNight) AS PrezzoMinimo,
            AVG(rv.Rating) AS MediaVoto
        FROM hotels h
        JOIN rooms r ON h.HotelID = r.HotelID
        LEFT JOIN reviews rv ON r.RoomID = rv.RoomID
        WHERE r.IsAvailable = TRUE
        GROUP BY h.HotelID, h.Name, h.Images
        HAVING AVG(rv.Rating) >= 3.0  -- Filtra per ottenere solo offerte con un voto medio decente
        ORDER BY PrezzoMinimo ASC
        LIMIT 10;
    `
	rows, err := db.Query(query)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var offerte []types.ResponseOffertaImperdibile
	for rows.Next() {
		var off types.ResponseOffertaImperdibile
		if err := rows.Scan(&off.HotelID, &off.NomeHotel, &off.Immagine, &off.PrezzoMinimo, &off.MediaVoto); err != nil {
			return nil, err
		}
		offerte = append(offerte, off)
	}
	if err := rows.Err(); err != nil {
		return nil, err
	}
	return offerte, nil
}
