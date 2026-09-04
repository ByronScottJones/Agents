func DisableAltKeys() {
    // Pops the pushed flag state off the stack, restoring normal behavior
    os.Stdout.Write([]byte("\x1b[<1b"))
}
