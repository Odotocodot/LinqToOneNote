# LINQ to OneNote

A helper library for dealing with the [OneNote Interop API](https://learn.microsoft.com/en-us/office/client-developer/onenote/application-interface-onenote).
Originally made for [Flow.Launcher.Plugin.OneNote](https://github.com/Odotocodot/Flow.Launcher.Plugin.OneNote).

```csharp
//Search pages that have "hello there" in the title or content.
var pages = OneNote.FindPages("hello there");
Page page = pages.FirstOrDefault();

if (page == null)
    return;

Console.WriteLine(page.Name);
page.Open(); // or OneNote.Open(page)
```

```csharp
var root = OneNote.GetFullHierarchy();
var items = root.Notebooks
                .Descendants()
                .Take(10);

foreach(var item in items)
{
    Console.WriteLine("Item: " + item.Name + " | Parent:"  + item.Parent?.Name);
}
```

View the [documentation](https://odotocodot.github.io/LinqToOneNote/) for more information and examples or visit the the [API Reference](https://odotocodot.github.io/LinqToOneNote/api/LinqToOneNote.html) to see the full API.\
Most functions return an IEnumerable allowing for easy use with LINQ.

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


