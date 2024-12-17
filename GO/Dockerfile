# Usa l'immagine ufficiale di Go
FROM golang:latest AS builder

# Imposta la directory di lavoro
WORKDIR /app

# Copia i file go.mod e go.sum per scaricare e cache delle dipendenze
COPY go.mod go.sum ./
RUN go mod tidy && go mod download

# Copia il resto del codice sorgente
COPY . .

# Compila il progetto Go in un binario chiamato "main"
RUN go build -o main .

# Immagine finale
FROM golang:latest

# Imposta la directory di lavoro
WORKDIR /app

# Copia il binario compilato dalla fase di build
COPY --from=builder /app/main .

# Esponi la porta 8080
EXPOSE 8080

# Comando per avviare il server
CMD ["./main"]
