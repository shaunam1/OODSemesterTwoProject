# My Little Library
## Description
My Little Library is an application for book lovers.  
The user can log in with a username and password.
On the Home tab the user will find the books available for purchase.
These books can be filtered by author.
The user can also search for books.
Clicking on a book will display information about that book and allow the user to give it a star rating, add it to cart, or add / remove it from a shelf.
On the Bookshelf tab the user can create bookshelves which allows them to do things such as keep track of the books they'd like to read or record books that they have read before.
The search bar at the top of the screen can be used to filter the books in the shelves. You can search within a specific shelf by selecting it from the listbox on the left side of the screen.
On the Checkout tab the user can purchase the books they have added to their cart or remove books from their cart.

## How to use
Run DataManagement and UserDataManagement2 to populate the database before starting the app.
Login as either user one or user two by using the credentials in a comment at the top of UserDataManagement2.cs (Program.cs).
If you have searched for books and wish to see the books available for purchase again, clear the search bar and press enter.

## Important Notes
If there is an error when trying to login please change 'bo.ToTable("OrderBooks");' to 'bo.ToTable("BookOrders");' in the OnModelCreating method in Order.cs (in the Models folder). 
This should solve the issue. You will then need to run DataManagement and UserDataManagement2 again.
