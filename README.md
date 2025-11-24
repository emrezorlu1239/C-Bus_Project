Bus Seat Reservation System
🌟 Project Overview
This is a robust, single-user desktop application developed using C# Windows Forms (WinForms) and SQL Server LocalDB to manage bus seat registrations and passenger data. The project emphasizes clean database integration and a functional, easy-to-use graphical interface.

The application successfully demonstrates fundamental concepts of database connectivity and CRUD (Create, Read, Update, Delete) operations within a real-world scenario.

✨ Features
The application operates in two primary modes: Registration Mode and View Mode.

1. Seat Visualization and Availability
Dynamic Coloring: The 24-seat layout visually indicates the status of each seat:

Available: Green (Ready for registration).

Occupied: Red (Cannot be selected for a new registration).

Searched: Blue (Used in View Mode to highlight a specific passenger's seat).

Driver & Back: Dedicated buttons for context and navigation.

2. Registration (CRUD - Create)
Allows users to register a new passenger by entering Name, Surname, TC, Seat Number, and Gender.

Robust Validation: Includes essential checks:

All input fields must be filled.

Strict 11-Digit TC ID Validation to ensure data integrity.

Checks if the selected seat is already occupied before insertion.

3. Search and View (CRUD - Read)
View Mode: Enables users to search for existing passenger records in the database based on the information provided in the input fields. The search result is displayed in a separate area (data grid/list, if visible).

4. Deletion (CRUD - Delete)
Allows users to securely remove a passenger record by providing their TC ID and Seat Number.

Upon successful deletion, the corresponding seat is instantly marked Green (Available) and the data is permanently removed from the database.

🔍 Snapshot of Data
Your application's data when viewed in SQL Server looks like this :

Id,name,surname,no,tc,gender
1,Ahmet,Yılmaz,1,12345678901,Male
2,Mehmet,Şentürk,5,134562378901,Male
3,Ayşe,Küçük,10,14563278901,Female

🛠️ Tech Stack
Frontend/GUI: C# Windows Forms (WinForms)

Backend/Logic: C#

Database: SQL Server LocalDB (Database1.mdf)

Data Access: ADO.NET (SqlConnection, SqlCommand)

Portability: Dynamic connection string using System.IO to ensure the project works after cloning (GitHub-ready).

⚙️ Setup and Running Locally
To run this project, you need Visual Studio with the .NET desktop development workload installed.

Clone the Repository:

Bash

git clone [YOUR_REPOSITORY_URL]
Open Solution: Open the C#_SQL_Project.sln solution file in Visual Studio.

LocalDB Check: Ensure your Visual Studio installation includes SQL Server Express LocalDB.

Run: Press F5 (Start Debugging). The application will automatically connect to the Database1.mdf file located in the project's root folder, thanks to the portable connection setup.
