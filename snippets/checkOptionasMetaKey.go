package main

import (
	"bytes"
	"fmt"
	"os"
	"os/exec"
	"runtime"
	"strings"
)

// TerminalInfo holds the detected configurations
type TerminalInfo struct {
	Emulator      string
	ActiveProfile string
	MetaKeyEnabled bool
}

// CheckTerminalSession analyzes the current terminal emulator environment
func CheckTerminalSession() TerminalInfo {
	info := TerminalInfo{
		Emulator:      os.Getenv("TERM_PROGRAM"),
		ActiveProfile: "Unknown",
		MetaKeyEnabled: false,
	}

	// Normalized edge case where TERM_PROGRAM might be empty
	if info.Emulator == "" {
		info.Emulator = "Generic/Unknown"
		return info
	}

	// Stop execution here if we are not on macOS
	if runtime.GOOS != "darwin" {
		return info
	}

	// Process macOS specific terminal configurations
	if info.Emulator == "Apple_Terminal" {
		info.ActiveProfile = getActiveProfile()
		info.MetaKeyEnabled = isMetaKeyEnabled(info.ActiveProfile)
	}

	return info
}

// getActiveProfile fetches the live profile using AppleScript, with a plist fallback
func getActiveProfile() string {
	// 1. Attempt live check via AppleScript
	cmd := exec.Command("osascript", "-e", `tell application "Terminal" to get name of current settings of tab 1 of window 1`)
	var out bytes.Buffer
	cmd.Stdout = &out
	
	if err := cmd.Run(); err == nil {
		profile := strings.TrimSpace(out.String())
		if profile != "" {
			return profile
		}
	}

	// 2. Fallback to global default if AppleScript fails/is blocked
	cmdFallback := exec.Command("defaults", "read", "com.apple.Terminal", "Default Window Settings")
	out.Reset()
	cmdFallback.Stdout = &out
	
	if err := cmdFallback.Run(); err == nil {
		return strings.TrimSpace(out.String())
	}

	return "Basic" // Standard system fallback
}

// isMetaKeyEnabled parses the nested plist structure using plutil
func isMetaKeyEnabled(profile string) bool {
	// Construct the command pipeline equivalent to:
	// defaults read com.apple.Terminal "Window Settings" | plutil -extract "ProfileName.useOptionAsMetaKey" raw -
	defaultsCmd := exec.Command("defaults", "read", "com.apple.Terminal", "Window Settings")
	plutilCmd := exec.Command("plutil", "-extract", profile+".useOptionAsMetaKey", "raw", "-")

	// Pipe the output of defaults into plutil
	var defaultsOut bytes.Buffer
	defaultsCmd.Stdout = &defaultsOut
	
	if err := defaultsCmd.Run(); err != nil {
		return false
	}

	var plutilOut bytes.Buffer
	plutilCmd.Stdin = &defaultsOut
	plutilCmd.Stdout = &plutilOut

	if err := plutilCmd.Run(); err != nil {
		return false
	}

	result := strings.TrimSpace(plutilOut.String())
	return result == "1"
}

func main() {
	info := CheckTerminalSession()

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
