#!/bin/bash

# 1. Dynamically get the name of the currently active Terminal profile
CURRENT_PROFILE=$(osascript -e 'tell application "Terminal" to name of current settings of first window' 2>/dev/null)

# Fallback if AppleScript fails
if [ -z "$CURRENT_PROFILE" ]; then
    echo "Could not detect active Terminal profile. Exiting."
    exit 1
fi

PLIST="$HOME/Library/Preferences/com.apple.Terminal.plist"

# 2. Check the current value for this specific profile
CURRENT_STATE=$(/usr/libexec/PlistBuddy -c "Print :Window\ Settings:${CURRENT_PROFILE}:useOptionAsMetaKey" "$PLIST" 2>/dev/null)

# 3. Toggle the boolean value
if [ "$CURRENT_STATE" = "true" ]; then
    /usr/libexec/PlistBuddy -c "Set :Window\ Settings:${CURRENT_PROFILE}:useOptionAsMetaKey false" "$PLIST"
    echo "Option as Meta Key is now DISABLED for profile: '$CURRENT_PROFILE'"
else
    # Create key as bool if it doesn't exist, otherwise update it
    /usr/libexec/PlistBuddy -c "Add :Window\ Settings:${CURRENT_PROFILE}:useOptionAsMetaKey bool true" "$PLIST" 2>/dev/null || \
    /usr/libexec/PlistBuddy -c "Set :Window\ Settings:${CURRENT_PROFILE}:useOptionAsMetaKey true" "$PLIST"
    echo "Option as Meta Key is now ENABLED for profile: '$CURRENT_PROFILE'"
fi

# Note: Changes apply to NEW windows/tabs using this profile.
echo "Open a new tab or window using '$CURRENT_PROFILE' to see changes."
