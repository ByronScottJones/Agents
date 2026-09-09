package main

import "os"

func DisableAltKeys() error {
	// Pops the pushed flag state off the stack, restoring normal behavior
	_, err := os.Stdout.Write([]byte("\x1b[<1b"))
	return err
}
