//Make sure the following are imported
import (
	"os"
	"strconv"
	"strings"
	"time"

	"golang.org/x/term"
)

// IsAltKeyActive checks if the current Kitty keyboard flags enable Alt key tracking.
// This assumes IsKittyKeyboardAvailable() has already returned true.
func IsAltKeyActive() (bool, error) {
	fd := int(os.Stdout.Fd())
	if !term.IsTerminal(fd) {
		return false, nil
	}

	// 1. Switch to raw mode to read the response stream
	oldState, err := term.MakeRaw(fd)
	if err != nil {
		return false, err
	}
	defer term.Restore(fd, oldState)

	// 2. Query the current active flags using CSI ? u
	_, err = os.Stdout.Write([]byte("\x1b[?u"))
	if err != nil {
		return false, err
	}

	// 3. Read response with a safe execution timeout
	readChan := make(chan []byte, 1)
	errChan := make(chan error, 1)

	go func() {
		buf := make([]byte, 64)
		n, err := os.Stdin.Read(buf)
		if err != nil {
			errChan <- err
			return
		}
		readChan <- buf[:n]
	}()

	select {
	case response := <-readChan:
		respStr := string(response)
		
		// The terminal responds with: CSI ? {flags} u
		// Example matching strings: "\x1b[?1u" or "\x1b[?3u"
		if !strings.HasPrefix(respStr, "\x1b[?") || !strings.HasSuffix(respStr, "u") {
			return false, nil
		}

		// Extract the numeric flags from between "\x1b[?" and "u"
		flagStr := respStr[4 : len(respStr)-1]
		flags, err := strconv.Atoi(flagStr)
		if err != nil {
			return false, nil
		}

		// Bit 1 (value 1): Disambiguate escape codes (Reports Alt perfectly)
		// Bit 2 (value 2): Report event types (Press/Repeat/Release with modifiers)
		// If either bit is flipped active, Alt-keys are structurally usable.
		if (flags & 1) != 0 || (flags & 2) != 0 {
			return true, nil
		}
		return false, nil

	case err := <-errChan:
		return false, err

	case <-time.After(100 * time.Millisecond):
		return false, nil
	}
}
