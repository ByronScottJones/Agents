package main

import "os"

func EnableAltKeys() error {
	// CSI = 1 ; 1 b
	// This requests the terminal to turn on flag 1 (Disambiguate escape codes)
	_, err := os.Stdout.Write([]byte("\x1b[=1;1b"))
	return err
}
