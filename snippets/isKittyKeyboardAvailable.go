package main

// Confirm the following packages are imported.
import (
	"os"
	"time"

	"golang.org/x/term"
)

// IsKittyKeyboardAvailable queries the terminal to see if the Kitty keyboard
// protocol is supported. If true, Alt/Opt key patterns can be reliably detected.
func IsKittyKeyboardAvailable() (bool, error) {
	// 1. Ensure standard input and output are terminal devices
	stdinFD := int(os.Stdin.Fd())
	stdoutFD := int(os.Stdout.Fd())
	if !term.IsTerminal(stdinFD) || !term.IsTerminal(stdoutFD) {
		return false, nil
	}

	// 2. Put the terminal into raw mode so we can read unbuffered responses
	oldState, err := term.MakeRaw(stdinFD)
	if err != nil {
		return false, err
	}
	// Always restore the terminal state when exiting the function
	defer term.Restore(stdinFD, oldState)

	// 3. Send the Kitty keyboard protocol status query: CSI ? u
	_, err = os.Stdout.Write([]byte("\x1b[?u"))
	if err != nil {
		return false, err
	}

	// 4. Read the response with a strict timeout so the application doesn't hang
	// if the terminal doesn't support the protocol and ignores the query.
	readChan := make(chan []byte, 1)
	errChan := make(chan error, 1)

	go func() {
		buf := make([]byte, 32)
		n, err := os.Stdin.Read(buf)
		if err != nil {
			errChan <- err
			return
		}
		readChan <- buf[:n]
	}()

	select {
	case response := <-readChan:
		// A terminal supporting Kitty keyboard protocol will reply with CSI > flags u
		// Let's check if the response starts with CSI (`\x1b[`) and ends with `u`
		if len(response) >= 3 && response[0] == '\x1b' && response[1] == '[' && response[len(response)-1] == 'u' {
			return true, nil
		}
		return false, nil

	case err := <-errChan:
		return false, err

	case <-time.After(100 * time.Millisecond):
		// Timeout reached; terminal likely doesn't support the protocol
		return false, nil
	}
}
