<h1 align="center">
    <img src="https://github.com/Odotocodot/LinqToOneNote/assets/48138990/4b6025ab-6aa7-4d5e-aac6-2328961daeb5" alt="logo" width=40 height=40>
LINQ to OneNote
    <img src="https://github.com/Odotocodot/LinqToOneNote/assets/48138990/9f6b5f41-ed6a-4840-8766-fd5890c6bb7c" alt="logo mini" width=40 height=40>
</h1>

A helper library for dealing with the [OneNote Interop API](https://learn.microsoft.com/en-us/office/client-developer/onenote/application-interface-onenote).
Originally made for [Flow.Launcher.Plugin.OneNote](https://github.com/Odotocodot/Flow.Launcher.Plugin.OneNote).

## Installation

Get the library from NuGet [here](https://www.nuget.org/packages/LinqToOneNote/):
```
dotnet add package LinqToOneNote
```

## Features

- Search your OneNote pages. Optionally specify a notebook, section group or section to restrict the search to.
- Create OneNote items.
- Open items in OneNote.
- Rename items.
- Delete items.
- Traverse your whole OneNote hierarchy` with Linq To Tree-esque methods:
  - `IOneNoteItem.Children`
  - `IOneNoteItem.Descendants()`
  - `IOneNoteItem.Ancestors()`
  - `IOneNoteItem.AfterSelf()`
  - `IOneNoteItem.BeforeSelf()`
- Query only a part of your OneNote hierarchy with methods in `OneNote.Partial`. This is especially useful if you have a substantial amount of notes i.e `OneNote.GetFullHierarchy()` takes too long.
- Interact with OneNote sections that are not in any Notebook -> `Root.OpenSections`

## Usage

View the [documentation](https://odotocodot.github.io/LinqToOneNote/) for more information and examples or visit the the [API Reference](https://odotocodot.github.io/LinqToOneNote/api/LinqToOneNote.html) to see the full API.\
Most functions return an IEnumerable allowing for easy use with LINQ.

## Quick Start

The main entry point of the library is the static class ``OneNote`` which has a collection of [methods](https://odotocodot.github.io/LinqToOneNote/api/LinqToOneNote.OneNote.html#methods) that interact with your OneNote installation.

Below is quick example on using the library to search your OneNote pages.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using LinqToOneNote;

namespace Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Search pages that have "hello there" in the title or content.
            IEnumerable<Page> pages = OneNote.FindPages("hello there");
          
            Page page = pages.FirstOrDefault();

            if (page == null)
                return;
          
            Console.WriteLine(page.Name);
            page.Open(); // or OneNote.Open(page)

            //Get the full OneNote hierarchy.
            var root = OneNote.GetFullHierarchy();
            var items = root.Notebooks
                            .Descendants(x => x.LastModified > page.LastModified) //Traverse all items with a predicate
                            .Take(10);
            foreach (var item in items)
            {
                Console.WriteLine(item.Name);
            }
        }
    }
}
```

## Inspired By

- [ScipeBe Common Office](https://github.com/scipbe/ScipBe-Common-Office)
- [OneNote Object Model](https://github.com/idvorkin/onom)