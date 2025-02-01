import os
from fastapi import FastAPI, HTTPException, Depends
from pydantic import BaseModel
import requests
from user_owner import SignIn, SignUp
from utilis import connect_go
from typing import Optional
from jwt_utils import create_jwt_token, decode_jwt_token
from fastapi.security import HTTPBearer, HTTPAuthorizationCredentials
import jwt

app = FastAPI()
security = HTTPBearer()
#current_username = None

class LoginRequest(BaseModel):
    Username: str
    Password: str


class SignupRequest(BaseModel):
    Username: str
    Email: str
    Password: str
    PImage: Optional[str] = None  # Campo opzionale
    Role: str

class addReviewRequest(BaseModel):
    Username: str
    RoomID: int
    Comment: str
    Rating: int
    
class getReviewsRequest(BaseModel):
    Username: str
    RoomID: int
   
@app.post("/login")
def login(request: LoginRequest):
    payload = {"Username": request.Username, "Password": request.Password}
    print("Payload inviato:", payload)
    #global current_username  # Dichiarazione esplicita della variabile globale
    # Validazione tramite SigIn
    success, messaggio = SignIn(payload)
    if not success:
        raise HTTPException(status_code=400, detail=messaggio)
    #current_username = request.Username

    print("Username dell'utente loggato:", request.Username)

    try:
        response = connect_go("login", payload)
        print(response)

        user_role = response.get("role")

        token_data = {
            "username": request.Username,
            "role": user_role
        }

        jwt_token = create_jwt_token(token_data, expires_minutes=60)

        return {
        "status": "ok",
        "message": "Login effettuato correttamente",
        "role": user_role,
        "token": jwt_token
        }
     
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))

@app.post("/signup")
def signup(request: SignupRequest):
    payload = {"Username": request.Username, "Email": request.Email, "Password": request.Password, "PImage": request.PImage, "Role": request.Role}
    print("Payload inviato:", payload)
    #global current_username  # Dichiarazione esplicita della variabile globale

    # Validazione tramite SigIn
    success, messaggio = SignUp(payload)
    if not success:
        raise HTTPException(status_code=400, detail=messaggio)
      # Aggiorna la variabile globale con l'email dell'utente
    #current_username = request.Username
    print("Username dell'utente loggato:", request.Username)

    try:
        #print("Payload inviato:", request.dict())
        response = connect_go("signup", request.dict())
        print(response)

        jwt_token = create_jwt_token(
        data={
            "username": request.Username,
            "role": request.Role
        },
        expires_minutes=60
        )

        return {
        "status": "ok",
        "message": "Registrazione effettuata",
        "role": request.Role,
        "token": jwt_token
        }

    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))
    
@app.post("/getUserData")
def get_user_data(credentials: HTTPAuthorizationCredentials = Depends(security)):
    try:
        # 1) Prendi il token dall'header (Bearer <token>)
        token = credentials.credentials

        # 2) Decodifica per ottenere i claims
        payload = decode_jwt_token(token)

        # 3) Recupera l'username dai claims
        username = payload.get("username")
        if not username:
            raise HTTPException(status_code=400, detail="Claim 'username' mancante")

        # 4) Ora chiami Go con quell'username
        response = connect_go("getUserData", {"Username": username})
        return response
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token scaduto")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Token non valido")
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))
    
@app.post("/getHotelsHost")
def get_user_data(credentials: HTTPAuthorizationCredentials = Depends(security)):
    try:
        token = credentials.credentials

        # 2) Decodifica per ottenere i claims
        payload = decode_jwt_token(token)

        # 3) Recupera l'username dai claims
        username = payload.get("username")
        if not username:
            raise HTTPException(status_code=400, detail="Claim 'username' mancante")
        
        response = connect_go("getHotelsHost", {"Username": username})
        print(response)
        return response
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token scaduto")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Token non valido")
    
@app.post("/getBookings")
def get_user_data(credentials: HTTPAuthorizationCredentials = Depends(security)):
    try:
        token = credentials.credentials

        # 2) Decodifica per ottenere i claims
        payload = decode_jwt_token(token)

        # 3) Recupera l'username dai claims
        username = payload.get("username")
        if not username:
            raise HTTPException(status_code=400, detail="Claim 'username' mancante")
        
        response = connect_go("getBookings", {"Username": username})
        print(response)
        return response
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token scaduto")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Token non valido")
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))
    
@app.post("/addReview")
def add_review(request: addReviewRequest):
    try:

        response = connect_go("addReview", request.dict())

        print(response)
        return response
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))
    
        
@app.post("/getReviews")
def add_review(request: getReviewsRequest):
    try:

        response = connect_go("getReviews", request.dict())

        print(response)
        return response
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))
    
    
    