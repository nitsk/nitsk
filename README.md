# Graphify + Ollama — 100% local, private stack

Docker Compose setup for running [Graphify](https://github.com/Graphify-Labs/graphify)
(a knowledge-graph generator for codebases) entirely on your own machine, using
[Ollama](https://ollama.com) as the LLM backend. No cloud API keys are used —
source code is parsed locally via tree-sitter (no LLM at all), and the optional
semantic pass over docs/PDFs/images is served by your local Ollama instance.

## Prerequisites

- Docker and Docker Compose

## Setup

1. Copy the env file and pick a model:

   ```bash
   cp .env.example .env
   ```

2. Start Ollama and pull the model:

   ```bash
   docker compose up -d ollama
   docker compose exec ollama ollama pull qwen2.5-coder:7b
   ```

3. Build the graphify image:

   ```bash
   docker compose build graphify
   ```

## Usage

Drop the codebase you want to analyze into `./target` (it's bind-mounted into
the container), then run:

```bash
# Extract a knowledge graph (docs/PDFs/images use the local Ollama model;
# source code parsing is fully offline and never calls an LLM)
docker compose run --rm graphify extract target --backend ollama --out .

# Query the resulting graph
docker compose run --rm graphify query "what connects auth to database?"

# Serve the graph over MCP for team/editor access
docker compose run --rm --service-ports --entrypoint python graphify \
  -m graphify.serve graphify-out/graph.json --transport http --host 0.0.0.0
```

Output (`graph.json`, HTML graph, markdown report) is written to the
`graphify-out` volume under `/workspace/graphify-out` inside the container.

## Privacy

- `OLLAMA_BASE_URL` points at the `ollama` container — nothing leaves your
  machine.
- No `ANTHROPIC_API_KEY`, `OPENAI_API_KEY`, or other cloud provider keys are
  configured or required.
