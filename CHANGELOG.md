# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
