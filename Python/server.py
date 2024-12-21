import os
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import requests
from user_owner import SignIn, SignUp
from utilis import connect_go
from typing import Optional

app = FastAPI()

class LoginRequest(BaseModel):
    Username: str
    Password: str


class SignupRequest(BaseModel):
    Username: str
    Email: str
    Password: str
    PImage: Optional[str] = None  # Campo opzionale



@app.post("/login")
def login(request: LoginRequest):
    payload = {"Username": request.Username, "Password": request.Password}
    print("Payload inviato:", payload)

    # Validazione tramite SigIn
    success, messaggio = SignIn(payload)
    if not success:
        raise HTTPException(status_code=400, detail=messaggio)

    try:
        response = connect_go("login", payload)
        return response
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))

@app.post("/signup")
def signup(request: SignupRequest):
    payload = {"Username": request.Username, "Email": request.Email, "Password": request.Password, "PImage": request.PImage}
    print("Payload inviato:", payload)

    # Validazione tramite SigIn
    success, messaggio = SignUp(payload)
    if not success:
        raise HTTPException(status_code=400, detail=messaggio)

    try:
        #print("Payload inviato:", request.dict())
        response = connect_go("signup", request.dict())
        return response
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))