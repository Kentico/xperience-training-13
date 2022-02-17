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
