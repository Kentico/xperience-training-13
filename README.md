# The Kentico Xperience 13 training website

[![CircleCI](https://circleci.com/gh/Kentico/xperience-training-13.svg?style=svg&circle-token=c6fc8e5fb427fcfb6aab9eac3c65f789c7d2c660)](https://circleci.com/gh/Kentico/xperience-training-13)

## About the repo

The repo contains the **Medio Clinic** sample website developed throughout the two following e-learning courses:
* [Kentico Xperience for developers](https://xperience.io/services/training/xperience-for-developers)
* [Kentico Xperience marketing for developers](https://xperience.io/services/training/xperience-marketing-for-developers)

### The current branch

The code in the current branch represents the final state of development of the **Essentials** module of the [Kentico Xperience for developers](https://xperience.io/services/training/xperience-for-developers) course.

If you want to code along the exercises in the module, the only thing you'll need from the branch is the [CIRepository](https://github.com/Kentico/xperience-training-13/tree/xp-for-developers-essentials/CMS/App_Data/CIRepository) folder used to restore database objects. See the instructions [in the courses](https://xperience.training.kentico.com/) for details.

## Other branches

The repo has branches that represent the various stages of development. The above two courses are split into modules. A course always has an introductory module, followed by several optional ones. Typically, a single branch represents the outcome of all the exercises in a given course module. At the same time, the branch serves as a starting point for another module or another course.

As for the **Kentico Xperience 13 for developers** course, the repo offers the following branches:  

* For the introductory **Essentials** module, you can checkout the [xp-for-developers-essentials](https://github.com/Kentico/xperience-training-13/tree/xp-for-developers-essentials) branch. The branch contains the final state of the development of the Essentials module. If you wish to start developing the solution from scratch, the only thing you need from the branch are the XML files in the [CIRepository](https://github.com/Kentico/xperience-training-13/tree/xp-for-developers-essentials/CMS/App_Data/CIRepository) folder used to restore database objects.
* To start coding along the exercises in the **Builders** module, checkout the [xp-for-developers-essentials](https://github.com/Kentico/xperience-training-13/tree/xp-for-developers-essentials) branch. To see the final state of development, checkout the [xp-for-developers-builders](https://github.com/Kentico/xperience-training-13/tree/xp-for-developers-builders) branch.
* To start coding along the **Identity** module, checkout the [xp-for-developers-essentials](https://github.com/Kentico/xperience-training-13/tree/xp-for-developers-essentials) branch. To see the final state of development, checkout the [xp-for-developers-identity](https://github.com/Kentico/xperience-training-13/tree/xp-for-developers-identity) branch.

As for the **Kentico Xperience 13 online marketing for developers** course, the repo offer the following branches:

* To start coding along the exercises in the introductory **Contact management** module, you can checkout the [xp-marketing-for-developers-starting-point](https://github.com/Kentico/xperience-training-13/tree/xp-marketing-for-developers-starting-point) branch. To see the the final state of development, checkout the [xp-marketing-for-developers-contact-management](https://github.com/Kentico/xperience-training-13/tree/xp-marketing-for-developers-contact-management) branch.
* To start coding along the exercises in the **Personalization** module, you can checkout the [xp-marketing-for-developers-personalization-starting-point](https://github.com/Kentico/xperience-training-13/tree/xp-marketing-for-developers-personalization-starting-point) branch. To see the final state of development, checkout the [xp-marketing-for-developers-personalization](https://github.com/Kentico/xperience-training-13/tree/xp-marketing-for-developers-personalization) branch.
* To start coding along the exercises in the **Email marketing** module, you can checkout the [xp-marketing-for-developers-email-marketing-starting-point](https://github.com/Kentico/xperience-training-13/tree/xp-marketing-for-developers-email-marketing-starting-point) branch. To see the final state of development, checkout the [xp-marketing-for-developers-email-marketing](https://github.com/Kentico/xperience-training-13/tree/xp-marketing-for-developers-email-marketing) branch.

The [master](https://github.com/Kentico/xperience-training-13) branch represents the final state of development of **both** the _Kentico Xperience 13 for developers_ and _Kentico Xperience 13 online marketing for developers_ courses, including their optional modules. The branch has all the code working together.

## About the Medio Clinic sample site

The Medio Clinic sample site was developed for other purposes than the Dancing Goat sample site shipped with the Kentico Xperience installer. You'll find different approaches and design patterns in this repo.

See [Kentico Xperience sample sites](https://devnet.kentico.com/articles/kentico-xperience-sample-sites-and-their-differences) for a detailed description of the Medio Clinic site versus the other Xperience sample sites.

## How to run the code

> Please find the instructions [in the courses](https://xperience.training.kentico.com/).

## Coding conventions

The code in this repo follows the standard [C# coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) documented in the [C# programming guide](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/), and the [coding guidelines](https://github.com/dotnet/aspnetcore/wiki/Engineering-guidelines#coding-guidelines) held by the ASP.NET Core product team at Microsoft.

## Troubleshooting

If you encounter a problem while going through the course, please let us know either through the course survey or by [filing an issue](https://github.com/Kentico/training-xperience-13/issues/new) here in GitHub.
