# Contributing

Thanks for your interest in improving UCB Hold. Contributions are welcome,
including bug fixes, new features, and documentation updates.

## Code of Conduct

Please read and follow the Code of Conduct in CODE_OF_CONDUCT.md.

## How to Start

- Check existing issues before starting work.
- For large changes, open an issue or discussion to align on scope.
- Keep changes focused and easy to review.

## Project Setup

See [Docs/SETUP.md](Docs/SETUP.md) for the full local setup and run instructions.

## Repository Structure

- Code/Client: Angular frontend
- Code/Server: ASP.NET Core backend
- DataBase: database schema and backups
- Tests: unit and integration tests
- docs: project documentation

## Branching and Commits

Recommended branch naming:

- bugfix-<issue>-short-description
- feature-<issue>-short-description
- docs-<issue>-short-description
- refactor-<issue>-short-description

If there is no issue, omit the number and keep the description short.

Commit messages should be clear and concise. Use a short summary line and add
context in the body when needed.

## Quality Checklist

Before submitting a pull request:

- Backend build:
  - cd Code/Server
  - dotnet build
- Backend tests:
  - cd Code/Tests
  - dotnet test
- Frontend build:
  - cd Code/Client
  - ng build

If you changed UI behavior, include screenshots or a short video in the PR.

## Pull Requests

- Explain what changed, why it changed, and how to test it.
- Link related issues or discussions.
- Avoid committing secrets (use dotnet user-secrets for local credentials).
- Update documentation when behavior or setup changes.

## Dependencies

Avoid adding new dependencies without discussion. If you need one, explain the
benefit, footprint, and any security impact in the PR.
