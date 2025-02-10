package utils

import (
	"database/sql"
	"log"
	"progetto-go/database"
	"progetto-go/types"
	"sort"
	"time"
)

// MetaConPunteggio include i dati della meta e il punteggio calcolato.
type MetaConPunteggio struct {
	types.ResponseMeta
	Punteggio float64 `json:"Punteggio"`
}

// Calcola il punteggio per ogni meta.
//
//	punteggio = (NumeroHotel * 1.0) + (NumeroPrenotazioni * 2.0) + (MediaVoto * 3.0) - (PrezzoMedio * 1.0)
func CalcolaPunteggi(mete []types.ResponseMeta) []MetaConPunteggio {
	var risultati []MetaConPunteggio
	for _, m := range mete {
		// Se PrezzoMedio è 0 evitiamo errori, impostandolo a 1
		prezzo := m.PrezzoMedio
		if prezzo == 0 {
			prezzo = 1
		}

		punteggio := float64(m.NumeroHotel)*1.0 +
			float64(m.NumeroPrenotazioni)*2.0 +
			m.MediaVoto*3.0 -
			(prezzo/100)*1.0

		risultati = append(risultati, MetaConPunteggio{
			ResponseMeta: m,
			Punteggio:    punteggio,
		})
	}
	return risultati
}

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
