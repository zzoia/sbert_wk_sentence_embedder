FROM python:3.7-slim-buster

EXPOSE 5000

ENV PYTHONDONTWRITEBYTECODE=1
ENV PYTHONUNBUFFERED=1
ENV PYTHONIOENCODING=UTF-8

COPY ./requirements.txt /app/requirements.txt
WORKDIR /app

RUN pip install -r requirements.txt -f https://download.pytorch.org/whl/torch_stable.html
COPY . /app

ENTRYPOINT [ "flask"]
CMD [ "run", "--host", "0.0.0.0" ]

# RUN useradd appuser && chown -R appuser /app
# USER appuser

# CMD ["gunicorn", "-w", "4", "--graceful-timeout", "60", "--bind", "0.0.0.0:5000", "--timeout", "60", "--log-level", "debug", "webapp:app"]