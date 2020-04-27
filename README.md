# The Kentico Xperience 13 training website

## About the repo

The repo contains the Medio Clinic sample website developed throughout the Kentico [Xperience MVC for Developers course](https://www.kentico.com/services/training/). The codebase represents the final state of development.

The repo contains code of all modules of the course:

* Essentials
* Builders
* Identity

## How to run the code

To make the project work, follow these steps:

1. To run the Xperience administration application, make sure your computer meets the [system requirements](https://docs.kentico.com/13/installation/system-requirements) outlined in the Xperience documentation.
1. To run the ASP.NET Core 3.1 application, download the [.NET Core 3.1.3 SDK](https://github.com/dotnet/core/blob/master/release-notes/3.1/3.1.3/3.1.3.md).
1. Clone the repo (`git clone https://github.com/Kentico/training-xperience-13`).
1. Extract either a database backup file out of [/Db/MedioClinic.zip](/Db/MedioClinic.zip) or, a database build script out of [/Db/MedioClinicSqlScript.zip](/Db/MedioClinicSqlScript.zip) (if you happen to have an older version of SQL Server).
1. Start your [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) and restore the extracted MedioClinic.bak file. Alternatively, run the extracted MedioClinicSqlScript.sql file to build out the database.
1. [Change the below instructions for Core]
1. Register both the [administration interface](/CMS) and the [Medio Clinic website](/MedioClinic) in IIS.
    * If you register the administration interface as an application that sits under `Default Web Site` and has a `Kentico12_Admin` alias, then you won't have to do any adjustments in Visual Studio.
    * The same applies to the Medio Clinic project: If you register it under `Default Web Site` as `Kentico12_MedioClinic`, then you should be ready to compile and run.
1. Open Visual Studio with elevated credentials, open the `WebApp.sln` solution and build it (`Ctrl+Shift+B`).
1. Open the `web.config` file and adjust the connection string to your SQL Server instance (if your database instance runs on a different machine).
1. Close the solution.
1. Open the `MedioClinic.sln` solution.
    * If you haven't opened Visual Studio with elevated credentials, then you may encounter an error message saying Visual Studio doesn't have access to your local IIS.
    * If you haven't registed the project under `Default Web Site` as `Kentico12_Admin`, then you might want to adjust debugging settings through the following steps:
        * Go to the solution explorer
        * Right-click the `MedioClinic` project
        * Go to the Web tab
        * Under the Servers section > Project Url, set the correct URL according to your IIS configuration.
1. Build the solution.
1. Open the `/Config/ConnectionStrings.config` file to eventually adjust the connection string (in the same way as you did with the administration interface project).
