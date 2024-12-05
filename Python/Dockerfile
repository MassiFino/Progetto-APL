# Dockerfile per il server
FROM python:3.9

WORKDIR /app

COPY ./requirements.txt /app/
RUN pip install --no-cache-dir -r requirements.txt

COPY . /app

# Espone la porta 8000
EXPOSE 8000

# Comando per avviare l'applicazione
CMD ["uvicorn", "server:app", "--host", "0.0.0.0", "--port", "8000", "--log-level", "debug"]

