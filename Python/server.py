import os
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import requests

app = FastAPI()

class LoginRequest(BaseModel):
    Username: str
    Password: str

# Indirizzo del server Go
go_server_url = "http://go-server:8080/Signup"

@app.post("/login")
def login(request: LoginRequest):
    payload = {"Username": request.Username, "Password": request.Password}
    print("Tentativo di connessione al server Go:", go_server_url)
    print("Payload inviato:", payload)

    try:
        response = requests.post(go_server_url, json=payload)
        print("Risposta ricevuta dal server Go:", response.status_code, response.text)
        response.raise_for_status()  # Verifica che lo status code sia 2xx
        return {"status": "success", "go_response": response.json()}
    except requests.RequestException as e:
        print(f"Errore nella comunicazione con il server Go: {e}")
        raise HTTPException(status_code=502, detail=f"Errore comunicazione con server Go: {str(e)}")
