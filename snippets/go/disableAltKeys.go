package main

import (
	"fmt"
	"os"
)

// DisableAltKeys pops the flag state pushed by EnableAltKeys off the
// terminal's stack, restoring normal key reporting.
func DisableAltKeys() error {
	// CSI < 1 b
	if _, err := os.Stdout.WriteString("\x1b[<1b"); err != nil {
		return fmt.Errorf("disable alt keys: %w", err)
	}
	return nil
}
