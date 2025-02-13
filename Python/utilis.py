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


def assign_points_for_booking(
    total_amount: float, 
    room_type: str,
    guests: int,
    nights: int
) -> int:
    """
    calcolo punti per una prenotazione:

    - Punti base fissi: 5
    - +1 punto ogni 50€ (floor division)
    - Soglie spesa: se >500 => +5, se >1000 => +10 (cumulabili)
    - Tipologia stanza via match/case
    - Bonus guests (>=2 e >4) in un'unica riga
    - Bonus nights (1 punto a notte) + soglie 7 e 14
    """

    # (1) Punti base
    base_points = 5

    # (2) +1 punto ogni 50 euro
    extra_points_amount = int(total_amount // 50)

    # (3) Soglie spesa => lista di tuple (threshold, bonus)
    #    useremo una comprensione per sommare i bonus se superiamo la soglia
    thresholds_spending = [
        (1000, 10),
        (500, 5),
    ]
    # Somma i bonus per tutte le soglie che superi
    extra_big_spender = sum(bonus for threshold, bonus in thresholds_spending if total_amount > threshold)

    room_type_bonus_map = {
        "Suite": 5,
        "Deluxe": 3,
        "Doppia": 2,
        "Tripla": 2,
        "Singola": 1,
    }
    extra_points_room_type = room_type_bonus_map.get(room_type, 0)

    # (5) Bonus guests: +2 se guests >=2, +3 aggiuntivi se guests>4 (cumulati)
    extra_points_guests = (2 if guests >= 2 else 0) + (3 if guests > 4 else 0)

    # (6) Bonus nights
    #     +1 punto per ogni notte
    extra_points_nights = nights

    #    Soglie per le notti => similmente a thresholds_spending
    thresholds_nights = [
        (14, 6),
        (7, 3),
    ]
    extra_night_bonus = sum(b for limit, b in thresholds_nights if nights >= limit)

    # (7) Somma di tutti i contributi
    total_points = (
        base_points
        + extra_points_amount
        + extra_big_spender
        + extra_points_room_type
        + extra_points_guests
        + extra_points_nights
        + extra_night_bonus
    )

    return total_points

def assign_points_for_review(review_text: str) -> int:
    """
    Calcolo punti per recensione 

    - 10 punti base
    - Bonus in base alla lunghezza del testo, usando soglie e comprensione.
      Esempio:
         >200 caratteri => +4
         >100 caratteri => +2
         (cumulabili se vuoi, o in ordine decrescente)
    """

    base_points = 10
    text_len = len(review_text.strip())

    # Lista di tuple (soglia, bonus). 
    # ATTENZIONE: se vuoi un "ordine" (prima 200, poi 100) 
    # e NON cumulare i bonus, potresti dover gestire diversamente
    thresholds_text_length = [
        (200, 4),
        (100, 2),
    ]

    # Esempio: sommi i bonus di tutte le soglie che superi => cumulabile
    # Se invece vuoi che superare 200 dia SOLO +4, devi cambiare logica.
    bonus_for_length = sum(bonus for limit, bonus in thresholds_text_length if text_len > limit)

    points = base_points + bonus_for_length
    return points

