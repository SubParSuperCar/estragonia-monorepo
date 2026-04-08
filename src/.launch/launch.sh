#!/usr/bin/env sh

echo "Runtime context: POSIX Shell"

GD_CANDIDATES="godot godot-mono"

for name in $GD_CANDIDATES; do
  path="$(command -v "$name" 2>/dev/null)"
  if [ -n "$path" ]; then
    echo "Godot found via PATH ($name): $path"
    exec "$path" "$@"
  fi
done

SCRIPT_DIR="$(cd -- "$(dirname -- "$0")" && pwd)"
GD_PATH="$SCRIPT_DIR/bin/godot"

if [ -x "$GD_PATH" ]; then
  echo "Godot found via local bin: $GD_PATH"
  exec "$GD_PATH" "$@"
fi

echo "Godot not found." >&2
exit 1
