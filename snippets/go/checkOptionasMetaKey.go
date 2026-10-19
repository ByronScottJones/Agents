package main

import (
	"bytes"
	"context"
	"fmt"
	"os"
	"os/exec"
	"runtime"
	"strings"
	"time"
)

// commandTimeout bounds each helper command; osascript can block on a
// permission prompt.
const commandTimeout = 5 * time.Second

// TerminalInfo holds the detected configurations
type TerminalInfo struct {
	Emulator       string
	ActiveProfile  string
	MetaKeyEnabled bool
}

// CheckTerminalSession analyzes the current terminal emulator environment.
func CheckTerminalSession(ctx context.Context) (TerminalInfo, error) {
	info := TerminalInfo{
		Emulator:       os.Getenv("TERM_PROGRAM"),
		ActiveProfile:  "Unknown",
		MetaKeyEnabled: false,
	}

	// Normalized edge case where TERM_PROGRAM might be empty
	if info.Emulator == "" {
		info.Emulator = "Generic/Unknown"
		return info, nil
	}

	// Stop execution here if we are not on macOS
	if runtime.GOOS != "darwin" {
		return info, nil
	}

	// Process macOS specific terminal configurations
	if info.Emulator == "Apple_Terminal" {
		profile, err := getActiveProfile(ctx)
		if err != nil {
			return info, err
		}
		info.ActiveProfile = profile

		enabled, err := isMetaKeyEnabled(ctx, profile)
		if err != nil {
			return info, err
		}
		info.MetaKeyEnabled = enabled
	}

	return info, nil
}

// output runs a command with the package timeout and returns its trimmed stdout.
func output(ctx context.Context, stdin *bytes.Buffer, name string, args ...string) (string, error) {
	ctx, cancel := context.WithTimeout(ctx, commandTimeout)
	defer cancel()

	cmd := exec.CommandContext(ctx, name, args...)
	var out bytes.Buffer
	cmd.Stdout = &out
	if stdin != nil {
		cmd.Stdin = stdin
	}
	if err := cmd.Run(); err != nil {
		return "", fmt.Errorf("run %s: %w", name, err)
	}
	return strings.TrimSpace(out.String()), nil
}

// getActiveProfile fetches the live profile using AppleScript, with a plist fallback
func getActiveProfile(ctx context.Context) (string, error) {
	// 1. Attempt live check via AppleScript
	profile, err := output(ctx, nil, "osascript", "-e",
		`tell application "Terminal" to get name of current settings of tab 1 of window 1`)
	if err == nil && profile != "" {
		return profile, nil
	}

	// 2. Fallback to global default if AppleScript fails/is blocked
	profile, err = output(ctx, nil, "defaults", "read", "com.apple.Terminal", "Default Window Settings")
	if err != nil {
		return "", fmt.Errorf("read default terminal profile: %w", err)
	}
	return profile, nil
}

// isMetaKeyEnabled parses the nested plist structure using plutil
func isMetaKeyEnabled(ctx context.Context, profile string) (bool, error) {
	// Equivalent to:
	// defaults read com.apple.Terminal "Window Settings" | plutil -extract "ProfileName.useOptionAsMetaKey" raw -
	settings, err := output(ctx, nil, "defaults", "read", "com.apple.Terminal", "Window Settings")
	if err != nil {
		return false, fmt.Errorf("read terminal window settings: %w", err)
	}

	// A profile without the key set is treated as disabled
	result, err := output(ctx, bytes.NewBufferString(settings), "plutil", "-extract",
		profile+".useOptionAsMetaKey", "raw", "-")
	if err != nil {
		return false, nil
	}
	return result == "1", nil
}

func main() {
	ctx := context.Background()

	info, err := CheckTerminalSession(ctx)
	if err != nil {
		fmt.Fprintf(os.Stderr, "check terminal session: %v\n", err)
		os.Exit(1)
	}

	fmt.Printf("Terminal Detected: %s\n", info.Emulator)

	switch info.Emulator {
	case "Apple_Terminal":
		fmt.Printf("Active Profile:    %s\n", info.ActiveProfile)
		fmt.Printf("Option as Meta:    %t\n", info.MetaKeyEnabled)
	case "kitty":
		fmt.Println("Note: Kitty handles option/meta natively via 'macos_option_as_alt' in kitty.conf.")
	default:
		fmt.Println("Running in an unhandled alternative terminal environment.")
	}
}
