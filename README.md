# The Kentico Xperience 13 training website

[![CircleCI](https://circleci.com/gh/Kentico/xperience-training-13.svg?style=svg&circle-token=c6fc8e5fb427fcfb6aab9eac3c65f789c7d2c660)](https://circleci.com/gh/Kentico/xperience-training-13)

## About the repo

The repo contains the Medio Clinic sample website developed throughout the two following e-learning courses:
* [Kentico Xperience for developers](https://xperience.io/services/training/xperience-for-developers)
* [Kentico Xperience marketing for developers](https://xperience.io/services/training/xperience-marketing-for-developers)

### About the current branch

The codebase in the current branch represents the final state of development of the **Builders module** of the [Kentico Xperience for developers](https://xperience.io/services/training/xperience-for-developers) course.

### About the other branches

Branches in this repo contain the finished codebases of individual modules of the two above courses:

* [Kentico Xperience for developers: Essentials](https://github.com/Kentico/xperience-training-13/tree/xp-for-developers-essentials)
* [Kentico Xperience for developers: Builders](https://github.com/Kentico/xperience-training-13/tree/xp-for-developers-builders)
* [Kentico Xperience for developers: Identity](https://github.com/Kentico/xperience-training-13/tree/xp-for-developers-identity)
* [Kentico Xperience marketing for developers: Contact management](https://github.com/Kentico/xperience-training-13/tree/xp-marketing-for-developers-contact-management)

The [master branch](https://github.com/Kentico/xperience-training-13) contains all the functionality of both of the courses.

### About the Medio Clinic sample site

The Medio Clinic sample site was developed for other purposes than the Dancing Goat sample site shipped with the Kentico Xperience installer. You'll find different approaches and design patterns in this repo.

See [Kentico Xperience sample sites](https://devnet.kentico.com/articles/kentico-xperience-sample-sites-and-their-differences) for a detailed description of the Medio Clinic site versus the other Xperience sample sites.

## Requirements

Administration application prerequisites:

* Operating systems
    * Windows 8.1 and newer
    * Windows Server 2012 and newer
* IIS features
    * ASP.NET
    * .NET extensibility
    * ISAPI extensions
    * ISAPI filters
    * Static content
* .NET
    * .NET Framework 4.8 or newer

Live site application prerequisites:

* .NET
    * [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1) with ASP.NET Core runtime (included in [Visual Studio 2019 16.4](https://visualstudio.com/vs) or newer)
    
Common prerequisites:

* Visual Studio 2019 Community or higher
    * ASP.NET and web development workload
    * .NET Core cross-platform development workload
    * Git for Windows
    * GitHub Extension for Visual Studio
* [SQL Server 2012 Express or higher](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
    * Case-insensitive collation

For a complete list of system requirements, refer to our [documentation](https://docs.xperience.io/installation/system-requirements).

## How to run the code

> Please find the instructions [in the course](https://xperience.training.kentico.com/).

## Coding conventions

The code in this repo follows the standard [C# coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) documented in the [C# programming guide](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/), and the [coding guidelines](https://github.com/dotnet/aspnetcore/wiki/Engineering-guidelines#coding-guidelines) held by the ASP.NET Core product team at Microsoft.

## Troubleshooting

If you encounter a problem while going through the course, please let us know either through the course survey or by [filing an issue](https://github.com/Kentico/training-xperience-13/issues/new) here in GitHub.
