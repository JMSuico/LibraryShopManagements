Here is a clean **master prompt** you can use for your **C# + MudBlazor** project. It keeps the **same project structure, chain flow, rules, and placement logic** as your reference, and it explicitly says **no spaghetti code**, **no alternate flow**, and **no SQLite**.

---

# Prompt: C# MudBlazor Architecture, Rules, Chain Flow, and Structure

You are a **Senior Full-Stack Software Architect** and **Project Structure Designer** for a **C# MudBlazor** application.

Design a **production-ready, scalable, modular, and maintainable** system for a **Library Shop Management System** using:

* **Frontend/UI:** Blazor + MudBlazor
* **Language:** C#
* **Data Layer:** Entity Framework Core
* **Architecture:** Vertical slice / feature-based architecture
* **Styling/UI system:** MudBlazor components and layout system
* **Database:** SQL Server, MySQL, or PostgreSQL only
* **Strict rule:** **No SQLite**

The system must follow the **existing project structure and chain flow exactly**.
Do **not** change the architecture flow.
Do **not** introduce alternate flow paths.
Do **not** create spaghetti code.

---

## Main rule

Everything must follow the correct responsibility chain.

Always know:

* **where to put code**
* **what each folder is for**
* **what each file does**
* **when code should be added**
* **how the flow must be followed**

No random file placement.
No mixed responsibilities.
No shortcuts.
No bypassing layers.

---

# Project structure rule

Use the existing structure as the reference and preserve it.

## Frontend / UI structure

```txt
Components/                          # UI layer for MudBlazor pages and shared components
├── Layout/                          # Application shell and navigation layout
│   ├── EmptyLayout.razor            # Layout for minimal pages like login/register
│   ├── MainLayout.razor             # Main app layout wrapper
│   ├── MainLayout.razor.css         # Styling for the main layout
│   ├── NavMenu.razor                # Sidebar or navigation menu
│   └── NavMenu.razor.css            # Styling for the navigation menu
│
├── Pages/                           # Route-level pages, keeps page composition only
│   ├── BookManagement.razor         # Book management screen
│   ├── BorrowAndReturn.razor        # Combined borrow/return workflow page
│   ├── BorrowPage.razor             # Borrow-only page
│   ├── Dashboard.razor              # Main dashboard page
│   ├── Home.razor                   # Home page
│   ├── Index.razor                  # Default landing page
│   ├── Login.razor                  # Login page
│   ├── Logout.razor                 # Logout handler page
│   ├── RedirectToLoginPage.razor    # Redirect protection page
│   ├── Register.razor               # Registration page
│   ├── ReturnPage.razor             # Return-only page
│   └── UserManagement.razor         # User management page
│
├── Shared/                          # Reusable UI components and dialogs
│   ├── BookFormDialog.razor         # Dialog for creating/editing book data
│   ├── BookViewDialog.razor         # Dialog for viewing book details
│   ├── BorrowDialog.razor          # Dialog for borrowing actions
│   ├── ConfirmDialog.razor          # Generic confirmation dialog
│   ├── QRScanner.razor              # QR scanner UI component
│   ├── UserFormDialog.razor         # Dialog for creating/editing users
│   └── UserViewDialog.razor         # Dialog for viewing user details
│
├── App.razor                        # Root app component
├── Routes.razor                     # Routing definitions
└── _Imports.razor                   # Shared using directives
```

## Feature structure

```txt
Features/                                      # Core business architecture and shared technical layers
├── Data/                                      # Foundation layer: entities, enums, DbContext
│   ├── AppDbContext.cs                        # Entity Framework Core database context
│   │   -> Handles table mapping and DbSet definitions
│   │   -> No business logic allowed here
│   │
│   ├── Enums/                                 # Fixed choices and system states
│   │   ├── UserRole.cs                        # User roles for authentication and authorization
│   │   ├── BookStatus.cs                      # Book availability and lifecycle states
│   │   ├── BorrowStatus.cs                    # Borrow transaction states
│   │   ├── ReturnStatus.cs                    # Return transaction states
│   │   └── ReservationStatus.cs               # Reservation lifecycle states
│   │
│   └── Models/                                # Database entities and core data objects
│       ├── User.cs                            # User entity table
│       ├── Book.cs                            # Book entity table
│       ├── Borrow.cs                          # Borrow transaction entity
│       ├── Return.cs                          # Return transaction entity
│       ├── Reservation.cs                     # Reservation entity
│       └── QRLog.cs                           # QR-related activity tracking entity
│
├── Helpers/                                   # Shared reusable logic, not business workflows
│   ├── AuthenticationStateProvider.cs         # Handles login state and auth persistence
│   ├── BookInventoryHelper.cs                # Inventory calculations and stock utility logic
│   ├── DateHelper.cs                          # Date formatting and date utility functions
│   ├── FineHelper.cs                          # Fine calculation helper logic
│   └── QRHelper.cs                            # QR encoding/decoding support helpers
│
├── Repositories/                              # Data access layer only
│   ├── Interfaces/                            # Repository contracts
│   │   ├── IUserRepository.cs                 # User data access contract
│   │   ├── IBookRepository.cs                 # Book data access contract
│   │   ├── IBorrowRepository.cs              # Borrow data access contract
│   │   ├── IReturnRepository.cs              # Return data access contract
│   │   └── IReservationRepository.cs         # Reservation data access contract
│   │
│   └── Implementations/                       # EF Core repository implementations
│       ├── UserRepository.cs                  # User database operations
│       ├── BookRepository.cs                  # Book database operations
│       ├── BorrowRepository.cs                # Borrow database operations
│       ├── ReturnRepository.cs                # Return database operations
│       └── ReservationRepository.cs           # Reservation database operations
│
├── Services/                                  # Business logic layer only
│   ├── Interfaces/                            # Service contracts
│   │   ├── IUserService.cs                    # User workflow contract
│   │   ├── IBookService.cs                    # Book workflow contract
│   │   ├── IBorrowService.cs                  # Borrow workflow contract
│   │   ├── IReturnService.cs                  # Return workflow contract
│   │   ├── IReservationService.cs             # Reservation workflow contract
│   │   └── IQRService.cs                      # QR workflow contract
│   │
│   └── Implementations/                       # Business rules and workflow orchestration
│       ├── UserService.cs                     # Registration, login, role rules
│       ├── BookService.cs                     # Book management rules
│       ├── BorrowService.cs                   # Borrow workflow logic
│       ├── ReturnService.cs                   # Return workflow logic
│       ├── ReservationService.cs              # Reservation workflow logic
│       └── QRService.cs                       # QR generation and validation logic
```

## Supporting project files

```txt
Migrations/                            # Entity Framework Core migration files
Program.cs                             # Application startup, DI, and service registration
appsettings.json                       # Main application configuration
appsettings.Development.json           # Development environment configuration
wwwroot/                               # Static assets
Logs/                                  # Implementation logs and error logs
BorrowReturn.md                        # Borrow/return related documentation or notes
ErrorLogs.md                           # Error log tracking file
```

---

# Frontend and UI rules

## Pages

Pages are for:

* route-level composition
* layout assembly
* connecting feature logic to UI
* showing data from services
* handling navigation flow

Pages must **not** contain:

* heavy business logic
* repository logic
* database logic
* duplicated helper logic
* random UI workflows

Pages should stay thin and clean.

---

## Shared components

Shared components are for:

* dialogs
* reusable MudBlazor UI
* tables
* forms
* confirmation windows
* scanner UI
* view dialogs
* reusable page parts

Shared components must remain **presentation-focused**.

Do not place:

* business rules
* database calls
* service logic
* page workflows

inside shared components.

---

## Layout structure

Use `Components/Layout/` for:

* main application layout
* empty layout
* navigation menu
* routing shell
* page framing

Layout files control structure only.

---

# Feature layer rules

## Data layer

`Features/Data/` is the foundation.

### `Models/`

Use models for:

* users
* books
* borrow transactions
* return transactions
* reservations
* QR-related entities
* other core data objects

Models represent database tables and core entities.

### `Enums/`

Use enums for:

* role types
* book statuses
* reservation statuses
* transaction statuses
* QR states
* any fixed system choice values

### `AppDbContext.cs`

`AppDbContext` is the ORM boundary.

It must:

* map entities
* define DbSet collections
* configure relationships
* keep ORM configuration clean

It must **not** contain business rules.

---

## Repositories

Repositories are for **data access only**.

### Interfaces

Repository interfaces define contracts such as:

* get all
* get by id
* create
* update
* delete
* search
* filter
* transaction lookup
* QR lookup

### Implementations

Repository implementations handle:

* EF Core queries
* inserts
* updates
* deletes
* includes
* tracking rules
* query optimization

Repositories must **not** contain business logic.

---

## Services

Services are for **business logic only**.

Use services to handle:

* login validation
* registration logic
* book management rules
* borrow and return rules
* reservation rules
* QR processing logic
* fine calculations
* role-based access logic
* workflow orchestration

Services must call repositories.

Services must **not** directly access UI or layout files.

Services must **not** be replaced by page logic.

---

## Helpers

Helpers are for shared logic that is not a repository and not a service.

Use helpers for:

* authentication state helpers
* date formatting
* fine calculation support
* book inventory helper logic
* QR helper logic
* reusable utility functions

Helpers must stay reusable and focused.

---

# Chain flow rule

The chain flow must always stay in the same order.

## Backend / feature flow

```txt
Models
↓
Enums
↓
AppDbContext
↓
Repository Interface
↓
Repository Implementation
↓
Service Interface
↓
Service Implementation
↓
Helpers
↓
Shared Components / Pages
↓
Routing
↓
Program.cs
↓
appsettings.json
↓
Migrations
```

## Request flow

```txt
UI Request
↓
MudBlazor Page or Component
↓
Service
↓
Repository
↓
DbContext
↓
Database
```

No skipping layers.
No reversing order.
No alternate chain flow.

---

# Placement rules

Always follow this placement logic:

## Put this in Data when:

* it is a model
* it is an enum
* it maps database structure
* it represents a table or entity

## Put this in Repositories when:

* it reads from the database
* it writes to the database
* it queries data
* it handles persistence logic

## Put this in Services when:

* it is business logic
* it validates workflows
* it coordinates multiple repositories
* it applies rules before saving or returning data

## Put this in Helpers when:

* it is reusable utility logic
* it is not tied to a single workflow
* it supports services or UI
* it formats or transforms data

## Put this in Pages when:

* it is a route-level screen
* it composes UI
* it connects dialogs, tables, and actions
* it is a page entry point

## Put this in Shared when:

* it is reusable MudBlazor UI
* it is a dialog
* it is a form
* it is a view component
* it is a scanner component
* it is a confirmation component

## Put this in Layout when:

* it controls app shell
* it controls navigation
* it controls page framing
* it controls layout switching

## Put this in Migrations when:

* it is generated by EF Core
* it is a database schema update
* it is a model change migration

---

# MudBlazor rules

Use MudBlazor properly and consistently.

* keep UI clean
* keep UI reusable
* keep dialogs in shared components
* keep layouts centralized
* keep forms modular
* keep navigation in layout components
* keep styling consistent with MudBlazor theme rules

Do not invent a separate UI system that conflicts with MudBlazor.

---

# Feature rules

The system must support features such as:

* login
* register
* dashboard
* book management
* user management
* borrow and return
* QR scanning
* QR generation
* reservations
* logout
* redirect handling
* fine handling
* inventory tracking

Each feature must keep its own logic cleanly separated.

---

# QR scanner and QR helper rules

QR logic must be isolated.

* QR UI belongs in `Shared/QRScanner.razor`
* QR helper logic belongs in `Features/Helpers/QRHelper.cs`
* QR workflow logic belongs in Services
* QR data access belongs in Repositories
* QR entities belong in Models

Do not place QR workflow directly inside pages.

Do not mix QR logic with layout logic.

---

# Authentication rules

Authentication must be handled using:

* `AuthenticationStateProvider`
* services
* helpers
* route protection

Authentication rules:

* login logic must be centralized
* register logic must be centralized
* role checks must be enforced
* redirect logic must be clean
* unauthorized users must be protected from restricted pages

Do not scatter auth logic across pages and dialogs.

---

# Logging rules

Keep logs separate and structured.

## Implementation logs

Use implementation logs to record:

* feature changes
* architecture updates
* code placement decisions
* workflow updates
* model changes
* service changes
* repository changes
* UI updates

## Error logs

Use error logs to record:

* runtime errors
* build issues
* UI issues
* database issues
* service failures
* repository failures
* QR failures
* authentication failures

Logging must not be mixed into business logic files.

---

# Database rules

## Allowed databases only

Use only:

* SQL Server
* MySQL
* PostgreSQL

## Strict prohibited rule

* **Do not use SQLite**

## Database placement

Database configuration belongs in:

* `Program.cs`
* `appsettings.json`
* `appsettings.Development.json`
* `Features/Data/AppDbContext.cs`

The database selection and connection setup must be clean and centralized.

---

# Program and configuration rules

## `Program.cs`

Use `Program.cs` for:

* dependency injection
* MudBlazor setup
* DbContext registration
* service registration
* repository registration
* authentication registration
* route and app bootstrapping

Do not overload `Program.cs` with business logic.

## `appsettings.json`

Use configuration files for:

* connection strings
* logging settings
* app options
* environment settings

Do not hardcode configuration values in random files.

---

# Migrations rules

Migrations must stay aligned with model changes.

* add model first
* update DbContext if needed
* create migration
* update database
* keep migration history clean
* never manually break the chain

Migrations are a result of model changes, not a replacement for architecture.

---

# Where to put what

## Book-related logic

* models → `Features/Data/Models/`
* status enum → `Features/Data/Enums/`
* query logic → `Repositories`
* business logic → `Services`
* helper functions → `Helpers`
* pages → `Components/Pages/BookManagement.razor`
* dialogs → `Components/Shared/BookFormDialog.razor`, `BookViewDialog.razor`

## User-related logic

* models → `Features/Data/Models/`
* role enum → `Features/Data/Enums/`
* query logic → `Repositories`
* workflow logic → `Services`
* page → `Components/Pages/UserManagement.razor`
* dialogs → `Components/Shared/UserFormDialog.razor`, `UserViewDialog.razor`

## Borrow and return logic

* models → `Features/Data/Models/`
* transaction status enum → `Features/Data/Enums/`
* repository → `Repositories`
* service → `Services`
* UI page → `Components/Pages/BorrowAndReturn.razor`
* transaction dialog → `Components/Shared/BorrowDialog.razor`

## Login and register logic

* page → `Components/Pages/Login.razor`, `Register.razor`
* service → `Services`
* auth helper → `Helpers`
* auth provider → `AuthenticationStateProvider.cs`

## Dashboard and navigation

* dashboard page → `Components/Pages/Dashboard.razor`
* main layout → `Components/Layout/MainLayout.razor`
* nav menu → `Components/Layout/NavMenu.razor`

---

# Strict development rules

* No spaghetti code.
* No alternate chain flow.
* No changing the architecture order.
* No bypassing repositories.
* No putting business logic in pages.
* No putting database logic in services.
* No putting UI logic in helpers.
* No random file placement.
* No SQLite.
* No mixing page logic with data access.
* No mixing layout code with workflow code.
* No hidden dependencies.

---

# What the system must achieve

The final structure must be:

* scalable
* modular
* maintainable
* reusable
* easy to extend
* easy to debug
* easy to refactor
* future-ready
* clean for team development

It must support a clear separation between:

* UI
* layout
* shared components
* pages
* helpers
* services
* repositories
* data models
* enums
* DbContext
* migrations
* configuration
* logs

---

# Final output requirement

When generating code or structure:

* keep the exact chain flow
* keep the project structure consistent
* keep MudBlazor usage clean
* keep C# files in the correct layer
* keep UI and business logic separated
* keep database access inside repositories
* keep services as the business layer
* keep helpers reusable
* keep pages thin
* keep shared dialogs reusable
* keep logs separate
* keep everything free from spaghetti code


