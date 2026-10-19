#!/usr/bin/env bash
# Reports whether the current terminal session can use the Option key as Meta/Alt.

set -euo pipefail

require_command() {
    command -v "$1" >/dev/null 2>&1 || {
        printf '%s is required on macOS.\n' "$1" >&2
        exit 1
    }
}

check_apple_terminal() {
    local profile meta_setting

    require_command osascript
    printf 'Terminal Detected: Standard macOS Terminal\n'
    printf -- '----------------------------------------\n'

    # 1. Fetch the name of the active profile for the current tab
    profile=$(osascript -e 'tell application "Terminal" to get name of current settings of tab 1 of window 1' 2>/dev/null || true)

    if [[ -z "${profile}" ]]; then
        # Fall back to the global default profile if AppleScript fails or lacks permissions
        require_command defaults
        profile=$(defaults read com.apple.Terminal "Default Window Settings" 2>/dev/null || true)
        printf 'Active Profile: %s (Fallback/Default)\n' "${profile}"
    else
        printf 'Active Profile: %s\n' "${profile}"
    fi

    # 2. Extract 'useOptionAsMetaKey' from the profile's settings
    require_command plutil
    meta_setting=$(defaults read com.apple.Terminal "Window Settings" |
        plutil -extract "${profile}.useOptionAsMetaKey" raw - 2>/dev/null || true)

    # 3. Report the setting status
    if [[ "${meta_setting}" == "1" ]]; then
        printf 'Option as Meta Key: ENABLED\n'
    else
        printf 'Option as Meta Key: DISABLED\n'
    fi
}

main() {
    case "${TERM_PROGRAM:-}" in
    Apple_Terminal)
        check_apple_terminal
        ;;
    kitty)
        printf 'Terminal Detected: Kitty\n'
        printf "Configuration note: Kitty handles option/meta mappings natively via 'macos_option_as_alt' in kitty.conf.\n"
        ;;
    *)
        printf 'Terminal Detected: Alternative Emulator (%s)\n' "${TERM_PROGRAM:-unknown}"
        ;;
    esac
}

main "$@"
