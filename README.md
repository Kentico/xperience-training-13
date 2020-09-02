# The Kentico Xperience 13 training website

[![CircleCI](https://circleci.com/gh/Kentico/xperience-training-13.svg?style=svg&circle-token=c6fc8e5fb427fcfb6aab9eac3c65f789c7d2c660)](https://circleci.com/gh/Kentico/xperience-training-13)

## About the repo

The repo contains the Medio Clinic sample website developed throughout the Kentico [Xperience MVC for Developers course](https://www.kentico.com/services/training/). The codebase represents the final state of development.

The repo will eventually contain code of all modules of the course:

* Essentials
* Builders
* Identity

The code of the modules is not separated into git branches or otherwise. It lives together as one working Visual Studio solution. When taking the course, bear in mind that the code snippets in the course might slightly differ from what you see here in GitHub.

## Requirements

You need the following prerequisites:

* [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)
    * You can also get the SDK by installing [Visual Studio 2019 16.4](https://visualstudio.com/vs) or higher.
* [SQL Server 2016 SP2 or newer](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
    * You can use the Express edition as well as the Developer edition (for development use only).

## How to run the code

To make the project work, follow these steps:

1. To run the Xperience administration application, make sure your computer meets the [system requirements](https://docs.kentico.com/13/installation/system-requirements) outlined in the Xperience documentation.
1. To run the ASP.NET Core 3.1 application, download the [.NET Core 3.1.3 SDK](https://github.com/dotnet/core/blob/master/release-notes/3.1/3.1.3/3.1.3.md).
1. Clone the repo (`git clone https://github.com/Kentico/training-xperience-13`).
1. Register the [administration interface](/CMS) in IIS.
    * If you register the administration interface as an application that sits under `Default Web Site` and has a `Kentico13_Admin` alias, then you won't have to do any adjustments in Visual Studio.
1. Open Visual Studio with elevated credentials, open the `WebApp.sln` solution and build it (`Ctrl+Shift+B`).
1. Close the solution.
1. Access the administrative interface. Follow the installation wizard to create a new database named "training-xperience-13" with the default Kentico objects.
1. Run `ContinuousIntegration.exe -r` from the ~/CMS/bin directory
1. Open the `MedioClinic.sln` solution.
1. Build the solution.
1. Under [MedioClinic](/MedioClinic), create an `appsettings.Development.json` file. If you run your SQL Server locally, the file can have the following structure:

```json
    {
      "ConnectionStrings": {
        "CMSConnectionString": "Data Source=localhost;Initial Catalog=training-xperience-13;Integrated Security=True;Persist Security Info=False;Connect Timeout=60;Encrypt=False;Current Language=English;"
      },
      "CMSHashStringSalt": "ea505e97-2d5f-4d74-b4eb-1cbea203f877",
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft": "Warning",
          "Microsoft.Hosting.Lifetime": "Information"
        }
      }
    }
```

## Coding conventions

The code in this repo follows the standard [C# coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) documented in the [C# programming guide](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/), and the [coding guidelines](https://github.com/dotnet/aspnetcore/wiki/Engineering-guidelines#coding-guidelines) held by the ASP.NET Core product team at Microsoft.

## Troubleshooting

If you encounter a problem while going through the course, please let us know either through the course survey or by [filing an issue](https://github.com/Kentico/training-xperience-13/issues/new) here in GitHub.
