#!/bin/bash

# 1. Identify the running terminal emulator
if [ "$TERM_PROGRAM" = "Apple_Terminal" ]; then
    echo "Terminal Detected: Standard macOS Terminal"
    echo "----------------------------------------"

    # 2. Dynamically fetch the name of the active profile/theme for the current tab
    ACTIVE_PROFILE=$(osascript -e 'tell application "Terminal" to get name of current settings of tab 1 of window 1' 2>/dev/null)

    if [ -z "$ACTIVE_PROFILE" ]; then
        # Fallback to the global default profile if AppleScript fails or lacks permissions
        ACTIVE_PROFILE=$(defaults read com.apple.Terminal "Default Window Settings" 2>/dev/null)
        echo "Active Profile: $ACTIVE_PROFILE (Fallback/Default)"
    else
        echo "Active Profile: $ACTIVE_PROFILE"
    fi
    
    # 3. Extract the 'useOptionAsMetaKey' setting from the specific profile structure
    META_SETTING=$(defaults read com.apple.Terminal "Window Settings" | plutil -extract "$ACTIVE_PROFILE.useOptionAsMetaKey" raw - 2>/dev/null)
    
    # 4. Evaluate the setting status
    if [ "$META_SETTING" = "1" ]; then
        echo "Option as Meta Key: ENABLED"
    else
        echo "Option as Meta Key: DISABLED"
    fi

elif [ "$TERM_PROGRAM" = "kitty" ]; then
    echo "Terminal Detected: Kitty"
    echo "Configuration note: Kitty handles option/meta mappings natively via 'macos_option_as_alt' in kitty.conf."

else
    echo "Terminal Detected: Alternative Emulator ($TERM_PROGRAM)"
fi
