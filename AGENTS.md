# AGENTS.md

## Repository Overview

School coursework repository (SWP 3. AHWII) containing date-prefixed project
directories. Each directory is a **standalone project** -- all build, test, and
run commands must be executed from within the specific project directory.

Two tech stacks are used:
- **C# / .NET 9.0** -- Console applications with xUnit tests
- **TypeScript / Deno** -- Scripts using Prisma ORM with SQLite

## Project Structure

| Directory | Stack | Description |
|---|---|---|
| `2025_09_03_introC#/` | C# | Fraction arithmetic (intro) |
| `2025_09_10_introC#Erweiterung/` | C# | Extended fraction project (contains nested project) |
| `2025_09_10_introC#Erweiterung.Tests/` | C# (xUnit) | Unit tests for the above |
| `2025_09_24_ts_prisma/` | Deno + Prisma | TypeScript Prisma intro |
| `2025_10_08_deno_prisma/` | Deno + Prisma | Deno Prisma project |
| `2025_10_15_prisma_loginssytem/` | Deno + Prisma | Login system with bcrypt |
| `2025_11_05_Brueche/` | C# | Fractions (German naming) |
| `2025_11_12_Brueche_Debug/` | C# | Fractions with debugging |
| `2026_01_14_CSV_HUE_Nachholen/` | C# | CSV reader with CsvHelper |

## Build, Run, and Test Commands

### C# Projects (.NET 9.0)

All commands run from the project directory containing the `.csproj` file.

```bash
# Build
dotnet build

# Run
dotnet run
dotnet run -- "1/2" "1/3"          # Pass CLI arguments

# Run all tests in a test project
dotnet test

# Run a single test by method name
dotnet test --filter "FullyQualifiedName~TestMethodName"

# Run a single test by display name
dotnet test --filter "DisplayName~some description"

# Example: run the fraction addition test
cd 2025_09_10_introC#Erweiterung.Tests
dotnet test --filter "FullyQualifiedName~AddTest"
```

Test framework: **xUnit v2.9.2** with `Microsoft.NET.Test.Sdk` and `coverlet.collector`.
Test project: `2025_09_10_introC#Erweiterung.Tests/` (references `2025_09_03_introC#/SWP3AHWIIx.csproj`).
Test files use `[Fact]` attributes.

### Deno / TypeScript Projects

All commands run from the project directory containing `deno.json`.

```bash
# Run the main entry point
deno run -A main.ts

# Run in watch mode
deno task dev

# Run all tests (files matching *_test.ts)
deno test -A

# Run a single test by name filter
deno test -A --filter "test name substring"

# Prisma commands
deno task prisma migrate dev       # Run migrations
deno task prisma db push           # Push schema to DB
deno task prisma generate          # Generate Prisma client
deno task prisma studio            # Open Prisma Studio

# Seed the database
deno task seed                     # or: deno task prisma:seed
```

Test files follow the Deno convention: `main_test.ts` (suffix `_test.ts`).
Assertions use `assertEquals` from `@std/assert` (JSR).

## Formatting

### C# -- CSharpier

CSharpier is configured as a dotnet local tool (v1.1.2) in some projects.

```bash
# Install (if .config/dotnet-tools.json exists)
dotnet tool restore

# Format
dotnet csharpier .
```

### TypeScript -- deno fmt

Formatting is configured in each project's `deno.json`:

```bash
deno fmt              # Format all files
deno fmt --check      # Check without modifying
```

Settings (from `deno.json`):
- Indent: **4 spaces**
- Line width: **120** (or 130 in older projects)
- Quotes: **double quotes** (`singleQuote: false`)

## Code Style Guidelines

### C# Conventions

**Naming (follow standard C# conventions):**
- Classes/Structs: `PascalCase` -- `Fraction`, `Program`, `Person`
- Public methods: `PascalCase` -- `Simplify()`, `ParseMixedFraction()`
- Private fields: `camelCase` or `_camelCase` -- `numerator`, `_denominator`
- Local variables / parameters: `camelCase` -- `whole`, `remainder`
- Constants: `PascalCase`
- Note: Some older projects use German camelCase method names (`addiere()`, `kgv()`). New code should use PascalCase.

**Formatting:**
- Allman brace style (opening brace on its own line)
- 4-space indentation
- Semicolons always required

**Imports:**
- `ImplicitUsings` is enabled in all `.csproj` files, so most system usings are automatic
- When explicit `using` statements are needed: `System.*` first, then third-party packages
- Remove unused imports

**Types:**
- `<Nullable>enable</Nullable>` is active -- use `?` for nullable reference types
- Use `var` when the type is obvious from the right-hand side; use explicit types otherwise
- Prefer structs for small immutable value types (see `Fraction`)

**Error handling:**
- Validate inputs early and throw descriptive exceptions
- Wrap `Main()` body in `try/catch(Exception e)` for user-facing error messages
- Use `Environment.Exit(1)` for error exits
- Use specific exception types when appropriate (e.g., `DivideByZeroException`)

**File organization:**
- `Program.cs` as entry point with `Main()` method
- Separate domain classes into their own files (e.g., `Bruch.cs`, `Person.cs`)
- One class per file
- Use namespaces for test projects and larger applications

### TypeScript / Deno Conventions

**Naming:**
- Functions: `camelCase` -- `add()`, `seed()`
- Variables: `camelCase` -- `const prisma = new PrismaClient()`
- Data arrays: `snake_case` is used in existing code (`seed_users`, `seed_posts`)
- Files: `snake_case` -- `main.ts`, `seed.ts`, `main_test.ts`

**Imports:**
- Use relative paths with explicit `.ts` extensions: `import { X } from "./module.ts";`
- Standard library via JSR: `import { assertEquals } from "@std/assert";`
- npm packages via `npm:` specifier or import map in `deno.json`
- Order: standard library / third-party first, then local modules

**Module pattern:**
- Use `import.meta.main` guard for files that are both importable and runnable:
  ```ts
  if (import.meta.main) {
      await seed();
  }
  ```

**Types:**
- Add explicit type annotations on function parameters and return types
- Use TypeScript strict mode (Deno default)

**Prisma:**
- Model names in schema: lowercase (`user`, `post`)
- IDs: `@id @default(uuid())` or `@default(cuid())`
- Database: SQLite (`provider = "sqlite"`)
- Client import: `import { PrismaClient } from "./prisma/client/client.ts";`

## General Patterns

- **No CI/CD** -- this is a local school repository
- **No DI framework** -- direct instantiation throughout
- **Logging** -- `Console.WriteLine` (C#) / `console.log` (TS) only; no structured logging
- **Comments** -- sparse; add comments for non-obvious algorithms
- **`.gitignore`** -- excludes `bin/`, `obj/`, `prisma/client/`, `dev.db`, `.env`
- **Secrets** -- `.env` files are gitignored; never commit database URLs or credentials
- **New projects** -- follow the date-prefix naming pattern: `YYYY_MM_DD_description/`
