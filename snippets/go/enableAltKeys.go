package main

import (
	"fmt"
	"os"
)

// EnableAltKeys asks the terminal to push Kitty keyboard flag 1
// (disambiguate escape codes) so Alt/Option key presses are reported.
// Pair every call with DisableAltKeys.
func EnableAltKeys() error {
	// CSI = 1 ; 1 b
	if _, err := os.Stdout.WriteString("\x1b[=1;1b"); err != nil {
		return fmt.Errorf("enable alt keys: %w", err)
	}
	return nil
}
