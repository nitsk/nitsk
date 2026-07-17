#!/usr/bin/env bash
# Installs Graphify (https://github.com/Graphify-Labs/graphify) configured to
# run entirely against a local Ollama server, so no repo/doc content is ever
# sent to a cloud LLM provider. Run this on the machine you'll actually use
# graphify on (it needs to reach ollama.com and pull a multi-GB model).
set -euo pipefail

MODEL="${OLLAMA_MODEL:-qwen2.5-coder:7b}"
ENV_FILE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)/graphify.env"

echo "==> Checking for Ollama"
if ! command -v ollama >/dev/null 2>&1; then
  echo "    not found, installing (https://ollama.com/install.sh)"
  curl -fsSL https://ollama.com/install.sh | sh
else
  echo "    found: $(command -v ollama)"
fi

echo "==> Checking Ollama server"
if ! curl -fsS -m 3 http://localhost:11434/ >/dev/null 2>&1; then
  echo "    not running, starting 'ollama serve' in the background"
  nohup ollama serve >/tmp/ollama-serve.log 2>&1 &
  for _ in $(seq 1 20); do
    curl -fsS -m 1 http://localhost:11434/ >/dev/null 2>&1 && break
    sleep 0.5
  done
fi

echo "==> Pulling model: ${MODEL}"
ollama pull "${MODEL}"

echo "==> Installing graphify (with the ollama/openai-compatible client extra)"
if command -v uv >/dev/null 2>&1; then
  uv tool install "graphifyy[ollama]" --force
elif command -v pipx >/dev/null 2>&1; then
  pipx install "graphifyy[ollama]" --force
else
  pip install --user "graphifyy[ollama]"
fi

echo "==> Writing ${ENV_FILE}"
sed "s/^export OLLAMA_MODEL=.*/export OLLAMA_MODEL=${MODEL}/" \
  "$(dirname "${BASH_SOURCE[0]}")/graphify.env.example" > "${ENV_FILE}"

echo "==> Registering the graphify skill for Claude Code"
# shellcheck disable=SC1090
source "${ENV_FILE}"
graphify claude install || echo "    (skipped: run 'graphify install' manually to target another platform)"

echo "==> Smoke test: local code extraction (no LLM call)"
graphify extract "$(pwd)" --code-only --out /tmp/graphify-setup-smoke-test >/dev/null
echo "    ok"

cat <<EOF

Done. To use graphify against your local model in a new shell:
  source ${ENV_FILE}
  graphify extract <path> --backend ollama

See graphify/README.md for the privacy notes on why --backend ollama (or
unsetting cloud API keys) is required for every run.
EOF
