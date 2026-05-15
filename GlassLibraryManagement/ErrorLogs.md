Issue Summary

1. In Book Management, adding a new book with an uploaded cover image could fail.
2. The user could not clearly see the saved cover image after adding or viewing a book.
3. The user could not clearly see the generated QR codes for each book copy.
4. The browser logged a `favicon.ico` 404 error.
5. The Blazor WebSocket information message was reported together with errors even though it is normal.

Root Cause Analysis

1. `An error occurred while saving the entity changes` was caused by the inner MariaDB error:
   `Got a packet bigger than 'max_allowed_packet' bytes`
   This happened because `CoverImage` was being stored as a full base64 string from the uploaded image, and large images made the insert payload too big.

2. The QR generation service was already wired in the backend after book creation, but the UI did not visibly present the generated QR images in the book details dialog, which made it look like no QR codes were generated.

3. The cover image field existed in the model and database, but the dialogs did not clearly preview the image, so it appeared as if the book had no cover image.

4. The `favicon.ico` 404 happened because the app had `wwwroot/favicon.png`, but there was no `<link rel="icon">` entry in the HTML head. The browser then tried the default `/favicon.ico` path and failed.

5. The message
   `blazor.web.js: WebSocket connected ...`
   is normal runtime information for Blazor Server and is not an error.

Fixes Applied

1. `BookFormDialog.razor`
   - Resizes uploaded images before saving.
   - Converts uploaded covers to a smaller JPEG.
   - Limits the cover payload size to reduce MariaDB packet size issues.
   - Shows a cover preview after upload.
   - Shows a clearer error message if the cover image is still too large.

2. `BookViewDialog.razor`
   - Shows the saved cover image when viewing a book.
   - Shows the generated QR codes for each copy so QR generation is visible in the UI.
   - Displays each generated `CopyId`.

3. `App.razor`
   - Added a favicon link to `favicon.png`.
   - This removes the `favicon.ico` 404 in normal browser loading.

Current Result

1. A new book can now be added with a resized cover image without hitting the previous large-packet database error in the normal flow.
2. The book cover is now visible in the add/edit workflow and in the book details dialog.
3. Generated QR codes are now visible in the book details dialog for each book copy.
4. The favicon 404 issue is fixed by pointing the app to the existing `favicon.png`.
5. The Blazor WebSocket line is confirmed as normal informational output, not a failure.

Verification Result

1. Razor diagnostics for the edited files are clean.
2. The project build succeeds when built to an alternate output folder:
   `dotnet build -o temp-build`
3. A normal build to the default `bin` folder can still fail if the running app or Visual Studio locks the output DLL/EXE, but that is a file-lock issue, not a code issue.

Note

The requested future enhancement for QR PNG zip export, added-quantity-only export, and edit-page switch behavior is not part of this fix. The current fix resolves the logged save failure, restores visible cover handling, exposes generated QR codes in the UI, and explains the browser and console messages accurately.
