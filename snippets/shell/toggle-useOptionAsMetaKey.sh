#!/usr/bin/env bash
# Toggles useOptionAsMetaKey for the frontmost macOS Terminal profile.
# Changes apply to new windows and tabs that use the profile.

set -euo pipefail

readonly PLIST="${HOME}/Library/Preferences/com.apple.Terminal.plist"
readonly PLIST_BUDDY="/usr/libexec/PlistBuddy"

die() {
    printf '%s\n' "$1" >&2
    exit 1
}

main() {
    local profile key current_state

    command -v osascript >/dev/null 2>&1 || die 'osascript is required on macOS.'
    [[ -x "${PLIST_BUDDY}" ]] || die "${PLIST_BUDDY} is required on macOS."

    # 1. Get the name of the active Terminal profile
    profile=$(osascript -e 'tell application "Terminal" to name of current settings of first window' 2>/dev/null || true)
    [[ -n "${profile}" ]] || die 'Could not detect active Terminal profile.'

    # PlistBuddy separates key path segments with ':' and escapes spaces with '\'
    key=":Window\\ Settings:${profile// /\\ }:useOptionAsMetaKey"

    # 2. Read the current value; empty when the key does not exist yet
    current_state=$("${PLIST_BUDDY}" -c "Print ${key}" "${PLIST}" 2>/dev/null || true)

    # 3. Toggle the boolean value, creating the key when it is missing
    if [[ "${current_state}" == "true" ]]; then
        "${PLIST_BUDDY}" -c "Set ${key} false" "${PLIST}"
        printf "Option as Meta Key is now DISABLED for profile: '%s'\n" "${profile}"
    else
        "${PLIST_BUDDY}" -c "Add ${key} bool true" "${PLIST}" 2>/dev/null ||
            "${PLIST_BUDDY}" -c "Set ${key} true" "${PLIST}"
        printf "Option as Meta Key is now ENABLED for profile: '%s'\n" "${profile}"
    fi

    printf "Open a new tab or window using '%s' to see changes.\n" "${profile}"
}

main "$@"
