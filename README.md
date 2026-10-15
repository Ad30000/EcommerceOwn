# 🛍️ CloudAndCO — E-Commerce Web Application

CloudAndCO is an e-commerce web application built with ASP.NET Core and C#. The project uses Microsoft Azure storage services and Entity Framework Core to provide a foundation for managing application data and product-related content.

## 📌 Overview

CloudAndCO is designed around an online shopping experience, with an application architecture that combines web development, database integration, and cloud storage.

The project explores modern Microsoft web technologies and cloud-based services to support the development of an e-commerce platform.

## ✨ Key Technologies

- **C#** — application development.
- **ASP.NET Core 9** — web application framework.
- **Entity Framework Core 9** — object-relational mapping and database access.
- **SQL Server** — relational database provider.
- **Azure Table Storage** — structured NoSQL storage.
- **Azure Blob Storage** — cloud object storage for files and media.
- **Razor and ASP.NET Core MVC** — include where applicable to the implemented interface.

## 🧰 Prerequisites

Before running the application, install:

- .NET 9 SDK.
- Visual Studio 2022 with the ASP.NET and web development workload, or a compatible IDE.
- SQL Server or an appropriate SQL Server instance.
- An Azure Storage account if the cloud storage features are enabled.

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/Ad30000/EcommerceOwn.git
cd EcommerceOwn
```

### 2. Open the solution

Open `CloudAndCO.sln` in Visual Studio.

### 3. Configure application settings

Configure the database connection string and any required Azure Storage settings using local development configuration or environment variables.

Do not commit passwords, access keys, connection strings containing secrets, or other sensitive credentials to source control.

### 4. Restore dependencies

```bash
dotnet restore CloudAndCO.sln
```

### 5. Build the application

```bash
dotnet build CloudAndCO.sln
```

### 6. Run the application

From the directory containing the web project file, run:

```bash
dotnet run
```

Follow the URL displayed in the terminal to access the application.

**Note:** The exact database setup, migrations, storage configuration, and startup procedure depend on the application's current implementation.

## ☁️ Cloud Integration

The project references the following Azure and Microsoft data-access packages:

| Package | Purpose |
|---|---|
| `Azure.Data.Tables` | Access to Azure Table Storage |
| `Azure.Storage.Blobs` | Access to Azure Blob Storage |
| `Microsoft.EntityFrameworkCore` | Database access through Entity Framework Core |
| `Microsoft.EntityFrameworkCore.SqlServer` | SQL Server integration |
| `Microsoft.EntityFrameworkCore.Tools` | Entity Framework Core development tools |

## 📁 Repository Structure

```text
EcommerceOwn/
├── CloudAndCO/
│   ├── CloudAndCO.csproj
│   └── ...
├── CloudAndCO.sln
├── .gitattributes
└── .gitignore
```

The `CloudAndCO` directory contains the application project, while `CloudAndCO.sln` provides the Visual Studio solution.

## 🔒 Configuration and Security

- Keep credentials outside committed source files.
- Use environment-specific configuration for local development and deployment.
- Restrict access to Azure resources using appropriate permissions.
- Validate user input and handle application errors securely.

## 👨‍💻 Author

**Ad30000**

GitHub: [@Ad30000](https://github.com/Ad30000)

## 📂 Repository

[View CloudAndCO on GitHub](https://github.com/Ad30000/EcommerceOwn)

---

*CloudAndCO is a software development project exploring e-commerce application development with ASP.NET Core and Azure services.*
