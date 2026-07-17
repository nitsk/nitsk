FROM python:3.12-slim

RUN pip install --no-cache-dir "graphifyy[ollama]"

WORKDIR /workspace

ENTRYPOINT ["graphify"]
CMD ["--help"]
