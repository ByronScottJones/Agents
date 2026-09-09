# Code Snippets

- This directory contains code fragments to assist in development.
- Snippets are intended to be incorporated into larger projects, not run as
  standalone programs.
- Review each snippet's required imports, package context, error-handling
  conventions, and integration assumptions before using it.
- Formatting checks are useful, but standalone compilation is not required.
- Snippets are examples, not production-ready libraries. Verify dependencies,
  platform support, permissions, and error handling before reuse.
- Keep the catalog filename synchronized with the files in this directory.

## Bash / Zsh

- checkOptionasMetaKey.sh
  - checks if the current terminal session supports using the Option key as a Meta/Alt key.

The Go snippets require Go modules and any imports shown in their file. Format
Go snippets with `gofmt` before reuse. Terminal protocol snippets require a
real terminal input/output device and should be tested with a pseudoterminal.

## Go

- checkOptionasMetaKey.go
  - checks if the current terminal session supports using the Option key as a Meta/Alt key.
- isKittyKeyboardAvailable.go
  - checks if the Kitty Terminal Keyboard Protocol is available.
- isAltKeyActive.go
  - checks if the current Kitty keyboard flags enable Alt key tracking.
- enableAltKeys.go
  - Enable the Alt Key in the Terminal.
- DisableAltKeys.go
  - Disable the Alt Key in the Terminal.
