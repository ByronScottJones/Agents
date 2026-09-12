---
title: "Tauri & Rust Best Practices"
description: "Load this file when developing an application using the Tauri (v2) and Rust stack."
applies_to: ["src-tauri/**/*.rs", "src-tauri/tauri.conf.json", "tauri.conf.json"]
---

# Tauri & Rust Best Practices (Agent Context)

This document outlines the standard best practices, architectural patterns, and conventions for developing applications using the Tauri (v2) and Rust stack.

## 1. Architectural Philosophy

* **Rust for Heavy Lifting:** Offload complex calculations, file I/O, database interactions, networking, and OS-level operations to the Rust backend.
* **Thin Frontend:** Treat your frontend (Svelte, etc.) as a lightweight presentation layer.
* **Minimize IPC Overhead:** Serialization and deserialization across the IPC bridge (Inter-Process Communication) incurs a performance cost. Batch operations when possible instead of making thousands of rapid, small `invoke` calls.

## JavaScript/TypeScript UI Frameworks

* Svelte is the preferred toolkit by the Tauri team.
  **If you use SvelteKit** you must explicitly use adapter-static because Tauri does not support Server-Side Rendering (SSR).
* Other Frameworks such as React, Solid.js, Vue.js, can be used as needed.
* Tauri cannot host a Node.js server for Server-Side Rendering. Frameworks must generate static, single page code.

## 2. Project Structure

A standard Tauri v2 project should maintain a clear separation of concerns:

```text
├── src/                  # Frontend assets (React, Vue, Vite config, etc.)
├── src-tauri/            # Rust backend
│   ├── capabilities/     # Granular permission files (Tauri v2)
│   ├── src/              
│   │   ├── commands/     # IPC command definitions
│   │   ├── state/        # Application state management
│   │   ├── error.rs      # Custom application errors
│   │   ├── lib.rs        # Main library entry point
│   │   └── main.rs       # App initialization
│   ├── Cargo.toml
│   └── tauri.conf.json
```

## 3. Rust Backend Best Practices

### 3.1. Command Error Handling

Always return a `Result` from your Tauri commands when operations can fail. The error type must implement `serde::Serialize` so that it can be passed across the IPC boundary and correctly reject the JavaScript Promise.

```rust
use serde::Serialize;
use thiserror::Error;

#[derive(Debug, Error)]
pub enum AppError {
    #[error("I/O error: {0}")]
    Io(#[from] std::io::Error),
    #[error("Database error: {0}")]
    Database(String),
}

// Manually implement Serialize for custom Error enums
impl Serialize for AppError {
    fn serialize<S>(&self, serializer: S) -> Result<S::Ok, S::Error>
    where
        S: serde::ser::Serializer,
    {
        serializer.serialize_str(self.to_string().as_ref())
    }
}

#[tauri::command]
pub async fn read_file_data(path: String) -> Result<String, AppError> {
    let content = std::fs::read_to_string(path)?;
    Ok(content)
}
```

### 3.2. State Management

Use `tauri::State` for dependency injection. When dealing with mutable state, wrap your data in a `std::sync::Mutex` (for synchronous commands) or `tokio::sync::Mutex` (for asynchronous commands).

```rust
use std::sync::Mutex;
use tauri::State;

struct AppState {
    counter: Mutex<u32>,
}

#[tauri::command]
fn increment_counter(state: State<'_, AppState>) -> Result<u32, String> {
    let mut counter = state.counter.lock().map_err(|e| e.to_string())?;
    *counter += 1;
    Ok(*counter)
}

// In setup:
// app.manage(AppState { counter: Mutex::new(0) });
```

### 3.3. Asynchronous Commands

Use `async` for commands that perform I/O (network requests, heavy file operations) so you don't block the IPC thread.

```rust
#[tauri::command]
pub async fn fetch_data_from_api(url: String) -> Result<String, AppError> {
    // Runs on a Tokio worker thread natively
    let response = reqwest::get(&url).await.map_err(|e| AppError::Database(e.to_string()))?;
    let text = response.text().await.map_err(|e| AppError::Database(e.to_string()))?;
    Ok(text)
}
```

## 4. Frontend Integration Best Practices

### 4.1. Strongly Typed IPC Calls (TypeScript)

Always wrap Tauri `invoke` calls in typed service functions to maintain end-to-end type safety between Rust and TypeScript.

```typescript
import { invoke } from '@tauri-apps/api/core';

// Define expected return types
export interface UserProfile {
  id: number;
  name: string;
}

export const fetchUserProfile = async (userId: number): Promise<UserProfile> => {
  try {
    // The Rust command name is exactly as defined, but in camelCase usually via tauri-specta or manual mapping
    return await invoke<UserProfile>('get_user_profile', { id: userId });
  } catch (error) {
    console.error('Failed to fetch profile:', error);
    throw error;
  }
};
```

### 4.2. Event Driven Communication

Use Tauri events for unidirectional streams (e.g., progress bars, backend-initiated alerts) rather than polling from the frontend.

**Rust:**

```rust
use tauri::Emitter;

#[tauri::command]
fn start_long_task(app_handle: tauri::AppHandle) {
    std::thread::spawn(move || {
        for i in 1..=100 {
            app_handle.emit("download-progress", i).unwrap();
            std::thread::sleep(std::time::Duration::from_millis(50));
        }
    });
}
```

**TypeScript:**

```typescript
import { listen, UnlistenFn } from '@tauri-apps/api/event';
import { useEffect, useState } from 'react';

export function useDownloadProgress() {
  const [progress, setProgress] = useState(0);

  useEffect(() => {
    let unlisten: UnlistenFn;
    
    listen<number>('download-progress', (event) => {
      setProgress(event.payload);
    }).then(fn => unlisten = fn);

    return () => {
      if (unlisten) unlisten();
    };
  }, []);

  return progress;
}
```

## 5. Security & Permissions (Tauri v2)

Treat frontend input, filesystem paths, URLs, and IPC arguments as untrusted.
Validate them at the command boundary and expose only the minimum operations
needed by the frontend.

### 5.1. Capabilities

In Tauri v2, do not globally allow plugins or API methods. Use granular capability files inside `src-tauri/capabilities/` to restrict exactly what the frontend can call.

```json
// src-tauri/capabilities/default.json
{
  "$schema": "../gen/schemas/desktop-schema.json",
  "identifier": "default",
  "description": "Base capability for the application",
  "windows": ["main"],
  "permissions": [
    "core:default",
    "fs:default",
    {
      "identifier": "fs:allow-read-text-file",
      "allow": [{ "path": "$APPDATA/config.json" }]
    }
  ]
}
```

Official Tauri Plugins are hosted at:
<https://github.com/tauri-apps/plugins-workspace>. Check
<https://github.com/tauri-apps/awesome-tauri> when additional functionality is
needed.

### 5.2. Content Security Policy (CSP)

Enforce a strict CSP in `tauri.conf.json`. Disable remote execution.

```json
{
  "app": {
    "security": {
      "csp": "default-src 'self'; img-src 'self' asset: https://asset.localhost; connect-src ipc: http://ipc.localhost"
    }
  }
}
```

## 6. Build and Distribution

* Use `vite` for the fastest frontend build times.
* Always build the frontend for production before building the Rust binary (`tauri build`).
* Use `cargo clippy` and `cargo fmt` as standard CI steps for the Rust backend.
* Keep public frontend environment variables explicitly prefixed and never put
  secrets in frontend configuration.
* Use `tauri-plugin-store` or an equivalent platform-aware store for persisted
  user settings rather than ad hoc files in arbitrary paths.
* Review updater signing, deep-link handling, window permissions, and release
  artifacts as part of the distribution security review.
