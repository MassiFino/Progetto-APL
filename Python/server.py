import os
from fastapi import FastAPI, HTTPException, Depends
from pydantic import BaseModel
import requests
from user_owner import SignIn, SignUp
from utilis import connect_go, assign_points_for_booking, apply_discount_by_points_euro, assign_points_for_review, create_cost_chart
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


class DeleteBookingRequest(BaseModel):
    BookingID: int
    Username: str

class DeleteReviewRequest(BaseModel):
    Username: str
    RoomID: int


class AddRoomHotelRequest(BaseModel):
    HotelName: str
    Location: str
    Description: str
    Services: list[str]
    HotelImagePath: str
    RoomName: str
    RoomDescription: str
    PricePerNight: float
    MaxGuests: int
    RoomType: str
    RoomImagePath: str

class AddRoomRequest(BaseModel):
    HotelName: str
    RoomName: str
    RoomDescription: str
    PricePerNight: float
    MaxGuests: int
    RoomType: str
    RoomImagePath: str

class SearchRequest(BaseModel):
    City: str
    CheckInDate: str
    CheckOutDate: str
    Guests: int
    Services: list[str]
    OrderBy: Optional[str] = None  # Campo opzionale

class GetRoomsReviewsRequest(BaseModel):
    HotelID: int

class AvailableRoomsRequest(BaseModel):
    HotelID: int
    CheckInDate: str  
    CheckOutDate: str

class BookingRequest(BaseModel):
    RoomID: int
    CheckInDate: str 
    CheckOutDate: str 
    TotalAmount: float
    Status: str
    RoomType: str
    Guests: int
    Nights: int
    PointsSpent : int

class PreviewDiscountRequest(BaseModel):
    TotalAmount: float


class SetInterestRequest(BaseModel):
    RoomID: int
    MonitorValue: float
    
class DeleteRoomRequest(BaseModel):
    RoomID: int


class UpdateHotelDescriptionRequest(BaseModel):
    HotelID: int
    NewDescription: str

class CostChartResponse(BaseModel):
    status: str
    chart: str
    summary: dict

class AveragePriceRequest(BaseModel):
    RoomType: str
    Location: str

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
        # Invia la recensione al servizio Go
        response = connect_go("addReview", request.dict())
        
        # Se l'inserimento della recensione ha avuto successo, calcola i punti
        if response.get("status") == "success":
            review_text = request.Comment  # Assicurati che il campo sia "Comment"
            username = request.Username

            # Calcola i punti per la recensione (funzione definita in Utilis)
            points_review = assign_points_for_review(review_text)

            # Aggiorna i punti dell'utente nel sistema
            update_payload = {
                "Username": username,
                "PointsToAdd": points_review
            }
            update_response = connect_go("updateUserPoints", update_payload)

            print(f"[addReview] Assegnati {points_review} punti a {username}")
        
        print(response)
        return response

    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))
        
@app.post("/getReviews")
def get_review(request: getReviewsRequest):
    try:

        response = connect_go("getReviews", request.dict())

        print(response)
        return response
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))
    
    
@app.post("/deleteBooking")
def delete_booking(request: DeleteBookingRequest):
    try:
        
        # Prepara il payload da inviare al servizio (o direttamente usalo per eliminare)
        response = connect_go("deleteBooking", request.dict())
        return response
    except Exception as e:
        raise HTTPException(status_code=502, detail=str(e))

# Endpoint per eliminare una recensione
@app.post("/deleteReview")
def delete_review(request: DeleteReviewRequest):
    try:
        
        
        response = connect_go("deleteReview", request.dict())
        return response
    except Exception as e:
        raise HTTPException(status_code=502, detail=str(e))

@app.post("/getMeteGettonate")
def get_mete_gettonate():
    try:

        print("Chiamata a getMeteGettonate")
        response = connect_go("getMeteGettonate", {})

        mete_ordinate = sorted(response, key=lambda m: m["Punteggio"], reverse=True)

        print(mete_ordinate)

        return mete_ordinate
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))
    
@app.post("/getOfferteImperdibili")
def get_offerte_imperdibili():
    try:
        response = connect_go("getOfferteImperdibili", {})

        if response is None:
            print("risposta: None")
        else:
            print("risposta: " + str(response))


        return response
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))


@app.post("/addRoomHotel")
def add_room_hotel(
    request: AddRoomHotelRequest,
    credentials: HTTPAuthorizationCredentials = Depends(security)
):
    try:
        print("funzione addRoomHotel")
        # 1. Estrai il token dall'header
        token = credentials.credentials

        # 2. Decodifica il token per ottenere i claims
        payload_token = decode_jwt_token(token)
        username = payload_token.get("username")

        if not username:
            raise HTTPException(status_code=400, detail="Claim 'username' mancante nel token")

        print ("richiesta: " + str(request.dict()))
        # 3. Prepara il payload da inviare al servizio Go
        data = request.dict()
        # Aggiungi il campo UserHost (o come lo chiami tu nel servizio Go)
        data["HostHotel"] = username

        # Se il campo Services deve essere una stringa (es. separata da virgole), puoi fare:
        # data["Services"] = ",".join(data["Services"])
        print("Data da inviare a Go:", data)
        # 4. Invia la richiesta al servizio Go
        response = connect_go("addRoomHotel", data)
        return response

    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token scaduto")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Token non valido")
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))


@app.post("/addRoom")
def add_room(
    request: AddRoomRequest
):
    try:
    
        print ("richiesta: " + str(request.dict()))
        # 3. Prepara il payload da inviare al servizio Go

        response = connect_go("addRoom", request.dict())
        return response

    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))

@app.post("/searchHotels")
def search_hotels(request: SearchRequest):
    # Stampa i parametri per debug
    print("Parametri ricevuti:", request.dict())

    try:

        response = connect_go("searchHotels", request.dict())

        return response
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))
    

@app.post("/getRooms")
def get_rooms(request: GetRoomsReviewsRequest):
    try:
        # Inoltra la richiesta al backend Go
        response = connect_go("getRooms", request.dict())
        print(response)

        return response
    except Exception as e:
        raise HTTPException(status_code=502, detail=str(e))

@app.post("/getHotelReviews")
def get_hotel_reviews(request: GetRoomsReviewsRequest):
    try:
        response = connect_go("getHotelReviews", request.dict())

        print(response)
        return response
    except Exception as e:
        raise HTTPException(status_code=502, detail=str(e))

@app.post("/getAvailableRooms")
def get_available_rooms(request: AvailableRoomsRequest):
    try:

        print("Richiesta ricevuta:", request.dict())
        # Inoltra la richiesta al backend Go usando connect_go
        response = connect_go("getAvailableRooms", request.dict())

        print("Risposta ricevuta:", response)
        return response
    except ConnectionError as e:
        raise HTTPException(status_code=502, detail=str(e))


@app.post("/addBooking")
def add_booking(request: BookingRequest, credentials: HTTPAuthorizationCredentials = Depends(security)):
    token = credentials.credentials
    try:
        # Decodifica il token per estrarre lo username
        payload = decode_jwt_token(token)
        username = payload.get("username")
        if not username:
            raise HTTPException(status_code=400, detail="Username non trovato nei claims")
    
        # Aggiungi lo username al payload da inoltrare al servizio Go
        data = request.dict()
        data["Username"] = username
        # Inoltra la richiesta al servizio Go tramite connect_go
        response = connect_go("addBooking", data)

        if response.get("status") == "success":
            # 2) Calcola i punti
            #    Prendendo i dati dalla request
            total_amount = request.TotalAmount
            room_type = request.RoomType
            guests = request.Guests
            nights = request.Nights

            points_to_add = assign_points_for_booking(total_amount, room_type, guests, nights)

            update_payload = {"Username": username, "PointsToAdd": points_to_add}

            points_spent = getattr(request, "PointsSpent", 0)

            points_net = points_to_add - points_spent

            if points_net != 0:
                update_payload = {"Username": username, "PointsToAdd": points_net}
                connect_go("updateUserPoints", update_payload)
            
            print(f"[addBooking] Assegnati {points_to_add} punti a {username}")

        return response
    except Exception as e:
        raise HTTPException(status_code=502, detail=str(e))
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token scaduto")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Token non valido")


@app.post("/previewDiscount")
def preview_discount(
    request: PreviewDiscountRequest,
    credentials: HTTPAuthorizationCredentials = Depends(security)  # <--- Serve per ottenerlo
):
    # 1) Decodifica token
    token = credentials.credentials
    payload = decode_jwt_token(token)
    username = payload.get("username")
    if not username:
        raise HTTPException(status_code=400, detail="Username non presente nel token")

    # 2) Recupera i punti da Go
    user_data = connect_go("getUserData", {"Username": username})
    current_points = user_data.get("Points", 0)

    # 3) Calcolo sconto
    discounted_price, points_used = apply_discount_by_points_euro(
        request.TotalAmount,
        current_points
    )

    return {
        "status": "ok",
        "discounted_price": discounted_price,
        "points_used": points_used
    }

@app.post("/setInterest")
def set_interest(request: SetInterestRequest, credentials: HTTPAuthorizationCredentials = Depends(security)):
    token = credentials.credentials
    payload = decode_jwt_token(token)
    username = payload.get("username")
    if not username:
        raise HTTPException(status_code=400, detail="Username non trovato nei claims")
    
    # Inoltra la richiesta al servizio Go
    # Aggiungi lo username al payload se necessario
    data = request.dict()
    data["Username"] = username
    print(f"interessi data: {data}")

    try:
        response = connect_go("setInterest", data)
        print(f"Risposta ricevuta da Go: {response}")

        return response
    except Exception as e:
        raise HTTPException(status_code=502, detail=str(e))
    

@app.post("/deleteRoom")
def delete_room(request: DeleteRoomRequest, credentials: HTTPAuthorizationCredentials = Depends(security)):
    token = credentials.credentials
    if not token:
        raise HTTPException(status_code=401, detail="Token mancante")
    
    payload = decode_jwt_token(token)
    username = payload.get("username")

    data = request.dict()
    data["Username"] = username

    response = connect_go("deleteRoom", data)
    if response.get("status") == "success":
        return response
    else:
        raise HTTPException(status_code=400, detail=response.get("message", "Errore sconosciuto"))


@app.post("/updateHotelDescription")
def update_hotel_description(request: UpdateHotelDescriptionRequest, credentials: HTTPAuthorizationCredentials = Depends(security)):
    token = credentials.credentials
    if not token:
        raise HTTPException(status_code=401, detail="Token mancante")
    
    payload = decode_jwt_token(token)
    username = payload.get("username")

    data = request.dict()
    data["Username"] = username


    response = connect_go("updateHotelDescription", data)
    if response.get("status") == "success":
        return response
    else:
        raise HTTPException(status_code=400, detail=response.get("message", "Errore sconosciuto"))

@app.post("/getCostChart")
def get_cost_chart(credentials: HTTPAuthorizationCredentials = Depends(security)):
    token = credentials.credentials
    try:
        payload_token = decode_jwt_token(token)
    except jwt.ExpiredSignatureError:
        raise HTTPException(status_code=401, detail="Token scaduto")
    except jwt.InvalidTokenError:
        raise HTTPException(status_code=401, detail="Token non valido")
    
    username = payload_token.get("username")
    if not username:
        raise HTTPException(status_code=400, detail="Username non presente nel token")
    
    try:
        cost_data = connect_go("getCostData", {"Username": username})

        print("Dati di costo ricevuti da Go:", cost_data)
    except Exception as e:
        raise HTTPException(status_code=502, detail=str(e))
    
    # Crea il grafico con la funzione definita in utilis.py
    try:
        chart_base64 = create_cost_chart(cost_data)

        total_cost = sum(item["cost"] for item in cost_data)
        summary = {"total": total_cost, "count": len(cost_data)}

        print("Grafico generato (base64):", chart_base64[:50] + "...")
    except Exception as e:
        raise HTTPException(status_code=500, detail="Errore nella creazione del grafico: " + str(e))
    
    return CostChartResponse(status="ok", chart=chart_base64, summary=summary)

@app.post("/getAveragePrice")
def get_average_price(request: AveragePriceRequest,credentials: HTTPAuthorizationCredentials = Depends(security)):
    # Estrai il token e decodifica il payload
    token = credentials.credentials
    payload = decode_jwt_token(token)
    username = payload.get("username")
    if not username:
        raise HTTPException(status_code=400, detail="Username non trovato nei claims")
    
    data = request.dict()  
    
    try:
        response = connect_go("getAveragePrice", data)
        return response
    except Exception as e:
        raise HTTPException(status_code=502, detail=str(e))