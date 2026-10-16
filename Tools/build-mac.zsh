#!/bin/zsh
set -euo pipefail
game_root="${0:A:h:h}"
unity_editor='/Applications/Unity/Hub/Editor/6000.6.5f1/Unity.app/Contents/MacOS/Unity'
mkdir -p "$game_root/Work"
"$unity_editor" -batchmode -quit -projectPath "$game_root" -executeMethod FactoryDelivery.Prepare -logFile "$game_root/Work/build-mac.log"
built_app="$game_root/Builds/Mac/Midnight Shift.app"
local_app="${1:-$HOME/Applications/Midnight Shift.app}"
expected_id='com.Willow-Works.Midnight-Shift---The-Last-Lantern'
if [[ -d "$local_app" ]]; then
    existing_id=$(/usr/libexec/PlistBuddy -c 'Print CFBundleIdentifier' "$local_app/Contents/Info.plist")
    if [[ "$existing_id" != "$expected_id" ]]; then
        print -u2 'A different app already uses this location. Nothing was overwritten.'
        exit 1
    fi
fi
mkdir -p "$HOME/Applications"
/usr/bin/ditto --norsrc "$built_app" "$local_app"
# Strip only Finder/resource presentation metadata from this generated app.
# Preserve quarantine and all system security settings.
/usr/bin/xattr -dr com.apple.FinderInfo "$local_app" 2>/dev/null || true
/usr/bin/xattr -dr com.apple.ResourceFork "$local_app" 2>/dev/null || true
/usr/bin/codesign --force --deep --sign - "$local_app"
/usr/bin/codesign --verify --deep --strict "$local_app"
"$local_app/Contents/MacOS/Midnight Shift - The Last Lantern" -batchmode -nographics --verify-build -logFile "$game_root/Work/packaged-smoke.log"
/usr/bin/ditto -c -k --norsrc --keepParent "$local_app" "$game_root/Builds/MidnightShift-Mac-v0.13.zip"
