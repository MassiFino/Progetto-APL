import jwt
import datetime

# Chiave segreta 
SECRET_KEY = "my_secret_key"

def create_jwt_token(data: dict, expires_minutes: int = 30) -> str:
    """
    Crea un token JWT con scadenza in 'expires_minutes' minuti.
    """
    expire_time = datetime.datetime.utcnow() + datetime.timedelta(minutes=expires_minutes)
    payload = data.copy()
    payload["exp"] = expire_time
    encoded_jwt = jwt.encode(payload, SECRET_KEY, algorithm="HS256")
    return encoded_jwt

def decode_jwt_token(token: str) -> dict:
    """
    Decodifica e verifica il token JWT, solleva eccezione se non valido o scaduto.
    """
    decoded = jwt.decode(token, SECRET_KEY, algorithms=["HS256"])
    return decoded
