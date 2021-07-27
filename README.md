# The Kentico Xperience 13 training website

[![CircleCI](https://circleci.com/gh/Kentico/xperience-training-13.svg?style=svg&circle-token=c6fc8e5fb427fcfb6aab9eac3c65f789c7d2c660)](https://circleci.com/gh/Kentico/xperience-training-13)

## About the repo

The repo contains the Medio Clinic sample website developed throughout the [Kentico Xperience 13 for Developers](https://xperience.io/services/training) course. The codebase represents the final state of development.

The repo currently contains code of the following modules of the course:

* Essentials
* Builders
* Identity

The code of the modules does not exist in separate git branches or is otherwise split. It lives together as one working Visual Studio solution, internally separated using standard conventions (separate projects, MVC areas). Therefore, when taking the course, bear in mind that the code snippets in the course might slightly differ from what you see here in GitHub.

See [Kentico Xperience sample sites](https://devnet.kentico.com/articles/kentico-xperience-sample-sites-and-their-differences) for a detailed description of this and other Xperience sample sites.

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

> **Note:** Below you'll find a high-level list of instructions. The full instructions can be found in the course.

You need to install Xperience program files along with a blank website instance. You'll only need the blank database. Once installed, both Visual Studio solutions of the administration application and the blank site can be deleted since all files are provided by this repository.

1. Clone or fork the repository.
1. Download the installer through the [trial download page](https://xperience.io/get-started/trial). (Existing customers can download the installer in the [client portal](https://client.kentico.com/). Partners can do so in the [partner portal](https://partner.kentico.com/).)
1. Run the `Kentico_13_0.exe` file.
1. Install a blank ASP.NET Core website.
1. Give the site a name `MedioClinic`.
1. Let the installer deploy a database.
1. In the cloned repository, adjust the connection strings to point to your newly deployed blank database.
1. Also, adjust the `CMSHashStringSalt` environment value in the cloned repository to match the one in the newly deployed blank site.
1. In the cloned repository, run `/CMS/bin/ContinuousIntegration.exe -r` to restore the objects to the database.
1. Open the `WebApp.sln` solution and build it.
1. Register the administration application in IIS and add development licenses (found in the course).

## Enabling external authentication

Prior to enabling external authentication for your development instance, make sure you've set the `ASPNETCORE_ENVIRONMENT` [environment variable](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-3.1) to `Development`.

### Google

Create a new [Google Console](https://console.developers.google.com/) project for your website. Create the OAuth Consent Screen and generate the [OAuth Client ID](https://support.google.com/cloud/answer/6158849). Set the __Authorized redirect URIs__ to `https://localhost:44324/signin-google`.

Add the generated __Client ID__ to your `appsettings.json`:

```json
"GoogleAuthenticationOptions": {
  "UseGoogleAuth": true,
  "ClientId": "<your-client-id>",
  "ClientSecret": "<your-client-secret>"
},
```

Store the __Client Secret__ value using the [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows) feature.

### Microsoft

In the Azure Portal, create a new __App registration__ following [Microsoft's documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-3.1#create-the-app-in-microsoft-developer-portal). Ensure that the __Redirect URI__ is set to `https://localhost:44324/signin-microsoft`.

Add the __Application (client) ID__ from the _Overview tab_ you generated to the `appsettings.json`:

```json
"MicrosoftAuthenticationOptions": {
  "UseMicrosoftAuth": true,
  "ClientId": "<your-client-id>",
  "ClientSecret": "<your-client-secret>"
},
```

Store the __Client secret__ value using the [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows) feature.

### Facebook

Create a [Facebook application](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins?view=aspnetcore-3.1#create-the-app-in-facebook) with the __OAuth Redirect URL__ of `https://localhost:44324/signin-facebook`. Add the following to your `appsettings.json` with the __App ID__ from the _Settings > Basic_ tab:

```json
"FacebookAuthenticationOptions": {
  "UseFacebookAuth": true,
  "AppId": "<your-app-id>",
  "AppSecret": "<your-app-secret>"
},
```

Store the __App secret__ value using the [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows) feature.

### Twitter

Create a [Twitter application](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/twitter-logins?view=aspnetcore-3.1#create-the-app-in-twitter) with the __Callback URL__ of `https://localhost:44324/signin-twitter`. On the _Keys and Tokens_ tab, copy the __API key__ into your `appsettings.json`:

```json
"TwitterAuthenticationOptions": {
  "UseTwitterAuth": true,
  "ConsumerKey": "<your-api-key>",
  "ConsumerSecret": "<your-api-secret-key>"
},
```

Store the __API secret key__ value using the [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows) feature.

## Coding conventions

The code in this repo follows the standard [C# coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) documented in the [C# programming guide](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/), and the [coding guidelines](https://github.com/dotnet/aspnetcore/wiki/Engineering-guidelines#coding-guidelines) held by the ASP.NET Core product team at Microsoft.

## Troubleshooting

If you encounter a problem while going through the course, please let us know either through the course survey or by [filing an issue](https://github.com/Kentico/training-xperience-13/issues/new) here in GitHub.
