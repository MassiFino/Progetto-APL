import os
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import requests
from user_owner import SignIn, SignUp
from utilis import connect_go
from typing import Optional

app = FastAPI()
current_username = None

class LoginRequest(BaseModel):
    Username: str
    Password: str


class SignupRequest(BaseModel):
    Username: str
    Email: str
    Password: str
    PImage: Optional[str] = None  # Campo opzionale

class UserData(BaseModel):
    st: str

@app.post("/login")
def login(request: LoginRequest):
    payload = {"Username": request.Username, "Password": request.Password}
    print("Payload inviato:", payload)
    global current_username  # Dichiarazione esplicita della variabile globale
    # Validazione tramite SigIn
    success, messaggio = SignIn(payload)
    if not success:
        raise HTTPException(status_code=400, detail=messaggio)
    current_username = request.Username
    print("Username dell'utente loggato:", current_username)

    try:
        response = connect_go("login", payload)
        return response
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))

@app.post("/signup")
def signup(request: SignupRequest):
    payload = {"Username": request.Username, "Email": request.Email, "Password": request.Password, "PImage": request.PImage}
    print("Payload inviato:", payload)
    global current_username  # Dichiarazione esplicita della variabile globale

    # Validazione tramite SigIn
    success, messaggio = SignUp(payload)
    if not success:
        raise HTTPException(status_code=400, detail=messaggio)
      # Aggiorna la variabile globale con l'email dell'utente
    current_username = request.Username
    print("Username dell'utente loggato:", current_username)

    try:
        #print("Payload inviato:", request.dict())
        response = connect_go("signup", request.dict())
        return response
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))
    
@app.post("/getUserData")
def get_user_data():
    try:
        payload = {"Username": current_username}
        response = connect_go("getUserData", payload)
        print(response)
        return response
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))
