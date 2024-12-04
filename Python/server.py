from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

app = FastAPI()

class LoginRequest(BaseModel):
    username: str
    password: str

@app.post("/login")
def login(request: LoginRequest):
    # Logica di autenticazione
    if request.username == "admin" and request.password == "password":
        return {"status": "success", "message": "Login effettuato con successo"}
    else:
        raise HTTPException(status_code=401, detail="Credenziali non valide")