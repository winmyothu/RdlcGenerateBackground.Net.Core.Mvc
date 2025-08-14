# RDLC SignalR Background Report Demo

This is a demo ASP.NET Core 8 MVC project that generates PDF reports in the background using a hosted service and updates clients in real-time using SignalR.

## Features

- Queue report generation jobs
- Real-time progress updates with SignalR
- Download generated PDF when complete
- Track job status: Pending, InProgress, Completed, Failed

## Tech Stack

- ASP.NET Core 8 MVC
- SignalR 8
- Entity Framework Core
- SQL Server
- Bootstrap 5

## How to Run

### 1. Clone the repository

```bash
git clone https://github.com/<USERNAME>/<REPO>.git
cd <REPO>
```

### 2. Install .NET SDK

Make sure you have **.NET 8 SDK** installed. Check with:

```bash
dotnet --version
```

### 3. Restore NuGet packages

```bash
dotnet restore
```

### 4. Update the database connection string

Open `appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=True;"
}
```

### 5. Apply database migrations

```bash
dotnet ef database update
```

### 6. Run the project

```bash
dotnet run
```

By default, the app will run at:

```
https://localhost:5001
http://localhost:5000
```

### 7. Open your browser

- Navigate to `https://localhost:5001` to generate reports.
- Go to `/Reports/List` to see all jobs and live progress updates.

### 8. Check generated PDFs

All generated reports are stored in:

```
/Reports/Output
```

## Notes

- SignalR updates progress **in real-time** on both the generation page and the job list page.
- Job statuses: **Pending → InProgress → Completed → Failed**.
- Completed reports will have a **Download** button.

