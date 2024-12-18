
from utilis import verifica_email, verifica_password


def SignIn(acces):
    if not isinstance(acces, dict):
        raise ValueError("Il payload deve essere un dizionario.")

    if 'Username' not in acces or 'Password' not in acces:
        return False, "I campi username e password sono obbligatori."
    
    password = acces['Password']
    
    # Validazione della password
    success, messaggio = verifica_password(password)
    if not success:
        return False, messaggio
    
    return True, "Validazione riuscita."

def SignUp(acces):
    if not isinstance(acces, dict):
        raise ValueError("Il payload deve essere un dizionario.")

    if 'Username' not in acces or 'Email' not in acces or 'Password' not in acces:
        return False, "I campi username, email e password sono obbligatori."
    
    email = acces['Email']
    password = acces['Password']
    
    # Validazione dell'email
    success, messaggio = verifica_email(email)
    if not success:
        return False, messaggio
    
    # Validazione della password
    success, messaggio = verifica_password(password)
    if not success:
        return False, messaggio
    
    return True, "Validazione riuscita."




    