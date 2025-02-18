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
	// Assumendo che la tabella users abbia colonna points (INT DEFAULT 0)
	query := `SELECT Username, Email, ProfileImage, Role, points FROM users WHERE Username = ?`

	err := db.QueryRow(query, username).Scan(
		&user.Username,
		&user.Email,
		&user.PImage,
		&user.Role,
		&user.Points,
	)
	if err != nil {
		if err == sql.ErrNoRows {
			return nil, nil
		}
		return nil, fmt.Errorf("errore QueryRow: %w", err)
	}

	return &user, nil
}

func GetHotelsHost(db *sql.DB, username string) ([]types.HotelResponse, error) {
	fmt.Printf("Cerco hotel per l'utente: %s\n", username)

	query := "SELECT HotelID,Name, Location, Description, Services, Rating, Images FROM hotels WHERE UserHost = ?"
	rows, err := db.Query(query, username)
	if err != nil {
		return nil, fmt.Errorf("errore durante l'esecuzione della query: %v", err)
	}
	defer rows.Close()

	var hotels []types.HotelResponse

	for rows.Next() {
		var hotel types.HotelResponse
		var services string
		var rating sql.NullFloat64

		err := rows.Scan(&hotel.HotelID, &hotel.Name, &hotel.Location, &hotel.Description, &services, &rating, &hotel.Images)
		if err != nil {
			return nil, fmt.Errorf("errore durante la scansione delle righe: %v", err)
		}

		if !rating.Valid {
			rating.Float64 = 0.0 // Se NULL, assegna 0.0 come valore di default
		}
		hotel.Rating = rating.Float64
		var serviceList []string
		for _, s := range strings.Split(services, ",") {
			serviceList = append(serviceList, strings.TrimSpace(s))
		}
		hotel.Services = serviceList

		hotels = append(hotels, hotel)
	}

	if err := rows.Err(); err != nil {
		return nil, fmt.Errorf("errore durante l'iterazione delle righe: %v", err)
	}

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
            h.HotelID,
            h.Name,
            h.Location,
            h.Description,
            h.Services,
            COALESCE(h.Rating, 0) AS Rating,
            h.Images,
            COUNT(DISTINCT h.HotelID) AS NumeroHotel,
            AVG(r.PricePerNight) AS PrezzoMedio,
            COUNT(b.BookingID) AS NumeroPrenotazioni
        FROM hotels h
        JOIN rooms r ON h.HotelID = r.HotelID
        LEFT JOIN bookings b ON r.RoomID = b.RoomID
        LEFT JOIN reviews rv ON r.RoomID = rv.RoomID
        GROUP BY 
            h.HotelID, h.Name, h.Location, h.Description, h.Services, h.Rating, h.Images
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

		var servicesStr string
		// L'ordine degli Scan deve corrispondere all'ordine dei campi selezionati
		if err := rows.Scan(&m.HotelID, &m.Name, &m.Location, &m.Description, &servicesStr, &m.MediaVoto, &m.Images, &m.NumeroHotel, &m.PrezzoMedio, &m.NumeroPrenotazioni); err != nil {
			return nil, err
		}

		// Trasforma la stringa dei servizi in un array di stringhe

		m.Services = strings.Split(servicesStr, ",")
		for i, s := range m.Services {
			m.Services[i] = strings.TrimSpace(s)
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
            h.Name,
            h.Description,
            h.Services,
			h.Rating,
            h.Location,
            h.Images,
            MIN(r.PricePerNight) AS PrezzoMinimo
        FROM hotels h
        JOIN rooms r ON h.HotelID = r.HotelID
        LEFT JOIN reviews rv ON r.RoomID = rv.RoomID
        WHERE r.IsAvailable = TRUE
        GROUP BY 
            h.HotelID, h.Name, h.Description, h.Services, h.Rating, h.Location, h.Images
        HAVING COALESCE(AVG(h.Rating), 0) >= 3.0
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

		var servicesStr string

		if err := rows.Scan(&off.HotelID, &off.Name, &off.Description, &servicesStr, &off.MediaVoto, &off.Location, &off.Images, &off.PrezzoMinimo); err != nil {
			return nil, err
		}

		off.Services = strings.Split(servicesStr, ",")
		for i, s := range off.Services {
			off.Services[i] = strings.TrimSpace(s)
		}
		offerte = append(offerte, off)
	}
	if err := rows.Err(); err != nil {
		return nil, err
	}
	return offerte, nil
}

func AddRoomHotel(db *sql.DB, name, location, description string, services []string, userHost, hotelImage, roomName, roomDescription string, prezzoPerNotte float64, maxGuest int, roomType, roomImages string) error {

	servicesStr := strings.Join(services, ",")

	// Inserimento dell'hotel
	query := "INSERT INTO hotels (Name, Location, Description, Services, UserHost, Images, Rating) VALUES (?, ?, ?, ?, ?, ?, 0)"

	_, err := db.Exec(query, name, location, description, servicesStr, userHost, hotelImage)

	if err != nil {
		log.Printf("Errore durante l'inserimento dell'hotel: %v", err)
		return fmt.Errorf("errore durante l'inserimento dell'hotel: %w", err)
	}

	// Query per ottenere l'ID dell'hotel appena inserito
	query = "SELECT HotelID FROM hotels WHERE Name = ? AND Location = ? AND UserHost = ?"
	var hotelID int
	err = db.QueryRow(query, name, location, userHost).Scan(&hotelID)
	if err != nil {
		log.Printf("Errore durante la ricerca dell'ID dell'hotel: %v", err)
		return fmt.Errorf("errore durante la ricerca dell'ID dell'hotel: %w", err)
	}

	// Inserimento della stanza
	query = "INSERT INTO rooms (HotelID, Name, Description, PricePerNight, MaxGuests, TipologiaStanza, Images) VALUES (?, ?, ?, ?, ?, ?, ?)"
	_, err = db.Exec(query, hotelID, roomName, roomDescription, prezzoPerNotte, maxGuest, roomType, roomImages)
	if err != nil {
		log.Printf("Errore durante l'inserimento della stanza: %v", err)
		return fmt.Errorf("errore durante l'inserimento della stanza: %w", err)
	}

	return nil
}

func AddRoom(db *sql.DB, hotelName, roomName, roomDescription string, pricePerNight float64, maxGuests int, roomType, roomImage string) error {

	// Query per ottenere l'ID dell'hotel
	query := "SELECT HotelID FROM hotels WHERE Name = ?"
	var hotelID int
	err := db.QueryRow(query, hotelName).Scan(&hotelID)
	if err != nil {
		log.Printf("Errore durante la ricerca dell'ID dell'hotel: %v", err)
		return fmt.Errorf("errore durante la ricerca dell'ID dell'hotel: %w", err)
	}

	// Inserimento della stanza
	query = "INSERT INTO rooms (HotelID, Name, Description, PricePerNight, MaxGuests, TipologiaStanza, Images) VALUES (?, ?, ?, ?, ?, ?, ?)"
	_, err = db.Exec(query, hotelID, roomName, roomDescription, pricePerNight, maxGuests, roomType, roomImage)
	if err != nil {
		log.Printf("Errore durante l'inserimento della stanza: %v", err)
		return fmt.Errorf("errore durante l'inserimento della stanza: %w", err)
	}

	return nil
}

func SearchHotels(db *sql.DB, location, checkIn, checkOut string, guest int, services []string) ([]types.SearchResponse, error) {
	// Costruzione della query SQL di base.
	// La query:
	// - seleziona gli hotel e le relative informazioni
	// - unisce la tabella hotels con rooms (almeno una stanza deve rispettare i criteri)
	// - filtra per location (usando LIKE)
	// - verifica che la stanza abbia capacità >= guest e sia marcata come disponibile
	// - controlla che non esistano booking che si sovrappongano all'intervallo [checkIn, checkOut]
	query := `
        SELECT DISTINCT 
            h.HotelID, 
            h.Name, 
            h.Location, 
            h.Description, 
            h.Services, 
            h.Rating, 
            h.Images,
			MIN(r.PricePerNight) AS minPrice
        FROM hotels h
        JOIN rooms r ON h.HotelID = r.HotelID
        WHERE h.Location LIKE ?
          AND r.MaxGuests >= ?
          AND r.IsAvailable = 1
          AND NOT EXISTS (
              SELECT 1 FROM bookings b
              WHERE b.RoomID = r.RoomID
                AND (b.CheckInDate < ? AND b.CheckOutDate > ?)
          )
		GROUP BY h.HotelID, h.Name, h.Location, h.Description, h.Services, h.Rating, h.Images
    `

	// Prepara i parametri:
	// - Per la location usiamo il pattern LIKE (ad es. "%Milano%")
	// - Per la disponibilità delle date, usiamo checkOut e checkIn (nell'ordine corretto per la condizione)
	params := []interface{}{
		"%" + location + "%",
		guest,
		checkOut, // Se un booking inizia prima della data di check-out...
		checkIn,  // ... e finisce dopo la data di check-in, allora si sovrappone.
	}

	// Aggiungiamo dinamicamente le condizioni per i servizi richiesti.
	// Poiché in hotels il campo Services è una stringa (ad esempio: "Free Wi-Fi,Free Parking,Gym,Spa,Restaurant"),
	// per ogni servizio richiesto aggiungiamo una condizione che verifichi che la stringa contenga quel servizio.
	for _, service := range services {
		query += " AND h.Services LIKE ?"
		params = append(params, "%"+service+"%")
	}

	// Eseguiamo la query
	rows, err := db.Query(query, params...)
	if err != nil {
		return nil, fmt.Errorf("errore durante l'esecuzione della query: %v", err)
	}
	defer rows.Close()

	// Prepariamo l'array di risposta
	var hotels []types.SearchResponse
	for rows.Next() {
		var hotelID int
		var name, loc, desc, servicesStr, images string
		var rating float64
		var price float64

		if err := rows.Scan(&hotelID, &name, &loc, &desc, &servicesStr, &rating, &images, &price); err != nil {
			return nil, fmt.Errorf("errore durante la scansione delle righe: %v", err)
		}

		// Convertiamo la stringa dei servizi in una slice, eliminando eventuali spazi in eccesso.
		serviceList := strings.Split(servicesStr, ",")
		for i, s := range serviceList {
			serviceList[i] = strings.TrimSpace(s)
		}

		hotel := types.SearchResponse{
			Name:        name,
			Location:    loc,
			Description: desc,
			Services:    serviceList,
			Rating:      rating,
			Images:      images,
			Price:       price,
		}
		hotels = append(hotels, hotel)
	}
	if err = rows.Err(); err != nil {
		return nil, fmt.Errorf("errore durante l'iterazione delle righe: %v", err)
	}
	return hotels, nil
}

func UpdateAllHotelsRating(db *sql.DB) error {
	query := `
        UPDATE hotels h 
        SET Rating = (
            SELECT AVG(r.rating)
            FROM reviews r
            JOIN rooms ro ON r.roomID = ro.RoomID
            WHERE ro.HotelID = h.HotelID
        )
    `
	_, err := db.Exec(query)
	if err != nil {
		return fmt.Errorf("errore nell'aggiornamento dei rating: %w", err)
	}
	return nil
}

func GetRooms(db *sql.DB, hotelID int) ([]types.Room, error) {
	query := `
        SELECT RoomID, Name, Description, PricePerNight, MaxGuests, TipologiaStanza, Images
        FROM rooms
        WHERE HotelID = ?
    `
	rows, err := db.Query(query, hotelID)
	if err != nil {
		return nil, fmt.Errorf("errore durante l'esecuzione della query GetRooms: %v", err)
	}
	defer rows.Close()

	var rooms []types.Room
	for rows.Next() {
		var room types.Room
		if err := rows.Scan(&room.RoomID, &room.RoomName, &room.RoomDescription, &room.PricePerNight, &room.MaxGuests, &room.RoomType, &room.Images); err != nil {
			return nil, fmt.Errorf("errore durante la scansione in GetRooms: %v", err)
		}
		rooms = append(rooms, room)
	}
	if err := rows.Err(); err != nil {
		return nil, fmt.Errorf("errore durante l'iterazione in GetRooms: %v", err)
	}
	return rooms, nil
}

func GetHotelReviews(db *sql.DB, hotelID int) ([]types.Review, error) {
	query := `
	SELECT 
		r.RoomID, 
		r.Name AS RoomName, 
		rv.Username, 
		rv.review, 
		rv.rating
	FROM rooms r
	LEFT JOIN reviews rv ON r.RoomID = rv.roomID
	WHERE r.HotelID = ?
	`

	rows, err := db.Query(query, hotelID)
	if err != nil {
		return nil, fmt.Errorf("errore durante l'esecuzione della query GetHotelReviews: %v", err)
	}
	defer rows.Close()

	var reviews []types.Review

	for rows.Next() {
		var rev types.Review
		var username sql.NullString
		var comment sql.NullString
		var rating sql.NullFloat64

		if err := rows.Scan(&rev.RoomID, &rev.RoomName, &username, &comment, &rating); err != nil {
			return nil, fmt.Errorf("errore durante la scansione in GetHotelReviews: %v", err)
		}
		// Assegna i valori convertendo i valori NULL:
		rev.Username = ""
		if username.Valid {
			rev.Username = username.String
		}
		rev.Comment = ""
		if comment.Valid {
			rev.Comment = comment.String
		}
		if rating.Valid {
			rev.Rating = rating.Float64
		} else {
			rev.Rating = 0
		}
		// Aggiungi la recensione solo se almeno uno dei campi è valorizzato
		if username.Valid || comment.Valid || rating.Valid {
			reviews = append(reviews, rev)
		}
	}

	if err := rows.Err(); err != nil {
		return nil, fmt.Errorf("errore durancte l'iterazione delle righe in GetHotelReviews: %v", err)
	}

	return reviews, nil
}

func GetAvailableRooms(db *sql.DB, hotelID int, checkOutDate, checkInDate string) ([]types.Room, error) {
	query := `
        SELECT RoomID, Name, Description, PricePerNight, MaxGuests, TipologiaStanza, Images
		FROM rooms
		WHERE HotelID = ?
		AND IsAvailable = TRUE
		AND NOT EXISTS (
    		SELECT 1 FROM bookings
    		WHERE bookings.RoomID = rooms.RoomID
      		AND (DATE(bookings.CheckInDate) < ? AND DATE(bookings.CheckOutDate) > ?)
	);
    `

	rows, err := db.Query(query, hotelID, checkOutDate, checkInDate)
	if err != nil {
		return nil, fmt.Errorf("errore durante l'esecuzione della query: %w", err)
	}
	defer rows.Close()

	var rooms []types.Room
	for rows.Next() {
		var room types.Room
		// Assicurati che i campi della struct types.Room corrispondano all'ordine dei campi selezionati
		if err := rows.Scan(&room.RoomID, &room.RoomName, &room.RoomDescription, &room.PricePerNight, &room.MaxGuests, &room.RoomType, &room.Images); err != nil {
			return nil, fmt.Errorf("errore durante la scansione dei dati: %w", err)
		}
		rooms = append(rooms, room)
	}
	if err := rows.Err(); err != nil {
		return nil, err
	}
	return rooms, nil
}

func AddBooking(db *sql.DB, username string, roomID int, checkInDate, checkOutDate string, totalAmount float64, status string) error {
	query := `
		INSERT INTO bookings (Username, RoomID, CheckInDate, CheckOutDate, TotalAmount, Status, CreatedAt)
		VALUES (?, ?, ?, ?, ?, ?, NOW())
	`
	_, err := db.Exec(query, username, roomID, checkInDate, checkOutDate, totalAmount, status)
	if err != nil {
		return fmt.Errorf("errore durante l'inserimento della prenotazione: %w", err)
	}
	return nil
}

func UpdateUserPoints(db *sql.DB, username string, pointsToAdd int) error {
	query := `
        UPDATE users
        SET points = points + ?
        WHERE Username = ?
    `
	_, err := db.Exec(query, pointsToAdd, username)
	if err != nil {
		return fmt.Errorf("errore update punti: %w", err)
	}
	return nil
}

func InsertInterest(db *sql.DB, username string, roomID int, monitorValue float64) (int, error) {
	query := `
        INSERT INTO interests (Username, RoomID, MonitorValue, DateAdded)
        VALUES (?, ?, ?, NOW())
    `
	result, err := db.Exec(query, username, roomID, monitorValue)
	if err != nil {
		return 0, fmt.Errorf("errore durante l'inserimento dell'interesse: %w", err)
	}
	interestID, err := result.LastInsertId()
	if err != nil {
		return 0, fmt.Errorf("errore nel recupero dell'ID: %w", err)
	}
	return int(interestID), nil
}
func DeleteRoom(db *sql.DB, roomID int, username string) error {
	// La query elimina la stanza solo se esiste un hotel associato a quella stanza
	// il cui host (UserHost) corrisponde al parametro 'username'
	query := `
		DELETE r 
		FROM rooms r
		JOIN hotels h ON r.HotelID = h.HotelID
		WHERE r.RoomID = ? AND h.UserHost = ?`

	_, err := db.Exec(query, roomID, username)
	if err != nil {
		return fmt.Errorf("errore durante l'eliminazione della stanza: %w", err)
	}
	return nil
}

func UpdateHotelDescription(db *sql.DB, hotelID int, newDescription, username string) error {

	query := "UPDATE hotels SET Description = ? WHERE HotelID = ? AND UserHost = ?"
	res, err := db.Exec(query, newDescription, hotelID, username)
	if err != nil {
		return fmt.Errorf("errore durante l'aggiornamento della descrizione: %w", err)
	}
	rowsAffected, err := res.RowsAffected()
	if err != nil {
		return fmt.Errorf("errore nel recupero delle righe aggiornate: %w", err)
	}
	if rowsAffected == 0 {
		return fmt.Errorf("nessun hotel aggiornato: controlla se il parametro Username è corretto")
	}
	return nil
}

func GetCostData(db *sql.DB, username string) ([]types.CostData, error) {
	query := `
        SELECT DATE_FORMAT(CheckInDate, '%M') AS Month, SUM(TotalAmount) AS Cost
        FROM bookings
        WHERE Username = ?
        GROUP BY Month
    `
	rows, err := db.Query(query, username)
	if err != nil {
		return nil, fmt.Errorf("errore durante l'esecuzione della query GetCostData: %w", err)
	}
	defer rows.Close()

	var costs []types.CostData
	for rows.Next() {
		var cd types.CostData
		if err := rows.Scan(&cd.Month, &cd.Cost); err != nil {
			return nil, fmt.Errorf("errore durante la scansione dei dati: %w", err)
		}
		costs = append(costs, cd)
	}
	if err := rows.Err(); err != nil {
		return nil, fmt.Errorf("errore iterando le righe: %w", err)
	}
	return costs, nil
}

// ----Notifiche
func GetInterestsToNotify(db *sql.DB) ([]types.SetInterestResponse, error) {
	query := `
        SELECT i.InterestID, i.Username, r.Name as RoomName, h.Name as HotelName, i.MonitorValue
        FROM interests i
        JOIN rooms r ON i.RoomID = r.RoomID
        JOIN hotels h ON r.HotelID = h.HotelID
        WHERE r.PricePerNight < i.MonitorValue
    `
	rows, err := db.Query(query)
	if err != nil {
		return nil, fmt.Errorf("errore durante il recupero degli interessi: %w", err)
	}
	defer rows.Close()

	var responses []types.SetInterestResponse
	for rows.Next() {
		var resp types.SetInterestResponse
		if err := rows.Scan(&resp.InterestID, &resp.Username, &resp.RoomName, &resp.HotelName, &resp.MonitorValue); err != nil {
			return nil, fmt.Errorf("errore durante la scansione degli interessi: %w", err)
		}
		responses = append(responses, resp)
		fmt.Printf("Recuperato interesse: ID=%d, Username=%s, RoomName=%s, HotelName=%s, MonitorValue=%.2f\n", resp.InterestID, resp.Username, resp.RoomName, resp.HotelName, resp.MonitorValue)
	}

	if err := rows.Err(); err != nil {
		return nil, fmt.Errorf("errore durante l'iterazione degli interessi: %w", err)
	}

	fmt.Printf("Totale interessi recuperati: %d\n", len(responses))
	return responses, nil
}

func GetUserEmailByUsername(db *sql.DB, username string) (string, error) {
	var email string
	query := "SELECT email FROM users WHERE username = ?"
	err := db.QueryRow(query, username).Scan(&email)
	if err != nil {
		return "", fmt.Errorf("errore durante il recupero dell'email per l'utente %s: %v", username, err)
	}
	return email, nil
}
func GetUpcomingCheckInBookings(db *sql.DB) ([]types.UpcomingBooking, error) {
	query := `
        SELECT b.BookingID, b.Username, b.RoomID, r.Name, b.CheckInDate, u.Email
        FROM bookings b
        JOIN users u ON b.Username = u.Username
        JOIN rooms r ON b.RoomID = r.RoomID
        WHERE b.CheckInDate BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 7 DAY)
    `
	rows, err := db.Query(query)
	if err != nil {
		return nil, fmt.Errorf("errore durante il recupero delle prenotazioni in arrivo: %w", err)
	}
	defer rows.Close()

	var bookings []types.UpcomingBooking
	for rows.Next() {
		var booking types.UpcomingBooking
		if err := rows.Scan(&booking.BookingID, &booking.Username, &booking.RoomID, &booking.RoomName, &booking.CheckInDate, &booking.Email); err != nil {
			return nil, fmt.Errorf("errore durante la scansione delle righe: %w", err)
		}
		bookings = append(bookings, booking)

		// Simuliamo l'invio della notifica di check-in
		fmt.Printf("Notifica: Prenotazione #%d per l'utente %s (stanza %s) il %s\n",
			booking.BookingID, booking.Username, booking.RoomName, booking.CheckInDate)
	}

	if err := rows.Err(); err != nil {
		return nil, fmt.Errorf("errore durante l'iterazione delle righe: %w", err)
	}

	return bookings, nil
}
func GetRoomDetails(db *sql.DB, roomID int) (string, string, error) {
	query := `
        SELECT r.Name AS RoomName, h.Name AS HotelName
        FROM rooms r
        JOIN hotels h ON r.HotelID = h.HotelID
        WHERE r.RoomID = ?
    `
	var roomName, hotelName string
	err := db.QueryRow(query, roomID).Scan(&roomName, &hotelName)
	if err != nil {
		return "", "", fmt.Errorf("errore nel recupero dei dettagli per RoomID %d: %w", roomID, err)
	}
	return roomName, hotelName, nil
}

func GetHostEmailByRoomID(db *sql.DB, roomID int) (string, error) {
	var email string
	query := `
        SELECT u.Email
        FROM users u
        JOIN hotels h ON u.Username = h.UserHost
        JOIN rooms r ON h.HotelID = r.HotelID
        WHERE r.RoomID = ?
    `
	err := db.QueryRow(query, roomID).Scan(&email)
	if err != nil {
		return "", fmt.Errorf("errore durante il recupero dell'email dell'host: %w", err)
	}
	return email, nil
}

func GetAveragePriceForRoomTypeAndLocation(db *sql.DB, roomType, location string) (float64, error) {
	query := `
        SELECT AVG(r.PricePerNight)
        FROM rooms r
        JOIN hotels h ON r.HotelID = h.HotelID
        WHERE r.TipologiaStanza = ? AND h.Location = ?`

	var avgPrice sql.NullFloat64

	err := db.QueryRow(query, roomType, location).Scan(&avgPrice)
	if err != nil {
		return 0, err
	}
	if !avgPrice.Valid {
		return 0, nil
	}
	return avgPrice.Float64, nil
}
