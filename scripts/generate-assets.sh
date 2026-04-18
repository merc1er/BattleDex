#!/usr/bin/env bash
set -euo pipefail

if [ $# -ne 1 ]; then
    echo "Usage: $0 <source-image>"
    echo "  Regenerates all packaging assets in BattleDex/Assets from a single source image."
    exit 1
fi

SRC="$1"
if [ ! -f "$SRC" ]; then
    echo "Error: source file not found: $SRC" >&2
    exit 1
fi

if command -v magick >/dev/null 2>&1; then
    IM=(magick)
elif command -v convert >/dev/null 2>&1; then
    IM=(convert)
else
    echo "Error: ImageMagick not found. Install 'imagemagick'." >&2
    exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ASSETS="$SCRIPT_DIR/../BattleDex/Assets"

if [ ! -d "$ASSETS" ]; then
    echo "Error: assets directory not found: $ASSETS" >&2
    exit 1
fi

# Square assets (direct resize)
square() {
    local size="$1" out="$2"
    "${IM[@]}" "$SRC" -strip -filter Lanczos -resize "${size}x${size}" "$ASSETS/$out"
}

# Padded assets (fit icon within inner square, transparent pad to target canvas)
padded() {
    local inner="$1" w="$2" h="$3" out="$4"
    "${IM[@]}" "$SRC" -strip -filter Lanczos -resize "${inner}x${inner}" \
        -background none -gravity center -extent "${w}x${h}" "$ASSETS/$out"
}

square 88  Square44x44Logo.scale-200.png
cp "$ASSETS/Square44x44Logo.scale-200.png" "$ASSETS/Square44x44Logo.png"
square 24  Square44x44Logo.targetsize-24_altform-unplated.png
square 300 Square150x150Logo.scale-200.png
cp "$ASSETS/Square150x150Logo.scale-200.png" "$ASSETS/Square150x150Logo.png"
square 50  StoreLogo.png
square 48  LockScreenLogo.scale-200.png

padded 300 620  300 Wide310x150Logo.scale-200.png
cp "$ASSETS/Wide310x150Logo.scale-200.png" "$ASSETS/Wide310x150Logo.png"
padded 600 1240 600 SplashScreen.scale-200.png
cp "$ASSETS/SplashScreen.scale-200.png" "$ASSETS/SplashScreen.png"

# Multi-resolution Windows icon
"${IM[@]}" "$SRC" -strip -filter Lanczos \
    \( -clone 0 -resize 256x256 \) \
    \( -clone 0 -resize 128x128 \) \
    \( -clone 0 -resize 64x64 \) \
    \( -clone 0 -resize 48x48 \) \
    \( -clone 0 -resize 32x32 \) \
    \( -clone 0 -resize 24x24 \) \
    \( -clone 0 -resize 16x16 \) \
    -delete 0 "$ASSETS/WindowIcon.ico"

echo "Regenerated assets in $ASSETS"
