# Rust Style, API & Lint Guide (`rustfmt` & `clippy` Standard)

Adhere strictly to the official Rust Style Guide (`rustfmt` defaults), Rust API Guidelines, and idiomatic `clippy` rules. All generated Rust code and `Cargo.toml` files must match these conventions.

## 1. Core Formatting & Whitespace
- **Indentation:** 4 spaces per level (never tabs).
- **Line Width:** Max 100 characters for code; max 80 characters for comments.
- **Indent Style:** Always use **block indent** over visual indent.
- **Trailing Commas:** Required on all multi-line comma-separated lists (structs, enums, args, tuples, arrays, `where` clauses, non-block `match` arms). Omit on single-line lists and `..base` functional record updates.
- **Whitespace:** Zero trailing whitespace. Separate items/statements by 0 or 1 blank lines.
- **Sorting:** Use "version sorting" (numeric chunks compared by value, e.g., `u8` before `u16`; `_` before characters; `UpperCamelCase` before `snake_case`).

## 2. Naming & Idioms
- **`UpperCamelCase`:** Types, traits, enum variants.
- **`snake_case`:** Functions, methods, modules, local variables, struct fields, macros.
- **`SCREAMING_SNAKE_CASE`:** `const` and immutable `static` items.
- **Reserved Words:** Use raw identifiers (`r#crate`) or trailing underscores (`crate_`), never misspellings (`krate`).
- **Expression-Oriented:** Prefer `let x = if cond { 1 } else { 0 };` over uninitialized `let x;` mutation.
- **Comparisons:** Prefer dereferencing over referencing (`*t == u` instead of `t == &u`).

## 3. Comments & Attributes
- **Comments:** Prefer `// ` over `/* ... */`. Place on own line above code. Write complete sentences (capitalized, ending with `.`).
- **Doc Comments:** Prefer outer `/// ` over `/** ... */`. Use inner `//! ` only for crate/module-level docs. **Always place doc comments *before* attributes.**
- **Attributes:** One attribute per line, indented with the item. Combine derives into a single `#[derive(Foo, Bar)]` preserving order. Space around `=` (`#[foo = 42]`).

## 4. Module Ordering & Imports
- **Top-of-File Order:** `extern crate` (alphabetical) -> `use` imports -> `mod foo;` declarations -> other items. Do not reorder `#[macro_use]` modules.
- **Imports (`use`):**
  - Keep on one line if it fits; no spaces inside `{}` (`use a::b::{c, d};`).
  - Version-sort within contiguous import groups (separated by blank lines).
  - Inside `{}` lists: `self` and `super` come first, regular names version-sorted, nested groups and `*` globs come last.
  - Normalize: `use a::self;` -> `use a;`, `use a::{b};` -> `use a::b;`, remove `use a::{};`.
  - If an import list contains any nested groups (`u::{...}`), format the outer brace block across multiple lines.

## 5. Items (`fn`, `struct`, `enum`, `trait`, `impl`)
- **Functions:** `[pub] [unsafe] [extern "ABI"] fn name(arg1: T1, arg2: T2) -> Ret {`. Always specify the ABI string for `extern` items (`extern "C"`). If multi-line args, break after `(`, block-indent one per line with trailing comma, put `) -> Ret {` on its own line.
- **Structs & Enums:**
  - Prefer unit structs (`struct Foo;`) over empty brace/tuple structs (`struct Foo {}` / `struct Foo();`).
  - Format empty traits/impls on one line: `trait Foo {}`, `impl Foo {}`.
  - Multi-line struct fields and enum variants: block-indented, one per line, trailing comma.
  - Small struct literals/variants may stay on one line with spaces inside braces: `Foo { a: 1, b: 2 }`.
- **Generics & `where` Clauses:**
  - Keep `<T: Display, U: Debug>` on one line; move long bounds to a `where` clause. Space around `=` in associated type bounds (`<T: Iterator<Item = u32>>`).
  - Place `where` on a new line (unindented) unless immediately following a closing `)` on its own line. Block-indent each bound with a trailing comma; put `{` on a new line after `where`.
  - When breaking multi-trait bounds (`+`), break *before* every `+` and block-indent.

## 6. Statements & Expressions
- **`let` & `let-else`:**
  - `let pat: Type = expr;` (space after `:`, spaces around `=`). If multi-line, break after `=` first and block-indent.
  - Single-line `let-else` allowed if short and body is a single expression: `let Some(x) = opt else { return };`. Otherwise, put `else {` on the same line (or aligned with `let` if multi-line) and block-indent the diverging body.
- **Statement Semicolons:** Always terminate statements and `()`-returning calls in statement position with `;` (except block expressions). Use `()` or `[]` for statement macros (`vec![];`), `macro_rules! foo { ... }` with `{}` for definitions.
- **Calls & Combinable Expressions:**
  - Multi-line calls: break after `(`, one arg per line, trailing comma.
  - **Combinable rule:** If an outer call takes a single multi-line argument (closure, struct literal, array, inner call), keep the outer delimiter hugging the inner expression without double-indenting:
    ```rust
    foo(Bar {
        field: val,
    });
    items.iter().map(|x| {
        let y = x + 1;
        y * 2
    });
    ```
- **Method Chains:** If multi-line, break before `.` and after `?`, placing each call on its own block-indented line.
- **Operators & Ranges:**
  - No space after unary ops (`!x`, `*x`, `&x`), except space after `&mut x`.
  - Spaces around binary ops (`x + 1`) and `as` casts. When breaking binary expressions, put the operator at the *start* of the next block-indented line (except `=`, which stays at the end of the first line).
  - No spaces in ranges (`0..10`, `..=y`). Wrap compound bounds in parens (`0..(x - 1)`).
- **Control Flow (`if`, `match`, `for`, `while`):**
  - Never put parens around `if`/`while` conditions.
  - Single-line `if else` allowed only in expression context when small: `let y = if x { 0 } else { 1 };`.
  - If a control condition wraps across lines, place the opening `{` on its own unindented line.
  - `match` arms: never start a pattern with a leading `|`. Use a trailing comma on non-block arms. Prefer wrapping the right-hand side in `{ ... }` rather than line-breaking the left-hand pattern.

## 7. `Cargo.toml` Conventions
- **Section Ordering:** `[package]` must be first. Within `[package]`, order keys as: `name`, `version`, [other keys version-sorted], `description` (last).
- **Sorting & Spacing:** Version-sort keys in all other sections (`[dependencies]`, etc.). Put 1 blank line *between* sections, 0 blank lines inside a section. Spaces around `=` (`key = "val"`). Use bare keys (unquoted) unless required by TOML syntax.
- **Tables & Arrays:** Keep inline dependency tables (`serde = { version = "1.0", features = ["derive"] }`) on a single line. If it exceeds 100 chars, convert to a `[dependencies.crate_name]` section or block-indent array items with a trailing comma.
- **Metadata:** `license` must be a valid SPDX expression. Wrap `description` at 80 columns and do not start it with the crate's name.

## 8. Rust API Guidelines
- **Conversion Prefixes (Cost & Ownership):**
  - `as_`: Free borrowed-to-borrowed view (`as_str()`, `as_bytes()`). Never allocates.
  - `to_`: Borrowed-to-owned conversion (`to_string()`, `to_vec()`) or expensive computation. May allocate.
  - `into_`: Owned-to-owned consuming conversion (`into_inner()`, `into_bytes()`). Preserves underlying allocation where possible.
- **Getters & Iterators:**
  - Omit the `get_` prefix for field accessors (`fn name(&self) -> &str`, `fn name_mut(&mut self) -> &mut str`). Use `get` only for lookup/indexing (`get(index)`).
  - Standard collection iteration methods: `iter(&self)`, `iter_mut(&mut self)`, `into_iter(self)`, paired with `IntoIterator` implementations for `&T`, `&mut T`, and `T`.
- **Constructors & Builders:**
  - Prefer `new()` for primary construction, and `with_...()` for secondary/parameterized constructors (`with_capacity`). Implement `Default` whenever a parameterless `new()` exists.
  - Use the Builder pattern (`FooBuilder`) for types with many optional inputs or complex validation.
- **Eager Type Signatures (Borrow vs. Generics):**
  - Prefer borrowed slices over owned containers in arguments: `&str` over `&String`, `&[T]` over `&Vec<T>`, `&Path` over `&PathBuf`.
  - Accept `impl AsRef<Path>` / `impl Into<String>` when ergonomics and zero-copy flexibility justify the generic boundary.
- **Standard Trait Implementations:**
  - Eagerly derive or implement `Debug`, `Clone`, `PartialEq`, `Eq`, `Hash`, and `Default` on public types where applicable.
  - Implement `From<T>` (which automatically provides `Into<U>`) and `TryFrom<T>` rather than ad-hoc conversion methods. Never implement `Into` or `TryInto` directly.
  - Ensure all public types are `Send` and `Sync` when safe and feasible.
- **Error Handling & Safety:**
  - Error types must implement `std::error::Error + Send + Sync + 'static`, `Display` (lowercase, concise, no trailing period), and `Debug`.
  - Never `unwrap()` or `expect()` in library or production paths unless statically infallible; propagate via `?`. If panicking is possible, document it under a `# Panics` doc header.
  - Document fallible behavior under `# Errors` and `unsafe` preconditions under `# Safety`. Every `unsafe { ... }` block requires a preceding `// SAFETY: ...` comment explaining why the invariants hold.

## 9. Idiomatic Clippy Rules (`clippy::all`, `clippy::pedantic`)
- **Attributes & Lints:**
  - Apply `#[must_use]` on pure functions, builder steps, and custom types whose return values should not be silently ignored.
  - Scope lint overrides narrowly using `#[expect(clippy::lint_name, reason = "...")]` (preferred over `#[allow(...)]` so stale suppressions warn automatically).
- **Control Flow & Pattern Matching:**
  - Use `if let` or `let ... else { return ... };` instead of single-arm `match` with a `_ => {}` wildcard (`single_match`, `manual_let_else`).
  - Collapse nested `if` statements when there is no `else` branch (`collapsible_if`), or combine using `&&` / let-chains.
  - Prefer iterator combinators (`filter`, `map`, `find`, `any`, `all`) or `.copied()`/`.cloned()` over manual indexing loops (`needless_range_loop`, `map_clone`).
  - Use `.is_empty()` instead of `.len() == 0` (`len_without_is_empty`—always pair `len()` with `is_empty()`).
- **Memory & Performance:**
  - Avoid redundant clones (`redundant_clone`, `clone_on_copy`). Pass by reference unless ownership transfer is required.
  - Prefer `format!` avoidance when a string literal or `to_owned()`/`to_string()` suffices (`useless_format`).
  - Use `Vec::with_capacity` when collection bounds are known ahead of time.
  - Box large enum variants (`large_enum_variant`) when one variant disproportionately inflates the size of the entire enum.
- **Clarity & Conciseness:**
  - Use struct field init shorthand (`Foo { a, b }` instead of `Foo { a: a, b: b }`) and elide lifetimes whenever the compiler allows (`needless_lifetimes`).
  - Omit explicit `return` at the tail of functions/blocks (`needless_return`).
  - Use `_` separator in large numeric literals (`1_000_000`) and avoid lossy `as` casts on numeric types—prefer `From::from`, `TryFrom::try_from`, or explicit methods (`cast_lossless`, `cast_possible_truncation`).
  - Inline variables directly into format strings: `println!("{foo}")` instead of `println!("{}", foo)` (`uninlined_format_args`).

## 10. Recommended `Cargo.toml` `[lints]` Configuration
Include this baseline `[lints]` block in `Cargo.toml` (or `[workspace.lints]` with `lints.workspace = true` in member crates) to enforce these rules automatically:

```toml
[lints.rust]
missing_debug_implementations = "warn"
redundant_lifetimes = "warn"
single_use_lifetimes = "warn"
unsafe_op_in_unsafe_fn = "deny"
unused_lifetimes = "warn"
unused_qualifications = "warn"

[lints.clippy]
all = { level = "warn", priority = -1 }
pedantic = { level = "warn", priority = -1 }
clone_on_ref_ptr = "warn"
dbg_macro = "warn"
expect_used = "warn"
let_underscore_must_use = "warn"
map_err_ignore = "warn"
mem_forget = "warn"
rc_buffer = "warn"
rc_mutex = "warn"
rest_pat_in_fully_bound_structs = "warn"
same_name_method = "warn"
 self_named_module_files = "warn"
semicolon_if_nothing_returned = "warn"
str_to_string = "warn"
todo = "warn"
undocumented_unsafe_blocks = "deny"
unimplemented = "warn"
unneeded_field_pattern = "warn"
unwrap_in_result = "warn"
unwrap_used = "warn"
use_debug = "warn"
verbose_file_reads = "warn"
```
