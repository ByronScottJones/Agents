package main

// Make sure the following are imported.
import (
	"fmt"
	"os"
	"strconv"
	"strings"
	"time"

	"golang.org/x/term"
)

// IsAltKeyActive checks if the current Kitty keyboard flags enable Alt key tracking.
// This assumes IsKittyKeyboardAvailable() has already returned true.
func IsAltKeyActive() (active bool, err error) {
	stdinFD := int(os.Stdin.Fd())
	stdoutFD := int(os.Stdout.Fd())
	if !term.IsTerminal(stdinFD) || !term.IsTerminal(stdoutFD) {
		return false, nil
	}

	// 1. Switch to raw mode to read the response stream
	oldState, err := term.MakeRaw(stdinFD)
	if err != nil {
		return false, fmt.Errorf("enter raw mode: %w", err)
	}
	defer func() {
		if rerr := term.Restore(stdinFD, oldState); rerr != nil && err == nil {
			err = fmt.Errorf("restore terminal: %w", rerr)
		}
	}()

	// 2. Query the current active flags using CSI ? u
	if _, err = os.Stdout.WriteString("\x1b[?u"); err != nil {
		return false, fmt.Errorf("query keyboard flags: %w", err)
	}

	// 3. Read response with a safe execution timeout
	readChan := make(chan []byte, 1)
	errChan := make(chan error, 1)

	go func() {
		buf := make([]byte, 64)
		n, rerr := os.Stdin.Read(buf)
		if rerr != nil {
			errChan <- rerr
			return
		}
		readChan <- buf[:n]
	}()

	select {
	case response := <-readChan:
		// The terminal responds with: CSI ? {flags} u
		// Example matching strings: "\x1b[?1u" or "\x1b[?3u"
		respStr := string(response)
		if !strings.HasPrefix(respStr, "\x1b[?") || !strings.HasSuffix(respStr, "u") {
			return false, nil
		}

		// Extract the numeric flags from between "\x1b[?" and "u".
		// TrimPrefix/TrimSuffix stay safe on a bare "\x1b[?u" response.
		flagStr := strings.TrimSuffix(strings.TrimPrefix(respStr, "\x1b[?"), "u")
		flags, perr := strconv.Atoi(flagStr)
		if perr != nil {
			return false, fmt.Errorf("parse keyboard flags %q: %w", flagStr, perr)
		}

		// Bit 1 (value 1): Disambiguate escape codes (Reports Alt perfectly)
		// Bit 2 (value 2): Report event types (Press/Repeat/Release with modifiers)
		// If either bit is flipped active, Alt-keys are structurally usable.
		return flags&(1|2) != 0, nil

	case rerr := <-errChan:
		return false, fmt.Errorf("read keyboard flags response: %w", rerr)

	case <-time.After(100 * time.Millisecond):
		return false, nil
	}
}
