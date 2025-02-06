package utils

import (
	"progetto-go/types"
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
