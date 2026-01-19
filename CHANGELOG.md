# Changelog

## [2.0.0](https://github.com/Odotocodot/Linq2OneNote/compare/v1.2.0...v2.0.0) (2026-01-03)

### ⚠ BREAKING CHANGES

* The package on NuGet.org has changed to `LinqToOneNote`.
* removed support for `netstandard2.0`
* renamed:
  * namespace `Odotocodot.OneNote.Linq` to `LinqToOneNote`.
  * `OneNoteApplication` -> `OneNote`
  * `Traverse` -> `Descendants`
  * `GetPages` -> `GetAllPages`
  * `Notebook.Nickname` -> `Notebook.DisplayName` to better illustrate what it represents.
  * `GetUnfiledNotesSection` -> `GetDefaultNotesLocation` to better illustrate what the method does.
  * `OpenInOneNote` to `Open`
  * `Page.Section` to `Page.Parent`
  * `OneNoteNotebook` -> `Notebook`
  * `OneNoteSectionGroup` -> `SectionGroup`
  * `OneNoteSection` -> `Section`
  * `OneNotePage` -> `Page`
  * `IOneNoteItem.ID` to `IOneNoteItem.Id`
* removed `RelativePathSeparator`
* removed `IOneNoteItem.Notebook` and `IOneNoteItem.RelativePath`
* `CreatePage`, `CreateSection`, `CreateSectionGroup` and `CreateNotebook` now return their respective instance. Replaced the `open` parameter of type `bool` to become type `OpenMode`. `CreatePage` will now through if the `section` parameter is null.
* combined `IsNotebookNameValid`, `IsSectionGroupNameValid`,`IsSectionNameValid` into `IsValidName<T>`. Moved `InvalidNotebookChars`, `InvalidSectionGroupChars` and `InvalidSectionChars` to their respecting class using `INameInvalidCharacters`
* removed `UnfiledNotes` as they are actually the default notes section ([cbd745d](https://github.com/Odotocodot/Linq2OneNote/commit/cbd745d7f75d3ca5a2ac0685d662b3b35b199693))
* removed `OneNote` from class names ([48bf76b](https://github.com/Odotocodot/Linq2OneNote/commit/48bf76b36e9b5d6028f0c94120c4bd486d437176))
* removed obsolete code ([c15c26a](https://github.com/Odotocodot/Linq2OneNote/commit/c15c26a3347de3633aaacba90b8b954ea331e331))
* renamed `Notebook.Nickname` to `Notebook.DisplayName` ([57d2c0c](https://github.com/Odotocodot/Linq2OneNote/commit/57d2c0c81e6488b05e82a9f3bdf09b351140ad7e))
* renamed project to LinqToOneNote ([d06a997](https://github.com/Odotocodot/Linq2OneNote/commit/d06a997ef2b1117c6496c328371c5a99b655e1aa))
* updated invalid name checking with `IsValidName<T>` ([82556b1](https://github.com/Odotocodot/Linq2OneNote/commit/82556b1eb00ba9ce1077d98df67539e11bbc8659))
* updated partial hierarchy methods ([7b55256](https://github.com/Odotocodot/Linq2OneNote/commit/7b55256f0f8b7f218b4978eacce03099a2c621be))

### Features

* added `GetRelativePath` extension method ([1678c14](https://github.com/Odotocodot/Linq2OneNote/commit/1678c14100748f2be7ff992ca795a3d157d25a95))
* added `Open` overload that takes an string id ([2337774](https://github.com/Odotocodot/Linq2OneNote/commit/233777493b14a8630724b37cc762fc336b2c1702))
* added `TryGetNotebook` extension method ([90f6c20](https://github.com/Odotocodot/Linq2OneNote/commit/90f6c20ac2b708ac4e0cce57fef23310a73f8819))
* added `UpdateDescendants` method to `OneNote.Partial` ([2439fff](https://github.com/Odotocodot/Linq2OneNote/commit/2439fff336c43b195b0494715e8e7d95b5841c65))
* added extension method to delete or close OneNote items ([26a0bee](https://github.com/Odotocodot/Linq2OneNote/commit/26a0beec4171926eef5a2f6b43c0936d7d8d5ee2))
* added extension methods for renaming, deleting, closing notebooks, and creating items ([4591d6e](https://github.com/Odotocodot/Linq2OneNote/commit/4591d6e99054f021cbcc99f0be5560cfd7cd1cff))
* added LINQ extensions for ancestor and sibling retrieval methods (`Ancestors`, `AfterSelf` and `BeforeSelf`) ([467403c](https://github.com/Odotocodot/Linq2OneNote/commit/467403c37dfbfe8bc443e8bb09326aa0f5c0f163))
* added option for custom separator to `GetRelativePath` ([6248a9d](https://github.com/Odotocodot/Linq2OneNote/commit/6248a9ddb7a5a5b4cc2c0b2795b4bbc05e5cb8ea))
* added option to create a notebook in a user specified directory ([c6f46bb](https://github.com/Odotocodot/Linq2OneNote/commit/c6f46bb67086776081b2d9eab533ac2b49879786))
* added option to open items in a new window ([7f0d47e](https://github.com/Odotocodot/Linq2OneNote/commit/7f0d47e84de4eb028ec2f95488cbc35e660aefc9))
* added retry when getting trying to get COM object ([625a9d6](https://github.com/Odotocodot/Linq2OneNote/commit/625a9d6b807f288939a6d1f705626dbb91cb9df1))
* added support for `OpenSections` and `UnfiledNotes` ([1fe03a2](https://github.com/Odotocodot/Linq2OneNote/commit/1fe03a242c368f1d672030e918dff6441cb38972))
* added support for closing notebooks (`OneNote.CloseNotebook`) ([110ada9](https://github.com/Odotocodot/Linq2OneNote/commit/110ada9a7274e197d047c9bb60f89b0949c965a6))
* added support for deleting items (`OneNote.DeleteItem`) ([5a97e1e](https://github.com/Odotocodot/Linq2OneNote/commit/5a97e1e5c3dd42d28e3496dad2d772d102d82ec2))
* added support for partial hierarchy parsing ([8b5f32f](https://github.com/Odotocodot/Linq2OneNote/commit/8b5f32f9224dd9db2a482723bdca28bc5f793982))
* added support for renaming items (`OneNote.RenameItem`) ([6160d88](https://github.com/Odotocodot/Linq2OneNote/commit/6160d88a3367d4d1936c5869add2534f9a4cd881))
* improved support for relative path on a partial hierarchy ([e15f185](https://github.com/Odotocodot/Linq2OneNote/commit/e15f185da72bd2f409d4fcbb80e162cf24345dc5))
* improved com object handling, now their are options: manual, init, and wrap ([eb50cff](https://github.com/Odotocodot/Linq2OneNote/commit/eb50cff9190d03cff61b98e6c7922f97346959cf))
* created `OneNoteItemEqualityComparer` for id comparison ([5224638](https://github.com/Odotocodot/Linq2OneNote/commit/52246380effe817ec226df90d5550c37aadd8589))
* `Create` item methods now return an instance of the class ([fffc389](https://github.com/Odotocodot/Linq2OneNote/commit/fffc389f6b8eac2c5d620223076017b363fff0dd))
* updated to .NET 9 ([3692e6e](https://github.com/Odotocodot/Linq2OneNote/commit/3692e6e9312b4c3c62e7e4b19cf7aa59fd183b99))

### Bug Fixes

* fixed `Page.Created` format ([5188864](https://github.com/Odotocodot/Linq2OneNote/commit/5188864c27911bd11b460c673e708d90a5d49c3f))

## [1.2.0] - 2025-07-05

### What’s Changed
- Refactored code base
  - Added abstractions that allow for a reduction in duplicated code, e.g. there are no more overloads for the method `CreateSection`. It now accepts both a `OneNoteNotebook` and `OneNoteSectionGroup` as the parent.
  - Refactored tests.
- Fix invalid links in online documentation.

## [1.1.0] - 2024-06-04

### Added
- Exposed OneNote COM object to allow for more advanced operations if needed.
- Added and refactored parser tests.
- Exposed UpdatePageContent method.
- LinqPad samples
- Added FindByID method to find a hierarchy item by its ID (Currently slow).

### Changed
- Updated logo!
- Renamed IOneNoteItemExtensions to OneNoteItemExtensions.
- OneNoteNotebook.Notebook returns itself rather than null.
- Updated documentation to include examples and more information on the library.
- The methods that create hierarchy items e.g. `CreatePage`, `CreateSection`, `CreateSectionGroup`, `CreateNotebook` now return the ID of the created item. Can be used with the new `FindByID`.

## [1.0.0] - 2023-10-16