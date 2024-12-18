from email_validator import validate_email, EmailNotValidError
import re
import requests
import json


def verifica_email(email):
    try:
        # Validazione dell'email
        valid = validate_email(email)
        # Restituisce l'email normalizzata se valida
        return True, "L'email è valida."
    except EmailNotValidError as e:
        # Errore se l'email non è valida
        print(str(e))
        return False, "L'email non è valida."


def verifica_password(password):

    lunghezza_minima = 8
    almeno_una_maiuscola = r'[A-Z]'
    almeno_un_numero = r'[0-9]'
    almeno_un_carattere_speciale = r'[!@#$%^&*(),.?":{}|<>]'
    
    # Controlla la lunghezza
    if len(password) < lunghezza_minima:
        return False, "La password deve essere lunga almeno 8 caratteri."
    
    # Controlla almeno una lettera maiuscola
    if not re.search(almeno_una_maiuscola, password):
        return False, "La password deve contenere almeno una lettera maiuscola."
    
    # Controlla almeno un numero
    if not re.search(almeno_un_numero, password):
        return False, "La password deve contenere almeno un numero."
    
    # Controlla almeno un carattere speciale
    if not re.search(almeno_un_carattere_speciale, password):
        return False, "La password deve contenere almeno un carattere speciale."
    
    return True, "La password è valida."


def connect_go(endpoint, pl={}):
    if not isinstance(pl, dict):
        raise TypeError("Il payload deve essere un dizionario.")
    
    url = f"http://go:8080/{endpoint}"
    headers = {'Content-Type': 'application/json'}

    try:
        response = requests.post(url, json=pl, headers=headers)
        response.raise_for_status()  # Solleva eccezioni per errori HTTP
        return response.json()
    except requests.exceptions.RequestException as e:
        raise ConnectionError(f"Errore nella connessione al server Go: {e}")

