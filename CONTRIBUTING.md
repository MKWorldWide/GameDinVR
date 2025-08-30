# Contributing to GameDinVR

We're excited you're interested in contributing to GameDinVR! This document outlines the process for contributing to our project.

## 📋 Table of Contents
- [Code of Conduct](#code-of-conduct)
- [Getting Started](#-getting-started)
- [Development Workflow](#-development-workflow)
- [Pull Request Process](#-pull-request-process)
- [Coding Standards](#-coding-standards)
- [Reporting Bugs](#-reporting-bugs)
- [Feature Requests](#-feature-requests)
- [License](#-license)

## Code of Conduct

This project and everyone participating in it is governed by our [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

## 🚀 Getting Started

1. **Fork** the repository on GitHub
2. **Clone** the project to your own machine
3. **Commit** changes to your own branch
4. **Push** your work back up to your fork
5. Submit a **Pull Request** so we can review your changes

### Prerequisites

- Unity 2022.3.6f1 (LTS)
- VRChat Creator Companion (VCC)
- Node.js 20.x (for Serafina development)
- Git

## 🔄 Development Workflow

### Unity Development

1. Open the project in Unity Hub
2. Use VCC to import required SDKs:
   - VRChat Worlds SDK 3.6.0+
   - UdonSharp 1.1.0+
3. Create a new branch for your feature/fix:
   ```bash
   git checkout -b feature/your-feature-name
   ```
4. Make your changes
5. Test your changes thoroughly
6. Commit your changes with a descriptive message

### Serafina (Discord Bot) Development

1. Navigate to the `serafina` directory
2. Install dependencies:
   ```bash
   npm install
   ```
3. Copy the example environment file:
   ```bash
   cp .env.example .env
   ```
4. Update `.env` with your configuration
5. Start the development server:
   ```bash
   npm run dev
   ```

## 🔀 Pull Request Process

1. Ensure any install or build dependencies are removed before the end of the layer when doing a build.
2. Update the README.md with details of changes to the interface, this includes new environment variables, exposed ports, useful file locations, and container parameters.
3. Increase the version numbers in any examples files and the README.md to the new version that this Pull Request would represent. The versioning scheme we use is [SemVer](http://semver.org/).
4. You may merge the Pull Request in once you have the sign-off of two other developers, or if you do not have permission to do that, you may request the second reviewer to merge it for you.

## 💻 Coding Standards

### C# (Unity)
- Follow the [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
- Use PascalCase for class names and method names
- Use camelCase for local variables and parameters
- Use `_camelCase` for private fields
- Add XML documentation for public APIs

### TypeScript/JavaScript
- Follow the [TypeScript Coding Guidelines](https://github.com/Microsoft/TypeScript/wiki/Coding-guidelines)
- Use `camelCase` for variables and functions
- Use `PascalCase` for classes and interfaces
- Use `UPPER_CASE` for constants
- Add JSDoc comments for all public functions

## 🐛 Reporting Bugs

Bugs are tracked as [GitHub issues](https://guides.github.com/features/issues/). Create an issue and provide the following information:

### Unity Tests
- Place tests in the `Assets/Tests` folder
- Use Unity Test Framework for unit tests
- Run tests from the Unity Test Runner window

### Serafina Tests
- Place tests in the `src/__tests__` directory
- Use Vitest for testing
- Run tests with:
  ```bash
  npm test        # Run tests once
  npm test:watch  # Run tests in watch mode
  ```
- Generate coverage report:
  ```bash
  npm run test:coverage
  ```

## Pull Request Process

1. Ensure your code follows our coding standards
2. Update the CHANGELOG.md with your changes
3. Make sure all tests pass
4. Run linters and formatters
5. Submit your pull request with a clear description of your changes
6. Reference any related issues
7. Request reviews from relevant team members

## CI/CD Workflows

### Unity CI
- Runs on every push to `main` and pull requests
- Builds the project for Windows
- Runs static analysis
- Validates project structure

### Serafina CI
- Runs on every push to `main` and pull requests
- Installs dependencies
- Runs linters and type checking
- Runs tests and generates coverage reports

### GitHub Pages
- Deploys documentation to GitHub Pages
- Runs on push to `main` when files in `docs/` change

## Security

- Report security vulnerabilities to [security@example.com](mailto:security@example.com)
- Follow security best practices in code
- Never commit secrets or sensitive information
- Keep dependencies up to date

## Documentation

- Update relevant documentation when making changes
- Follow the [Documentation Style Guide](docs/STYLE_GUIDE.md)
- Use Markdown for all documentation
- Keep documentation up to date with code changes

## Reporting Issues

Please use our [issue tracker](https://github.com/yourusername/GameDinVR/issues) to report any bugs or suggest new features. Include:

- A clear, descriptive title
- Steps to reproduce the issue
- Expected vs. actual behavior
- Screenshots or logs if applicable
- Environment details (Unity version, OS, etc.)

## 📄 License

By contributing, you agree that your contributions will be licensed under its [MIT License](LICENSE).

## 🙏 Acknowledgments

- Thank you to all contributors who help improve this project!
- Special thanks to the VRChat community for their support and feedback.
