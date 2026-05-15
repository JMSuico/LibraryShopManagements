﻿# Glass Library Management System - Comprehensive Architecture and Code Explanation

## Overview
This document serves as the ultimate architectural guide and detailed code explanation for the **Glass Library Management System**. 

The system follows a strict **Vertical Slice / Feature-based Architecture** combined with a layered approach (Models -> DbContext -> Repositories -> Services -> UI). It ensures a rigid separation of concerns, preventing "spaghetti code" and ensuring maintainability.

### Chain Flow & Request Flow
Based on the Chain Flow.md guidelines, the application strictly adheres to the following **Request Flow**:
UI Request -> MudBlazor Page/Component -> Service -> Repository -> DbContext -> Database

And the **Backend Flow**:
Models -> Enums -> AppDbContext -> Repository Interface -> Repository Implementation -> Service Interface -> Service Implementation -> Helpers -> Shared Components / Pages -> Routing -> Program.cs -> appsettings.json -> Migrations

---

## 1. Application Startup & Configuration

### Program.cs
**Purpose:** The application entry point and dependency injection (DI) container.
**Connections:** Depends on AppDbContext, all Interfaces, and Implementations. It defines the middleware pipeline.
**How it Functions:** Bootstraps the application, registers MudBlazor services, configures EF Core with MySQL, registers all Repositories and Services, and sets up Cookie Authentication.

**Code Block & Syntax:**
`csharp
var builder = WebApplication.CreateBuilder(args);

// Registers Blazor components and MudBlazor UI services
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddMudServices();

// EF Core Configuration with MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

// Dependency Injection: Scoped Repositories and Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Authentication setup
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => { options.LoginPath = "/login"; });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
`
* **Syntax Explanation:** Uses top-level statements introduced in C# 9+. uilder.Services.AddScoped<I, T>() registers dependencies so that whenever an interface I is requested, an instance of T is provided per HTTP request. AddDbContext injects the MySQL database context.

---

## 2. Data Layer: Models and Enums (Features/Data/)

### Models/Book.cs (Representative Model)
**Purpose:** Plain C# object representing the Books database table.
**Connections:** Mapped by AppDbContext. Used by IBookRepository and IBookService.
**How it Functions:** Defines the properties and relationships of a Book entity. It contains NO business logic.

**Code Block & Syntax:**
`csharp
using System.ComponentModel.DataAnnotations.Schema;
using GlassLibraryManagement.Features.Data.Enums;

namespace GlassLibraryManagement.Features.Data.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string MainId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public BookStatus Status { get; set; } = BookStatus.Available;
        
        [NotMapped]
        public int Quantity { get => AvailableCopies; set => AvailableCopies = value; }

        // Navigation Properties
        public ICollection<BorrowTransaction> BorrowTransactions { get; set; } = [];
    }
}
`
* **Syntax Explanation:** public int Id { get; set; } defines a property. [NotMapped] is a Data Annotation attribute telling EF Core NOT to create a column for Quantity in the database, as it's a computed wrapper for AvailableCopies. ICollection<T> defines a one-to-many relationship.

### Enums/BookStatus.cs (Representative Enum)
**Purpose:** Strongly-typed constants for system states.
**Connections:** Used in Book.cs and BookInventoryHelper.cs.
**How it Functions:** Replaces magic strings with typed enum values.

**Code Block & Syntax:**
`csharp
namespace GlassLibraryManagement.Features.Data.Enums
{
    public enum BookStatus
    {
        Available,
        Borrowed,
        Reserved,
        Overdue,
        Returned
    }
}
`
* **Syntax Explanation:** enum defines a set of named integral constants. It ensures type safety when assigning statuses.

---

## 3. Data Context (Features/Data/AppDbContext.cs)

### AppDbContext.cs
**Purpose:** The Entity Framework Core boundary.
**Connections:** Connects Models to the SQL provider. Injected into Repositories.
**How it Functions:** Maps C# entity models to MySQL tables. Configures constraints, indexes, and relationships via the Fluent API.

**Code Block & Syntax:**
`csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Book>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.MainId).IsRequired().HasMaxLength(80);
            e.HasIndex(b => b.MainId).IsUnique();
        });
    }
}
`
* **Syntax Explanation:** DbSet<Book> represents the table. = null!; tells the compiler this will be initialized by EF Core. OnModelCreating uses the Fluent API (e.HasKey, e.HasIndex) to define primary keys, unique constraints, and max lengths, overriding default EF Core conventions.

---

## 4. Repositories (Features/Repositories/)

### BookRepository.cs
**Purpose:** The exclusive data access layer for Books.
**Connections:** Implements IBookRepository. Injects AppDbContext. Called ONLY by BookService.
**How it Functions:** Abstracts EF Core queries. No business decisions are made here. Only fetches, adds, updates, and deletes.

**Code Block & Syntax:**
`csharp
public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context) { _context = context; }

    public async Task<IEnumerable<Book>> GetAllAsync()
        => await _context.Books.AsNoTracking().ToListAsync();

    public async Task UpdateAsync(Book book)
    {
        var existing = await _context.Books.FindAsync(book.Id);
        if (existing is null) return;
        
        existing.Title = book.Title;
        await _context.SaveChangesAsync();
    }
}
`
* **Syntax Explanation:** sync Task<T> enables asynchronous non-blocking database calls. .AsNoTracking() is used for read-only queries to improve performance since EF Core doesn't need to track state changes. .SaveChangesAsync() commits the tracked changes to the database.

---

## 5. Services (Features/Services/)

### ReservationService.cs
**Purpose:** The core business logic layer for Reservations.
**Connections:** Implements IReservationService. Injects IReservationRepository, IBookRepository, etc. Called by UI Pages (BookCatalog.razor).
**How it Functions:** Orchestrates workflows. Validates constraints (e.g., checking if the user exceeds their borrow limit or reserving more copies than physically exist).

**Code Block & Syntax:**
`csharp
public async Task ReserveAsync(int bookId, int userId, int quantity)
{
    var book = await _bookRepository.GetByIdAsync(bookId) ?? throw new InvalidOperationException("Book not found.");
    var existing = await _reservationRepository.GetActiveByBookAndUserAsync(bookId, userId);
    
    // Business Logic: Limit validation
    var maxForThisBook = book.TotalCopies;
    if (quantity > maxForThisBook)
        throw new InvalidOperationException($"You can reserve at most {maxForThisBook} copies.");

    if (existing != null)
    {
        existing.Quantity = quantity;
        await _reservationRepository.UpdateAsync(existing);
    }
    else
    {
        var reservation = new Reservation { BookId = bookId, UserId = userId, Quantity = quantity };
        await _reservationRepository.AddAsync(reservation);
    }
    
    // Update book inventory using Helper
    BookInventoryHelper.Normalize(book);
    await _bookRepository.UpdateAsync(book);
}
`
* **Syntax Explanation:** Throws InvalidOperationException to enforce business rules. It retrieves data via repositories, applies conditional logic (if (quantity > max)), constructs new entities, and passes them back to the repository to be saved.

---

## 6. Helpers (Features/Helpers/)

### BookInventoryHelper.cs
**Purpose:** Pure, stateless, reusable utility methods for inventory math.
**Connections:** Called by BookService, BorrowService, and ReservationService.
**How it Functions:** Ensures that Borrowed, Reserved, and Available copies accurately add up to TotalCopies without touching the database directly.

**Code Block & Syntax:**
`csharp
public static class BookInventoryHelper
{
    public static void Normalize(Book book)
    {
        book.TotalCopies = Math.Max(0, book.TotalCopies);
        book.AvailableCopies = Math.Max(0, book.TotalCopies - book.BorrowedCopies - book.ReservedCopies);
        book.Status = book.AvailableCopies > 0 ? BookStatus.Available : BookStatus.Borrowed;
    }
}
`
* **Syntax Explanation:** public static class means it cannot be instantiated. Math.Max(0, val) ensures no negative numbers exist in inventory. It modifies the object by reference.

---

## 7. UI: Pages (Components/Pages/)

### Dashboard.razor
**Purpose:** The main statistical overview page.
**Connections:** Injects IUserService, IBookService, IBorrowService. 
**How it Functions:** Fetches raw data from services on initialization and dynamically builds a responsive bar chart based on selected metrics and timeframes. The chart re-aggregates data automatically when the user changes the metric dropdown or time range tab.

**Code Block & Syntax:**
```razor
@page "/dashboard"
@attribute [Authorize]
@inject IBookService BookService

<MudSelect T="string" @bind-Value="_selectedLegend" Variant="Variant.Outlined">
    <MudSelectItem Value="@("All Legends")">All Legends</MudSelectItem>
    <MudSelectItem Value="@("Books Added")">Books Added</MudSelectItem>
</MudSelect>

<MudTabs ActivePanelIndexChanged="OnTabChanged">
    <MudTabPanel Text="All Time" />
    <MudTabPanel Text="Yearly" />
    <MudTabPanel Text="Monthly" />
    <MudTabPanel Text="Weekly" />
    <MudTabPanel Text="Today" />
</MudTabs>

<MudChart ChartType="ChartType.Bar" ChartSeries="@_chartSeries" XAxisLabels="@_chartLabels" Width="100%" Height="300px" />

@code {
    private void UpdateBarChart()
    {
        // 1. Define labels based on active tab (e.g., Monthly -> Jan to Dec)
        // 2. Determine which metrics to compute (All Legends or single selected)
        // 3. Loop through labels and compute counts based on the time range
        // 4. Update _chartSeries to trigger a re-render
    }

    private double ComputeMetricForLabel(string metric, int tabIndex, int labelIndex, DateTime localNow)
    {
        // Computes data dynamically using _allBooks, _allTransactions, _allUsers
        // Example: count records matching the metric within the calculated date range
    }
}
```
* **Implementation Logic:**
  * **Metric Dropdown:** Allows filtering by "All Legends", "Books Added", "Overdue", "Returns", and "Active Members".
  * **Time Range Tabs:** Dynamically adjusts the X-axis labels to represent "All Time", "Yearly", "Monthly", "Weekly", or "Today".
  * **Data Aggregation:** The `UpdateBarChart()` and `ComputeMetricForLabel()` methods calculate precise start and end dates for each label, then aggregate data points (e.g., counting `CreatedAt` for books or `ReturnedAt` for transactions) instantly when controls are changed.

---

## 8. UI: Shared Components (Components/Shared/)

### BookCatalogReserveDialog.razor
**Purpose:** A reusable popup dialog for reserving books.
**Connections:** Invoked by BookCatalog.razor via IDialogService. Injects IReservationService.
**How it Functions:** Provides a slider/UI for selecting quantity. Validates input against maximum allowed limits before calling the Service to submit.

**Code Block & Syntax:**
`
azor
<MudDialog>
    <DialogContent>
        <MudSlider T="int" @bind-Value="_quantity" Min="1" Max="@_maxQuantity" />
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="SubmitAsync">Confirm</MudButton>
    </DialogActions>
</MudDialog>

@code {
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public Book Book { get; set; } = default!;

    private async Task SubmitAsync()
    {
        await ReservationService.ReserveAsync(Book.Id, UserId, _quantity);
        MudDialog.Close(DialogResult.Ok(_quantity));
    }
}
`
* **Syntax Explanation:** [Parameter] marks properties that receive data from the parent component. @bind-Value creates a two-way binding between the UI slider and the _quantity C# variable. MudDialog.Close() returns the result to the caller.

---

## 9. UI: Layouts (Components/Layout/)

### MainLayout.razor
**Purpose:** The application shell defining the global theme and navigation structure.
**Connections:** Wraps all @page components. Uses MudThemeProvider.
**How it Functions:** Acts as the single source of truth for colors and Dark/Light mode toggling.

**Code Block & Syntax:**
`
azor
@inherits LayoutComponentBase
<MudThemeProvider Theme="@_theme" IsDarkMode="@ThemeService.IsDarkMode" />

<MudLayout>
    <MudAppBar>Library Shop</MudAppBar>
    <MudDrawer><NavMenu /></MudDrawer>
    <MudMainContent>
        @Body
    </MudMainContent>
</MudLayout>

@code {
    private readonly MudTheme _theme = new() { PaletteLight = new PaletteLight { Primary = "#8B5CF6" } };
}
`
* **Syntax Explanation:** @inherits LayoutComponentBase designates this as a layout. @Body renders the active page content inside the main content area. MudTheme defines the global color palette applied to all MudBlazor components automatically.
