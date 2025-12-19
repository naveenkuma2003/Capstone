# DigitalBank Lite - Setup Instructions

To run this project on another machine, follow these steps.

## Prerequisites
1.  **.NET 8 SDK**: Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0).
2.  **Node.js**: Download from [nodejs.org](https://nodejs.org/).
3.  **SQL Server**: A local SQL Server instance (e.g., SQL Server Express or LocalDB).

## 1. Backend Setup (.NET API)

1.  Navigate to the **API** folder:
    ```bash
    cd DigitalBankLite.API
    ```

2.  **Update Connection String**:
    *   Open `appsettings.json`.
    *   Locate `"ConnectionStrings": { "DefaultConnection": "..." }`.
    *   Change `Server=YOUR_OLD_SERVER_NAME` to your actual server instance.
    *   Example for LocalDB: `Server=(localdb)\\mssqllocaldb;Database=DigitalBankLiteDb;Trusted_Connection=True;...`
    *   Example for default local SQL: `Server=.;Database=DigitalBankLiteDb;Trusted_Connection=True;...`

3.  **Create Database**:
    Run the following commands to restore dependencies and create the database from migrations:
    ```bash
    dotnet restore
    dotnet ef database update
    ```

4.  **Run the Backend**:
    ```bash
    dotnet run
    ```
    The API will start (usually at `http://localhost:5105`).

## 2. Frontend Setup (Angular)

1.  Open a new terminal and navigate to the **Client** folder:
    ```bash
    cd DigitalBankLite.Client
    ```

2.  **Install Dependencies**:
    ```bash
    npm install
    ```

3.  **Run the Frontend**:
    ```bash
    npm start
    ```
    The application will open at `http://localhost:4200`.

## 3. Login Credentials

**Admin:**
*   Email: `admin@digitalbank.com`
*   Password: `admin` (or whatever password was set)

**Customer:**
*   Register a new user via the "Register" link on the login page.
*   Log in with the registered credentials.
