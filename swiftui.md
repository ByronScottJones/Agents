---
title: "SwiftUI"
description: "Open this file only when building or modifying SwiftUI views or SwiftUI app structure."
applies_to: ["*.swift"]
---

# SwiftUI

Open this file only when building or modifying SwiftUI views or SwiftUI app structure. Load [`swift.md`](swift.md) first; it owns language, concurrency, testing, and tooling rules. Load [`accessibility.md`](accessibility.md) for user-facing work.

## Platform Targets

- Target iOS 17 / macOS 14 or later for new apps so the Observation framework, `NavigationStack`, and current SwiftUI APIs are available. Raise the floor only when a required API needs it.
- Gate newer APIs with `if #available` or `@available` rather than raising the deployment target for one feature.
- Prefer SwiftUI for new screens. Bridge to UIKit or AppKit with `UIViewRepresentable` / `NSViewRepresentable` only for capabilities SwiftUI lacks.

## Architecture

- Keep views small and declarative. Extract a subview when `body` grows beyond what fits on one screen or mixes unrelated concerns.
- Keep business logic, networking, and persistence out of views. Put them in models or services that the view observes or calls.
- Use plain SwiftUI with `@Observable` models by default. Do not introduce a heavyweight architecture framework unless the project already uses one.
- Inject dependencies through the environment (`.environment(model)` and `@Environment(Model.self)`) or initializers. Avoid singletons accessed directly from views.
- Organize code by feature (`Features/Settings/`, `Features/Library/`), not by type (`Views/`, `ViewModels/`).

## State Management

- Use the Observation framework (`@Observable` classes) for reference-type models. Do not use `ObservableObject`, `@Published`, `@StateObject`, or `@ObservedObject` in new code that targets iOS 17 / macOS 14 or later.
- Choose the property wrapper by ownership:

  | Situation | Use |
  | --- | --- |
  | View owns a value or `@Observable` model | `@State` (mark `private`) |
  | Child reads and writes parent-owned value | `@Binding` |
  | Child needs bindings into an `@Observable` model | `@Bindable` |
  | Shared model or system value from the environment | `@Environment` |
  | Small persisted preference | `@AppStorage` / `@SceneStorage` |

- Keep a single source of truth. Never copy state into a second `@State` that must stay in sync.
- Mark `@Observable` models used by views `@MainActor`.
- Derive values in computed properties instead of storing and synchronizing them.

## Views & Layout

- Pass only the data a subview needs, not whole models, to limit invalidation.
- Use stack, `Grid`, `ViewThatFits`, and the `Layout` protocol before `GeometryReader`. Use `GeometryReader` only when a measurement is truly required.
- Use `.frame(maxWidth: .infinity)` and alignment instead of hard-coded sizes. Support Dynamic Type; never fix font point sizes for body text.
- Use semantic fonts (`.body`, `.headline`), semantic colors (`.primary`, `.secondary`, asset catalog colors), and SF Symbols so Dark Mode and accessibility settings work automatically.
- Create reusable styling with custom `ViewModifier`s, `ButtonStyle`s, and `LabelStyle`s rather than repeated modifier chains.
- Avoid `AnyView`. Use `@ViewBuilder`, generics, or `Group` to keep view types static.
- Give `ForEach` stable identity through `Identifiable` or an explicit stable `id`. Never use array indices or `UUID()` created in `body` as identity.
- Use `LazyVStack`, `LazyVGrid`, or `List` for large or unbounded collections.

## Navigation

- Use `NavigationStack` with value-based `NavigationLink(value:)` and `.navigationDestination(for:)`. Do not use the deprecated `NavigationView`.
- Use `NavigationSplitView` for multi-column layouts on iPad and macOS.
- Hold the path in a model (`NavigationPath` or a typed array) when navigation must be driven programmatically or restored.
- Present sheets and alerts from item- or Boolean-bound state (`.sheet(item:)`, `.alert(_:isPresented:)`), owned by the presenting view.

## Async Work & Side Effects

- Start async work with `.task { }` or `.task(id:)`, which cancel automatically when the view disappears or the `id` changes. Avoid `onAppear` with an unstructured `Task`.
- Do not perform side effects in `body` or in initializers. `body` may run many times.
- Represent loading with an explicit state enum (for example `idle`, `loading`, `loaded(Value)`, `failed(Error)`) and render each case.
- Use `.refreshable` for pull-to-refresh and `.searchable` for search.

## Persistence

- Use SwiftData (`@Model`, `@Query`, `.modelContainer`) for local app data in new apps on supported platforms. Use Core Data only when the project already uses it or needs a capability SwiftData lacks.
- Configure the `ModelContainer` once at the `App` or `Scene` level and inject it through the environment.
- Version SwiftData schemas with `VersionedSchema` and a `SchemaMigrationPlan` before the first release that changes a model. Follow [`database.md`](database.md) for migration discipline.

## Accessibility & Localization

- Give every interactive element without visible text an `.accessibilityLabel`. Use `.accessibilityHint`, `.accessibilityValue`, and traits where they add meaning.
- Combine related elements with `.accessibilityElement(children: .combine)`.
- Test with VoiceOver, Dynamic Type at accessibility sizes, Reduce Motion, and Increase Contrast.
- Use `String(localized:)` and `LocalizedStringKey` literals with a String Catalog (`Localizable.xcstrings`). Never concatenate localized fragments.
- Format numbers, dates, and measurements with `FormatStyle` (`.formatted()`), not manual string building.

## Previews

- Provide a `#Preview` for every non-trivial view, covering key states such as empty, loading, error, and populated.
- Feed previews with in-memory sample data or fakes, never live services. Use an in-memory `ModelContainer` for SwiftData previews.
- Keep preview-only fixtures in a `Preview Content` folder so they are excluded from release builds.

## Performance

- Use Instruments (SwiftUI template) and `Self._printChanges()` during debugging to find unnecessary view updates. Remove `_printChanges()` before committing.
- Keep `body` cheap: no formatting of large data sets, sorting, or I/O inside it.
- Use `.equatable()` or split views when a view re-renders because of unrelated state changes.
- Load images asynchronously (`AsyncImage` or a caching loader) and downsample large images before display.

## Testing

- Unit-test `@Observable` models and services with Swift Testing. Views should contain too little logic to need unit tests.
- Use XCUITest for critical user flows, locating elements by `.accessibilityIdentifier`, not by display text.
- Use snapshot testing only when the project has adopted a snapshot library.

## Code Generation Defaults

- Generate views as `struct`s conforming to `View`, with `private` `@State` properties and a `#Preview`.
- Use `@Observable` models, `NavigationStack`, and `.task` rather than their legacy equivalents.
- Include accessibility labels on icon-only controls.
