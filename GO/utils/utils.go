package utils

import (
	"database/sql"
	"fmt"
	"log"
	"progetto-go/database"
	"progetto-go/types"
	"sort"
	"time"

	"gopkg.in/gomail.v2"
)

// MetaConPunteggio include i dati della meta e il punteggio calcolato.
type MetaConPunteggio struct {
	types.ResponseMeta
	Punteggio float64 `json:"Punteggio"`
}

// Calcola il punteggio per ogni meta.

//	punteggio = (NumeroHotel * 1.0) + (NumeroPrenotazioni * 2.0) + (MediaVoto * 3.0) - (PrezzoMedio * 1.0)
//
// Massi
func CalcolaPunteggi(mete []types.ResponseMeta) []MetaConPunteggio {
	var risultati []MetaConPunteggio
	for _, m := range mete {
		// Se PrezzoMedio è 0 evitiamo errori, impostandolo a 1
		prezzo := m.PrezzoMedio
		if prezzo == 0 {
			prezzo = 1
		}

		punteggio := float64(m.NumeroPrenotazioni)*2.0 +
			m.MediaVoto*3.0 -
			(prezzo/100)*1.0

		risultati = append(risultati, MetaConPunteggio{
			ResponseMeta: m,
			Punteggio:    punteggio,
		})
	}
	return risultati
}

// Massi
func OrderResults(results []types.SearchResponse, orderBy string) []types.SearchResponse {

	switch orderBy {
	case "Prezzo: crescente":
		sort.Slice(results, func(i, j int) bool {
			return results[i].Price < results[j].Price
		})
	case "Prezzo: decrescente":
		sort.Slice(results, func(i, j int) bool {
			return results[i].Price > results[j].Price
		})
	case "Valutazione: migliore":
		sort.Slice(results, func(i, j int) bool {
			return results[i].Rating > results[j].Rating
		})
	default:
		sort.Slice(results, func(i, j int) bool {
			return results[i].Name < results[j].Name
		})
	}
	return results
}

// Dario
func StartPeriodicRatingUpdate(db *sql.DB, interval time.Duration) {
	ticker := time.NewTicker(interval)
	go func() {
		for {
			select {
			case <-ticker.C:
				// Ogni "interval" esegui l'aggiornamento
				err := database.UpdateAllHotelsRating(db)
				if err != nil {
					log.Printf("Errore durante l'aggiornamento dei rating: %v", err)
				} else {
					log.Println("Aggiornamento dei rating completato con successo")
				}
			}
		}
	}()
}

// ----------------------------------Funzioni di notifica----------------------------
// Dario
var emailDialer *gomail.Dialer

func InitializeEmail() {
	emailDialer = gomail.NewDialer("smtp.gmail.com", 587, "universita012@gmail.com", "hksk dmnn vgxq wewm")
}

func SendEmail(to, subject, body string) error {
	m := gomail.NewMessage()
	m.SetHeader("From", "universita012@gmail.com")
	m.SetHeader("To", to)
	m.SetHeader("Subject", subject)
	m.SetBody("text/html", body)

	return emailDialer.DialAndSend(m)
}

// Dario
func notifyInterests(db *sql.DB) {
	interests, err := database.GetInterestsToNotify(db)
	if err != nil {
		log.Printf("Errore durante il recupero degli interessi: %v", err)
		return
	}

	for _, interest := range interests {
		userEmail, err := database.GetUserEmailByUsername(db, interest.Username)
		if err != nil {
			log.Printf("Errore durante il recupero dell'email per l'utente %s: %v", interest.Username, err)
			continue
		}

		subject := "Notifica: Prezzo stanza in ribasso"
		body := fmt.Sprintf("Ciao %s, il prezzo della stanza %s dell'hotel %s è sceso al di sotto di %.2f. Approfitta dell'offerta!",
			interest.Username, interest.RoomName, interest.HotelName, interest.MonitorValue)
		if err := SendEmail(userEmail, subject, body); err != nil {
			log.Printf("Errore durante l'invio dell'email a %s: %v", userEmail, err)
			continue
		}
	}
}

// Dario
func notifyUpcomingCheckIn(db *sql.DB) {
	upcomingBookings, err := database.GetUpcomingCheckInBookings(db)
	if err != nil {
		log.Printf("Errore durante il recupero delle prenotazioni in arrivo: %v", err)
		return
	}

	for _, booking := range upcomingBookings {
		subject := "Promemoria: Check-in in arrivo"
		body := fmt.Sprintf("Ciao %s, il tuo check-in per la stanza %s è previsto per il %s.", booking.Username, booking.RoomName, booking.CheckInDate)

		// Usa direttamente l'email che hai già ottenuto
		if err := SendEmail(booking.Email, subject, body); err != nil {
			log.Printf("Errore durante l'invio dell'email: %v", err)
		}
	}
}

// Dario
func StartNotificationScheduler(db *sql.DB) {
	checkInTicker := time.NewTicker(24 * time.Hour)   // Notifica Check-in ogni 24 ore
	interestsTicker := time.NewTicker(12 * time.Hour) // Notifica interessi ogni 24 ore

	go func() {
		for {
			select {
			case <-checkInTicker.C:
				notifyUpcomingCheckIn(db)
			case <-interestsTicker.C:
				notifyInterests(db)
			}
		}
	}()
}

//----------------------------------Funzioni di notifica----------------------------
