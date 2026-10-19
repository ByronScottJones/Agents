package main

// Confirm the following packages are imported.
import (
	"fmt"
	"os"
	"time"

	"golang.org/x/term"
)

// IsKittyKeyboardAvailable queries the terminal to see if the Kitty keyboard
// protocol is supported. If true, Alt/Opt key patterns can be reliably detected.
func IsKittyKeyboardAvailable() (available bool, err error) {
	// 1. Ensure standard input and output are terminal devices
	stdinFD := int(os.Stdin.Fd())
	stdoutFD := int(os.Stdout.Fd())
	if !term.IsTerminal(stdinFD) || !term.IsTerminal(stdoutFD) {
		return false, nil
	}

	// 2. Put the terminal into raw mode so we can read unbuffered responses
	oldState, err := term.MakeRaw(stdinFD)
	if err != nil {
		return false, fmt.Errorf("enter raw mode: %w", err)
	}
	// Always restore the terminal state when exiting the function
	defer func() {
		if rerr := term.Restore(stdinFD, oldState); rerr != nil && err == nil {
			err = fmt.Errorf("restore terminal: %w", rerr)
		}
	}()

	// 3. Send the Kitty keyboard protocol status query: CSI ? u
	if _, err = os.Stdout.WriteString("\x1b[?u"); err != nil {
		return false, fmt.Errorf("query keyboard protocol: %w", err)
	}

	// 4. Read the response with a strict timeout so the application doesn't hang
	// if the terminal doesn't support the protocol and ignores the query.
	readChan := make(chan []byte, 1)
	errChan := make(chan error, 1)

	go func() {
		buf := make([]byte, 32)
		n, rerr := os.Stdin.Read(buf)
		if rerr != nil {
			errChan <- rerr
			return
		}
		readChan <- buf[:n]
	}()

	select {
	case response := <-readChan:
		// A terminal supporting Kitty keyboard protocol will reply with CSI ? flags u
		// Check that the response starts with CSI (`\x1b[`) and ends with `u`
		if len(response) >= 3 && response[0] == '\x1b' && response[1] == '[' && response[len(response)-1] == 'u' {
			return true, nil
		}
		return false, nil

	case rerr := <-errChan:
		return false, fmt.Errorf("read keyboard protocol response: %w", rerr)

	case <-time.After(100 * time.Millisecond):
		// Timeout reached; terminal likely doesn't support the protocol
		return false, nil
	}
}
