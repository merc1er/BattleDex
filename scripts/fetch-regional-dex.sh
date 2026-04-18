#!/usr/bin/env bash
set -euo pipefail

# Fetches regional Pokédex membership from PokéAPI and writes it as JSON.
# Output: BattleDex.Core/Data/regional-dex.json
# Format: { "<gen>": [<national-dex-id>, ...], ... } — ordered by regional dex position.
#
# Each generation uses the "pure" regional dex (original game in that gen) to avoid
# later-gen species leaking in via remakes (e.g. ORAS/updated-hoenn includes Gen 4+).

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUT="$SCRIPT_DIR/../BattleDex.Core/Data/regional-dex.json"

# generation -> space-separated PokéAPI pokedex IDs to concatenate (first-seen wins)
declare -A GEN_TO_PDX=(
    [1]="2"            # Kanto (RB) — pure Gen 1.
    [2]="3"            # Original Johto (GS) — pure Gen 2. HGSS/updated-johto (7) leaks later gens.
    [3]="4"            # Hoenn (RSE) — pure Gen 3. ORAS/updated-hoenn (15) leaks later gens.
    [4]="6"            # Extended Sinnoh (Platinum)
    [5]="9"            # Updated Unova (B2W2)
    [6]="12 13 14"     # Kalos Central + Coastal + Mountain
    [7]="21"           # Updated Alola (USUM)
    [8]="27 28 29"     # Galar + Isle of Armor + Crown Tundra
    [9]="31 32 33"     # Paldea + Kitakami + Blueberry
)

fetch_ids() {
    local pdx_id="$1"
    curl -sf "https://pokeapi.co/api/v2/pokedex/${pdx_id}/" \
        | jq -r '.pokemon_entries | sort_by(.entry_number)[] | .pokemon_species.url' \
        | sed -E 's|.*/pokemon-species/([0-9]+)/?|\1|'
}

tmp="$(mktemp)"
trap 'rm -f "$tmp"' EXIT
echo "{" > "$tmp"

first=1
for gen in 1 2 3 4 5 6 7 8 9; do
    ordered=""
    declare -A seen=()
    for pdx in ${GEN_TO_PDX[$gen]}; do
        echo "  fetching gen $gen / pokedex $pdx..." >&2
        for id in $(fetch_ids "$pdx"); do
            if [ -z "${seen[$id]:-}" ]; then
                seen[$id]=1
                ordered="$ordered $id"
            fi
        done
    done
    csv=$(echo $ordered | tr ' ' ',')
    unset seen
    if [ $first -eq 1 ]; then first=0; else echo "," >> "$tmp"; fi
    printf '  "%d": [%s]' "$gen" "$csv" >> "$tmp"
done
echo "" >> "$tmp"
echo "}" >> "$tmp"

mv "$tmp" "$OUT"
trap - EXIT
echo "Wrote $OUT"
