package database

import (
	"database/sql"
	"fmt"
	"log"
	"progetto-go/types"

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

func CheckUserCredentials(db *sql.DB, username, password string) (bool, error) {
	var storedPassword string

	// Query per verificare le credenziali
	query := "SELECT Password FROM users WHERE Username = ?"
	err := db.QueryRow(query, username).Scan(&storedPassword)
	if err != nil {
		if err == sql.ErrNoRows {
			// Nessun utente trovato
			return false, nil
		}
		log.Println("Errore nella query:", err)
		return false, err
	}

	// Controllo della password (non crittografata)
	if storedPassword == password {
		return true, nil
	}

	return false, nil
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
