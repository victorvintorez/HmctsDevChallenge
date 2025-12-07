# HMCTS DTS Junior Developer Challenge
### [Original Repo](https://github.com/hmcts/dts-developer-challenge-junior)

### Live Version available at [hmcts.projects.vintorez.dev](https://hmcts.projects.vintorez.dev)
- API Documentation available at [/api/docs/v1](https://hmcts.projects.vintorez.dev/api/docs/v1)
- OpenAPI Schema available at [/api/openapi/v1.json](https://hmcts.projects.vintorez.dev/api/openapi/v1.json)

## Introduction
This project is mostly based on the tech stack that I use day-to-day in my job, with some modifications. I mostly make ASP.NET Web API backends with React Single Page App Frontends. Most of the "infrastructure" parts of this codebase (e.g. the non-scenario based parts)are pulled from current projects I'm working on both in my current job and personal projects. All of the scenario-based elements including endpoints, models, queries and hooks, and UI were all completed within around 2.5 hours (however this was not timed).

### Backend
- Language: C#
- Framework: ASP.NET 10
- Database: PostgreSQL
- ORM: Entity Framework Core
- Testing Framework: xUnit

### Frontend
- Language: TypeScript
- Framework: React
- Builder/Bundler: Vite
- Router: TanStack Router
- UI Framework: [GovUK React Library](https://github.com/govuk-react/govuk-react)
  - Note: This library seems to be unmaintained, and [NotGovUK](https://not-gov.uk/), another React GovUK library which is maintained, was too inflexible to be used with TanStack Router

## Get Started (Local Development)
### Requirements
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) and [ASP.NET Core 10 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Node.JS v24+](https://nodejs.org/en/download) and [PNPM](https://pnpm.io/installation)
- [Podman](https://podman.io/docs/installation) or [Docker](https://docs.docker.com/engine/install/)

### Setup
1. Clone the repository
2. Create the `.env` file (optional: edit the default values)
```shell
cp .env.example .env
```
3. Create the `appsettings.Development.json` config file (if you edited the default values in the .env file, make sure to update the connection string)
```shell
cp HmctsDevChallenge.Backend/appsettings.json HmctsDevChallenge.Backend/appsettings.Development.json
```
4. Restore Frontend Dependencies
```shell
cd HmctsDevChallenge.Frontend
pnpm install --frozen-lockfile
cd ..
```
5. Restore Backend Dependencies
```shell
cd HmctsDevChallenge.Backend
dotnet restore
```
6. Run (If you use a JetBrains IDE, just use the run configuration labelled `HmctsDevChallenge.Backend: https`)
```shell
# Podman
podman compose -f compose.dev.yaml up -d && dotnet run --launch-profile https
# Docker
docker compose -f compose.dev.yaml up -d && dotnet run --launch-profile https
```
7. Open in browser
   - API Documentation: [localhost:8081/api/docs/v1](https://localhost:8081/api/docs/v1)
   - OpenAPI Schema: [localhost:8081/api/openapi/v1.json](https://localhost:8081/api/openapi/v1.json)
   - Frontend: [localhost:8082](https://localhost:8082)

### Running Tests
1. Restore Test Project Dependencies
```shell
cd HmctsDevChallenge.Backend.Test
dotnet restore
```
2. Run Tests
```shell
dotnet test
```

## Notes
- Compatibility with Linux environments is guaranteed, other platforms may function correctly but have not been actively tested.
- While the GovUK Design System has a [Date Input Component](https://design-system.service.gov.uk/components/date-input/), it doesn't have a Time Input Component, after doing some research I found this [Time Picker Github Issue](https://github.com/alphagov/govuk-design-system-backlog/issues/173) which showed a couple of options for accessible Time Picker Components. I decided to go with an hours input, minutes input, and am/pm select component as I felt it was most appropriate and although the [Select Component isn't reccommended](https://design-system.service.gov.uk/components/select/), it made validation much easier while still being relatively accessible.

## Future Improvements
- Follow proper CRUD guidelines:
  - Make `POST /api/task` (`CreateTask`) return 201 Created with the url/id of the created task
  - Make a `GET /api/task` (`ReadTask`) endpoint
  - Update the frontend to fetch the created task on `/task` instead of reading it from the url search params
- Update the frontend to provide some pre-defined status types
- Improve validation on the frontend (especially for the date+time fields)
- Implement testing on the frontend and improve testing on the backend
