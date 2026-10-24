# PhotoShop

PhotoShop is a web platform for browsing, selling and downloading photographs. Photographers' work is organised by tags, users can search and download photos, and administrators manage the whole catalogue through a dedicated admin interface.

The solution consists of two ASP.NET Core applications that share one SQL Server database:

| Project | Type | Purpose |
|---|---|---|
| **WebApp** | ASP.NET Core MVC (Razor views) | The main website for users and administrators |
| **WebAPI** | ASP.NET Core Web API | REST API with JWT authentication, Swagger docs and a simple log viewer |

---

## Features

### WebApp (MVC)
- **Landing page** with a call-to-action to sign in
- **Registration and login** with cookie-based authentication and BCrypt password hashing
- **Two roles:**
  - **User** – browses the photo gallery, searches by text, filters by tag, opens photo details and downloads photos; can edit their own profile
  - **Admin** – full CRUD for **photos**, **photographers** and **tags**, plus an overview of **which users downloaded which photos**
- Search, filtering and pagination of photos
- Many-to-many relationship between photos and tags
- Validation on all forms

### WebAPI (REST)
- **Auth:** `POST /api/auth/register`, `POST /api/auth/login` (returns a JWT), `POST /api/auth/changepassword`
- **Photos:** `GET /api/photo`, `GET /api/photo/{id}`, `GET /api/photo/search`, `POST`, `PUT /api/photo/{id}`, `DELETE /api/photo/{id}`
- **Photographers:** CRUD on `/api/photographer`
- **Tags:** CRUD on `/api/tag`
- **Downloads:** `POST /api/download`, `GET /api/download/user/{userId}`
- **Logs** (JWT protected): `GET /api/logs/get/{n}`, `GET /api/logs/count`
- Every important action is written to the `Log` table
- **Swagger UI** with Bearer token support
- Static pages `login.html` and `logs.html` for signing in and viewing the latest log entries in the browser

---

## Technologies

- **C# / .NET 8** (ASP.NET Core)
- **ASP.NET Core MVC** with **Razor** views – WebApp
- **ASP.NET Core Web API** – WebAPI
- **Entity Framework Core 8** (SQL Server provider)
- **Microsoft SQL Server** (SQL Server Express works fine)
- **Cookie authentication** (WebApp) and **JWT Bearer authentication** (WebAPI)
- **BCrypt.Net-Next** – password hashing
- **Swashbuckle / Swagger** – API documentation
- **Bootstrap 5.3** – UI framework, with a custom theme (`WebApp/wwwroot/css/theme.css`)
- **HTML, CSS, JavaScript** (Fetch API) – static log pages in WebAPI

---

## Project structure

```
PhotoShop/
├── Database/
│   └── Database.sql            # Creates tables and inserts sample data
└── PhotoShop/
    ├── PhotoShop.sln
    ├── WebApp/                 # MVC website
    │   ├── Controllers/
    │   ├── Data/               # AppDbContext
    │   ├── Models/
    │   ├── ViewModels/
    │   ├── Views/
    │   └── wwwroot/css/theme.css
    └── WebAPI/                 # REST API
        ├── Controllers/
        ├── Data/
        ├── DTOs/
        ├── Models/
        ├── Services/           # JwtService, LogService
        └── wwwroot/            # login.html, logs.html
```

---

## Getting started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Microsoft SQL Server (Express, Developer or LocalDB) and SQL Server Management Studio (or another SQL client)
- Visual Studio 2022 (recommended) or VS Code / the `dotnet` CLI

### 1. Create the database
1. In SQL Server Management Studio create an empty database called **`PhotoShopDB`**.
2. Open `Database/Database.sql`, select the `PhotoShopDB` database and run the script. It creates all tables (`Photographer`, `AppUser`, `Photo`, `Tag`, `PhotoTag`, `Download`, `Log`) and inserts sample photographers, tags and photos.

> The script also contains an example `UPDATE ... SET Role = 'Admin'` statement and a couple of `SELECT` statements at the end. These are only helpers – run just the table creation and the `INSERT` statements on the first run.

### 2. Configure the connection string
In **both** `WebApp/appsettings.json` and `WebAPI/appsettings.json`, change `DefaultConnection` so it points to your SQL Server instance, for example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=PhotoShopDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

For WebAPI you can also change the JWT settings (`Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiryHours`). The key must be at least 32 characters long.

### 3. Run the applications

**Visual Studio:** open `PhotoShop/PhotoShop.sln`, right-click the solution → *Configure Startup Projects* → *Multiple startup projects* → set both **WebApp** and **WebAPI** to *Start*, then press **F5**.

**Command line:**

```bash
cd PhotoShop/PhotoShop

# terminal 1
dotnet run --project WebApp

# terminal 2
dotnet run --project WebAPI
```

Default addresses (from `launchSettings.json`):

| Application | URL |
|---|---|
| WebApp | http://localhost:53734 |
| WebAPI – Swagger | http://localhost:5010/swagger |
| WebAPI – log viewer | http://localhost:5010/login.html |

### 4. Create an admin account
New accounts are always created with the **User** role. To get an administrator:

1. Register a new account in the WebApp.
2. Run this in SQL Server:
   ```sql
   UPDATE AppUser SET Role = 'Admin' WHERE Username = 'your_username';
   ```
3. Log out and log back in – you will now see the admin menu (Photos, Photographers, Tags, Users).

### 5. Using the API
1. Open Swagger at `http://localhost:5010/swagger`.
2. Call `POST /api/auth/login` with your username and password and copy the returned `token`.
3. Click **Authorize** and enter `Bearer {token}`.
4. You can now call protected endpoints such as `/api/logs/get/{n}`.

---
## Security note: JWT secret key

The JWT signing key is stored in plain text in `WebAPI/appsettings.json` (`Jwt:Key`), so anyone who can see the source code can see it. Anyone who knows this key can create valid tokens and get access to the protected API endpoints.

This was a deliberate choice. PhotoShop is a student/learning project that only runs locally, so hiding the key was not needed. Keeping it in `appsettings.json` also means the project runs right after cloning, with no extra setup.

In a real application the key should never be committed to the repository.

## Notes
- The UI is in Croatian.

