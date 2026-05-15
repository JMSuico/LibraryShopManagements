
**Prompt:**

Update the library management system with the following changes while strictly following the chain flow in:

`C:\Users\provu\Desktop\Claude zip file\PROJECT\LibraryShopManagement\Chain Flow.md`

Do not create any new files or modules that conflict with the existing chain flow. If new modules or files are needed, follow the same structure and naming style already defined in the project.

### Required Changes

1. **Remove QR Code Borrow and Return Transaction**

   * Remove the QR code-based borrow and return flow.
   * Replace it with a simple borrow and return transaction system.
   * The overdue time should be based on **1 day duration**.
   * After the due date, the book should be marked as overdue.

2. **Create a new module: `Book Catalog`**

   * Display all books in the system.
   * Each book must have:

     * **View icon button**
     * **Reserve button**

3. **Create a new module: `Reservation`**

   * Display all users who reserved books.
   * Show the books they reserved and their reservation status.

4. **Create a new module: `My Borrowings`**

   * Display all books that are:

     * overdue
     * successfully borrowed
     * successfully returned

5. **Update the `Borrow` module**

   * Change it into **Borrow Logs Record**.
   * It must show the transaction history of borrowed books.
   * It must also show the users who borrowed the books.

6. **Update the `Return` module**

   * Change it into **Return Logs Record**.
   * It must show the transaction history of returned books.
   * It must also show the users who returned the books.

7. **Borrow and Return Module**

   * This module should be the main transaction flow.
   * Use the basic process:

     * select a book to borrow
     * process the borrow transaction
     * process the return transaction
   * Make it simple and user-friendly.

8. **Student Role**

   * Add access to the **Book Catalog** module.
   * Students can:

     * view all books
     * click the view icon button
     * reserve a book

### Accessible Modules by Role

**Admin Accessible**

* Home
* Dashboard
* Book Catalog
* Reservation
* My Borrowings
* Borrow Logs
* Return Logs
* Borrow and Return
* Users
* Books

**Librarian Accessible**

* Home
* Dashboard
* Book Catalog
* Reservation
* My Borrowings
* Borrow Logs
* Return Logs
* Borrow and Return
* Books

**Student Accessible**

* Home
* Dashboard
* Book Catalog
* Reservation
* My Borrowings

### Main Flow

* A student reserves a book from the **Book Catalog**.
* The librarian or admin validates the reservation by verifying the user.
* After validation, the book can be borrowed and later returned through the normal transaction flow.

### Notes

* Keep the UI clean and consistent with the existing system.
* Do not break the current structure.
* Follow the chain flow carefully before adding or changing any component, page, route, or file.

---
