#!/usr/bin/env bash
# Installs (or updates) the servicestudio-mcp-oml skill into the local
# Claude Code skills directory. Works on macOS, Linux, and Git Bash on
# Windows.
#
# Usage:
#   bash scripts/install.sh                  # clone latest from GitHub
#   bash scripts/install.sh --source <path>   # copy from a local checkout instead
set -euo pipefail

REPO_URL="https://github.com/OutSystems/outsystems11-mcp.git"
SKILL_NAME="servicestudio-mcp-oml"
SKILLS_DIR="$HOME/.claude/skills"
SOURCE_DIR=""

while [ $# -gt 0 ]; do
  case "$1" in
    --source)
      SOURCE_DIR="$2"
      shift 2
      ;;
    *)
      echo "Usage: $0 [--source <path-to-local-checkout>]" >&2
      exit 1
      ;;
  esac
done

TMP_DIR=""
# An `if`, not `[ … ] && …`: the trap's status becomes the script's exit code,
# so a false test here would turn a successful --source install into exit 1.
cleanup() { if [ -n "$TMP_DIR" ]; then rm -rf "$TMP_DIR"; fi; }
trap cleanup EXIT

if [ -n "$SOURCE_DIR" ]; then
  REPO_DIR="$SOURCE_DIR"
else
  command -v git >/dev/null 2>&1 || { echo "Error: git is required but not found on PATH." >&2; exit 1; }
  TMP_DIR="$(mktemp -d)"
  echo "Cloning $REPO_URL..."
  git clone --depth 1 "$REPO_URL" "$TMP_DIR/repo"
  REPO_DIR="$TMP_DIR/repo"
fi

if [ ! -d "$REPO_DIR/$SKILL_NAME" ]; then
  echo "Error: '$SKILL_NAME' not found under $REPO_DIR" >&2
  exit 1
fi

TARGET="$SKILLS_DIR/$SKILL_NAME"
mkdir -p "$SKILLS_DIR"
rm -rf "$TARGET"
cp -R "$REPO_DIR/$SKILL_NAME" "$TARGET"

echo "Installed '$SKILL_NAME' to $TARGET"
echo "Start a new Claude Code session for the skill to load."
