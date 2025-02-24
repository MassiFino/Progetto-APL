# BookRoom

## Descrizione
BookRoom è una piattaforma per la prenotazione di stanze in hotel, pensata per semplificare l'esperienza sia per gli utenti che per i gestori di strutture. Gli utenti possono:
- Cercare hotel tramite filtri avanzati (location, date, numero di ospiti, servizi, ecc.)
- Prenotare stanze e visualizzare lo storico delle prenotazioni nel proprio profilo
- Accedere a grafici interattivi per monitorare le spese
- Lasciare recensioni e ricevere notifiche su offerte speciali

Per i gestori, BookRoom offre:
- Creazione e gestione delle schede degli hotel
- Monitoraggio delle prenotazioni e aggiornamento in tempo reale di disponibilità e tariffe
- Sistema di notifiche via e-mail per rimanere aggiornati

Inoltre, entrambi i gruppi possono beneficiare di un sistema di accumulo punti, che incentiva l'utilizzo della piattaforma offrendo vantaggi esclusivi.

## Caratteristiche
- **Ricerca avanzata:** Filtri per location, date, numero di ospiti e servizi.
- **Prenotazioni:** Possibilità di prenotare stanze in tempo reale.
- **Gestione profilo:** Visualizzazione dello storico prenotazioni e delle recensioni.
- **Notifiche:** Avvisi su offerte speciali e aggiornamenti via e-mail.
- **Sistema punti:** Accumulo e utilizzo di punti per ottenere sconti e vantaggi esclusivi.
- **Gestione strutture:** Interfaccia dedicata per i gestori per creare e aggiornare schede hotel.

## Requisiti
Per utilizzare il progetto, assicurati di avere installato:
- **Docker**: Per creare e gestire i container dei microservizi.
- **Docker Compose**: Per orchestrare i container.
- **Python**: Per eseguire i microservizi basati su FastAPI.
- **Go**: Per i microservizi sviluppati in Go.
- **FastAPI**: Per costruire l'API.
- **MySQL**: Per il database.

## Installazione e Avvio
1. **Clonare il repository:**
   ```bash
   git clone https://github.com/MassiFino/Progetto-APL
2. **Avvio dei servizi**
   docker-compose up -d
