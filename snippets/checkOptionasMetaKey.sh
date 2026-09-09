#!/usr/bin/env bash

set -euo pipefail

# 1. Identify the running terminal emulator
if [ "${TERM_PROGRAM:-}" = "Apple_Terminal" ]; then
    command -v osascript >/dev/null 2>&1 || {
        printf '%s\n' 'osascript is required on macOS.' >&2
        exit 1
    }

    echo "Terminal Detected: Standard macOS Terminal"
    echo "----------------------------------------"

    # 2. Dynamically fetch the name of the active profile/theme for the current tab
    ACTIVE_PROFILE=$(osascript -e 'tell application "Terminal" to get name of current settings of tab 1 of window 1' 2>/dev/null || true)

    if [ -z "$ACTIVE_PROFILE" ]; then
        # Fallback to the global default profile if AppleScript fails or lacks permissions
        command -v defaults >/dev/null 2>&1 || {
            printf '%s\n' 'defaults is required on macOS.' >&2
            exit 1
        }
        ACTIVE_PROFILE=$(defaults read com.apple.Terminal "Default Window Settings" 2>/dev/null || true)
        echo "Active Profile: $ACTIVE_PROFILE (Fallback/Default)"
    else
        echo "Active Profile: $ACTIVE_PROFILE"
    fi
    
    # 3. Extract the 'useOptionAsMetaKey' setting from the specific profile structure
    command -v plutil >/dev/null 2>&1 || {
        printf '%s\n' 'plutil is required on macOS.' >&2
        exit 1
    }
    META_SETTING=$(defaults read com.apple.Terminal "Window Settings" | plutil -extract "$ACTIVE_PROFILE.useOptionAsMetaKey" raw - 2>/dev/null || true)
    
    # 4. Evaluate the setting status
    if [ "$META_SETTING" = "1" ]; then
        echo "Option as Meta Key: ENABLED"
    else
        echo "Option as Meta Key: DISABLED"
    fi

elif [ "${TERM_PROGRAM:-}" = "kitty" ]; then
    echo "Terminal Detected: Kitty"
    echo "Configuration note: Kitty handles option/meta mappings natively via 'macos_option_as_alt' in kitty.conf."

else
    echo "Terminal Detected: Alternative Emulator (${TERM_PROGRAM:-unknown})"
fi
