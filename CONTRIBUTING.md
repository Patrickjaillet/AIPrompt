# Contributing to AIPrompt

AIPrompt is maintained by Patrick JAILLET (SANDEFJORD DEVELOPMENT). Contributions are welcome, provided the following guidelines are respected.

## Stack

C# 12 / .NET 8 (LTS) / WPF / MaterialDesignInXamlToolkit / MVVM / SQLite + EF Core 8. See `ROADMAP.md` for the full locked stack (local file, not part of this repository) or the "Stack technique" section of `README.md`.

## Development conventions

- Source code entirely in English (identifiers, class/method/variable names).
- No comments in the source code.
- Strict Windows 10/11 (x64) compatibility only.
- MVVM architecture: no business logic in code-behind or XAML.
- Every user-visible change must be reflected in `CHANGELOG.md` (Keep a Changelog format).
- Every change to `README.md` should keep the installation instructions and screenshots up to date.

## Getting started

```
dotnet build AIPrompt.sln
dotnet test src/AIPrompt.Tests/AIPrompt.Tests.csproj
```

Local build with automatic version bump:

```
pwsh scripts/build.ps1
```

## Submitting changes

1. Fork the repository and create a feature branch.
2. Make your changes, following the conventions above.
3. Ensure `dotnet build` and the test suite pass.
4. Update `CHANGELOG.md` under `[Unreleased]`.
5. Open a pull request describing the change and its motivation.

## Reporting issues

Use the issue templates in `.github/ISSUE_TEMPLATE` to report bugs or request features.

## License

By contributing, you agree that your contributions will be licensed under the [MIT License](LICENSE).
