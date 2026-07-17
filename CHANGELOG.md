# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.9.5] - 2026-07-17

### Added

- `AboutView`: application name, dynamically-read version, copyright, creator, and a placeholder logo icon (final artwork lands in Phase 10)
- Clickable links for the contact email (`mailto:`), website, and GitHub repository, opened via the default OS handler
- MIT license link opening the `LICENSE` file, now copied to the build output as `LICENSE.txt`
- `<Version>` set on `AIPrompt.App.csproj`, read at runtime through assembly reflection rather than hardcoded

## [0.9.0] - 2026-07-17

### Added

- Theme selector (Light / Dark / System) in Settings, replacing the old ad-hoc toggle button, applied instantly and persisted
- Accent color picker over the full Material Design swatch palette (10 colors), applied instantly and persisted
- Interface language selector (FR / EN) backed by `.resx` resource files and `ILanguageService`, with a restart notice on change
- Separate default export folders for prompts and roadmaps, plus a configurable backup folder, all wired into the corresponding export/backup flows
- Database reset action in a "danger zone", requiring two sequential confirmations before wiping the library via `IBackupService.ResetDatabaseAsync`

### Changed

- Removed the standalone "Basculer thème clair / sombre" button from the navigation rail; theme is now controlled exclusively from Settings, avoiding an unpersisted duplicate control

## [0.8.0] - 2026-07-17

### Added

- `ImportExportView`: full library export to `.json` (categories, genres, terms, templates, saved prompts, roadmap projects) and import from a `.json` file
- Import mode choice (`ImportModeWindow`): merge (skip existing items by name/title) or overwrite (replace the entire library), backed by `IBackupService`
- Periodic automatic backup (`IAutoBackupService`), configurable on/off and by interval in Import/Export, writing timestamped snapshots to `%AppData%/AIPrompt/backups` and pruning old files beyond the last 10
- Restore from any listed automatic backup, with a confirmation dialog before overwriting the current library
- `IBackupService` / `BackupService` covered by unit tests for export content and both merge/overwrite import paths

## [0.7.0] - 2026-07-17

### Added

- `RoadmapGeneratorView`: project name/description fields, dynamically orderable phases and tasks with checkbox rendering
- Real-time Markdown preview of the generated `ROADMAP.md` (headings, blockquote description, checkbox task lists)
- Reuse of the term library to pre-fill roadmap tasks from existing phrases
- Direct export to a `ROADMAP.md` file via the native `SaveFileDialog`
- Reusable roadmap structures: saving and reloading a `RoadmapProject` as a template, mirroring the prompt builder's template pattern
- `Description` field added to `RoadmapProject` (migration `AddRoadmapProjectDescription`)
- `IRoadmapProjectRepository` and its EF Core implementation, covered by unit tests

## [0.6.0] - 2026-07-17

### Added

- `SavedPromptsView`: searchable, sortable (date/title/category) list of saved prompts with per-row actions
- In-place editing of a saved prompt via `SavedPromptEditorWindow`
- Duplication of an existing saved prompt
- Deletion with a Material Design confirmation dialog (`ConfirmationWindow`)
- Unitary export to `.md` (heading + content) and `.txt` (Markdown stripped to plain text via Markdig), through the native `SaveFileDialog`
- Bulk export of multi-selected prompts to a chosen folder via `Microsoft.Win32.OpenFolderDialog`
- Default export folder setting, persisted to `%AppData%/AIPrompt/settings.json` via `ISettingsService`, with a first pass at the Settings screen
- `PromptExportService` covered by unit tests for Markdown/plain-text conversion
- Extended `ISavedPromptRepository` with `GetByIdAsync`, `UpdateAsync`, and `DeleteAsync`, all covered by unit tests

## [0.5.0] - 2026-07-17

### Added

- `PromptBuilderView`: two-panel prompt assembler with a filterable library on the left and an ordered assembly panel on the right
- Drag-and-drop block reordering and library-to-assembly drops via `gong-wpf-dragdrop`, plus up/down buttons as an accessible alternative
- Free-text blocks editable inline alongside library-sourced blocks
- Real-time Markdown preview of the assembled prompt, validated through Markdig
- Template save/reload (`PromptTemplate` + `PromptBlock`) so assemblies can be reused across sessions
- "Générer le prompt final" action creating a `SavedPrompt` from the assembled content
- `IPromptTemplateRepository`, `ISavedPromptRepository`, and their EF Core implementations, covered by unit tests

### Fixed

- `GongSolutions.WPF.DragDrop` referenced in `ROADMAP.md`/dependency list was not a real NuGet package; corrected to `gong-wpf-dragdrop`, its actual package ID

## [0.4.0] - 2026-07-17

### Added

- `TermLibraryView` with an instant-search DataGrid, category/genre/language filters, and per-row actions (use, edit, delete)
- `TermEditorWindow` modal dialog for creating and editing terms, with an editable tag combo box for lightweight autocompletion
- `CategoryManagerWindow` / `GenreManagerWindow` modals for full CRUD over categories and genres
- `IPromptCategoryRepository`, `IPromptGenreRepository`, and their EF Core implementations, covered by unit tests
- `ITermPhraseRepository.IncrementUsageAsync`, wired to a "use" action that increments `UsageCount`
- `IDialogService` abstraction so view models can open modal windows without depending on WPF directly

### Fixed

- Category/genre/language filter combo boxes resetting their selection to `null` after the underlying `ObservableCollection` was cleared and repopulated, which silently filtered out every term in the library

## [0.3.0] - 2026-07-17

### Added

- `App.xaml` MaterialDesignThemes integration (`BundledTheme` + `MaterialDesign3.Defaults`) with light/dark theme support
- `MainWindow` shell with a left-hand navigation rail covering all 8 functional modules and Material icons
- Dependency injection container (`Microsoft.Extensions.DependencyInjection`) wiring the database, repositories, and view models
- `ViewModelBase` and a `MainViewModel` driving navigation between placeholder views for each module
- `ThemeService` toggling the light/dark base theme at runtime

## [0.2.0] - 2026-07-17

### Added

- EF Core entities: `PromptCategory`, `PromptGenre`, `TermPhrase`, `PromptTemplate`, `PromptBlock`, `SavedPrompt`, `RoadmapProject`, `RoadmapPhase`, `RoadmapTask`
- `AIPromptDbContext` with relationship configuration
- Initial EF Core migration (`InitialCreate`)
- `IDatabaseInitializerService` / `DatabaseInitializerService`: creates and migrates the SQLite database in `%AppData%/AIPrompt/aiprompt.db` on first launch
- Initial seed data (categories, genres, sample term phrases)
- Serilog file logging in `%AppData%/AIPrompt/logs`
- `ITermPhraseRepository` / `TermPhraseRepository` with unit tests covering CRUD operations

## [0.1.0] - 2026-07-17

### Added

- Initial solution structure with four projects: `AIPrompt.App`, `AIPrompt.Core`, `AIPrompt.Data`, `AIPrompt.Tests`
- `Directory.Build.props` (nullable enable, langVersion latest, net8.0-windows)
- Base NuGet packages: `CommunityToolkit.Mvvm`, `MaterialDesignThemes`, `MaterialDesignColors`, `Microsoft.Extensions.DependencyInjection`
- `LICENSE` (MIT), `README.md`, `CHANGELOG.md`
- `.gitignore` with explicit exclusion of `ROADMAP.md`
