Here is the revised prompt:

---

Create a **C# Pure MudBlazor** project using **Vertical Slice Architecture** for a **Borrow and Return** system.

**Important requirements:**

* Do **not** change the topic or the ideas of the system.
* Change the enums to **Admin, Librarian, and Student**.
* Change the project name to **GlassLibraryManagement**.
* Rename every reference of **GlassLibraryManagement** to **GlassLibraryManagement**.
* Remove **Price** from `Book.cs` and remove all related references and connections.
* Change **Stock** to **Quantity** and update all related references and connections.
* Add **QRCode**, **Borrow**, and **Return** entity models in the **Data**, **Services**, and **Repositories** layers.
* QRCode must be used for uniqueness of the book:

  * **mainId** for the master record
  * **copyId** for each book quantity
  * **qrcodeId** for the PNG representation of each book copy

---

# BORROW & RETURN CORE IDEA

* **Borrow** = create a transaction record
* **Return** = update that transaction

Think of it as:

> One record = one borrow lifecycle

---

# VERTICAL SLICE (FEATURE-BASED)

Split the system into features:

* Borrow
* Return

Each feature should use:

* Data (models and enums)
* Repositories
* Services
* Helpers

---

# DATA (MODELS & ENUMS)

### Files:

* `Book`
* `User`
* `BorrowTransaction`
* `QRCode`
* `Enums/RoleType` or equivalent enum for:

  * Admin
  * Librarian
  * Student
* `Enums/TransactionStatus`
* `Enums/BookStatus`

### Purpose:

* Store system data
* Track the borrow lifecycle:

  * borrowed
  * returned
  * overdue

---

# REPOSITORY (DATA ACCESS)

### Files:

* `IBookRepository`
* `BookRepository`
* `IUserRepository`
* `UserRepository`
* `IBorrowRepository`
* `BorrowRepository`
* `IReturnRepository`
* `ReturnRepository`
* `IQRCodeRepository`
* `QRCodeRepository`

### Purpose:

* Get, save, and update data
* No business decisions

---

# SERVICE (BUSINESS LOGIC)

### Files:

* `IBorrowService`
* `BorrowService`
* `IReturnService`
* `ReturnService`
* `IQRCodeService`
* `QRCodeService`

### Borrow Logic:

* Check if the book exists
* Check availability
* Check user eligibility
* Create transaction
* Update book quantity/status
* Generate QR code for the borrowed copy

### Return Logic:

* Find the active transaction
* Validate that it was not already returned
* Check overdue status
* Update the transaction
* Update book quantity/status

---

# HELPERS (UTILITY ONLY)

### Files:

* `DateHelper`
* `FineHelper`
* `QRHelper`

### Purpose:

* Reusable utility logic only
* No business rules

---

# UI (MUD BLAZOR)

### Files:

* `BorrowPage.razor`
* `ReturnPage.razor`
* `Components/BorrowDialog`
* `Components/QRScanner`

### Flow:

* Borrow → select book → confirm → service call
* Return → select or scan QR → service call

---

# STATE FLOW

Available → Borrowed → Returned
↓
Overdue

---

# KEY RULES

### Borrow:

* Must be available
* Must pass user limits

### Return:

* Cannot return twice
* Must match the active transaction

---

# SIMPLE PROJECT STRUCTURE

```text
GlassLibraryManagement/
  Data/
    Models/
    Enums/

  Repositories/
    Interfaces/
    Implementations/

  Services/
    Interfaces/
    Implementations/

  Helpers/

  Features/
    Borrow/
    Return/

  UI/
    Pages/
    Components/
```

---

# ACCESS RIGHTS

## Admin Accessible:

### Navigation

* Home
* Dashboard

### Book Borrow and Return

* Borrow and Return page that displays the borrow and return records of students and librarians

### Management

* Users
* Books

---

## Librarian Accessible:

### Navigation

* Home
* Dashboard

### Book Borrow and Return

* Borrow and Return page that displays the borrow and return records of students and librarians

### Management

* Books

---

## Student Accessible:

### Navigation

* Home
* Dashboard

### Book Borrow and Return

* Borrow and Return page that displays the borrow and return records of students and librarians

---

# FINAL IDEA

* **Repository** → data only
* **Service** → rules and logic
* **Helper** → tools
* **UI** → interaction


