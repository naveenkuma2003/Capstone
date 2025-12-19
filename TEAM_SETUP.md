# How to Set Up DigitalBank Lite (For Team Members)

If you have received this project as a zip file, follow these steps to get everything running on your machine.

## 1. Prerequisites
Make sure you have the following installed:
*   **.NET 8 SDK**: [Download Here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
*   **Node.js (v18+)**: [Download Here](https://nodejs.org/)
*   **SQL Server** (LocalDB or Express)

## 2. Unzip and Open
1.  Extract the zip file.
2.  Open the folder in **Visual Studio Code** or **Visual Studio**.

## 3. Database Setup (CRITICAL STEP)
Since the database files are not shared, you must recreate the database on your local SQL Server.

1.  **Update Connection String**:
    *   Open `DigitalBankLite.API/appsettings.json`.
    *   Find `"DefaultConnection"`.
    *   Change `Server=YOUR_SERVER_NAME;` to your local SQL Server instance.
        *   Example for Visual Studio default: `Server=(localdb)\\mssqllocaldb;Database=DigitalBankLiteDb;Trusted_Connection=True;...`
        *   Example for SQL Express: `Server=.\\SQLEXPRESS;Database=DigitalBankLiteDb;Trusted_Connection=True;...`

2.  **Create Database**:
    *   Open a terminal in the `DigitalBankLite.API` folder.
    *   Run the following command:
        ```bash
        dotnet ef database update
        ```
    *   ✅ **What this does:** It looks at the `Migrations` folder in the project and automatically creates the Database, Tables, and Keys exactly as they were on the original machine.

## 4. Run the Backend API
1.  In the `DigitalBankLite.API` terminal:
    ```bash
    dotnet run
    ```
2.  The API should start at `http://localhost:5105`.

## 5. Run the Frontend (Angular)
1.  Open a new terminal in the `DigitalBankLite.Client` folder.
2.  Install dependencies (first time only):
    ```bash
    npm install
    ```
3.  Start the app:
    ```bash
    npm start
    ```
4.  Open `http://localhost:4200` in your browser.

## Troubleshooting
*   **"dotnet ef not found"**: Run `dotnet tool install --global dotnet-ef`.
*   **Connection Refused**: Double-check your `Server=` name in `appsettings.json`.
