#!/bin/sh
echo -ne '\033c\033]0;Expedition\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/Expedition_0.3.x86_64" "$@"
