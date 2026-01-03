# The Hierarchy Conundrum

When calling [OneNote.FindPages](xref:LinqToOneNote.OneNote.FindPages(System.String)) or using the functions in 
[OneNote.Partial](xref:LinqToOneNote.OneNote.Partial) your OneNote hierarchy is only _partially_ returned (compared to 
calling [OneNote.GetFullHierarchy()](xref:LinqToOneNote.OneNote.GetFullHierarchy) which returns everything).
For `OneNote.FindPages` the pages returned have their ancestor references up to the notebooks that owns them, but those 
notebooks will not have all their descendants (section groups, sections and pages) present, only the ones that contain pages that were found.

This can lead to weird scenarios such as:
```csharp
var page = OneNote.FindPages("A unique page").First();

page.TryGetNotebook(out Notebook searchNotebook);

var fullHierarchyNotebook = OneNote.GetFullHierarchy().Notebooks.First(n => n.Name == searchNotebook.Name);
//Here's where things get weird
Console.WriteLine(searchNotebook == fullHierarchyNotebook); //Prints "false". Even though they both represent the same item in OneNote
Console.WriteLine(searchNotebook.Id == fullHierarchyNotebook.Id); //Prints "true"
Console.WriteLine(searchNotebook.Descendants().Count() ==
                  fullHierarchyNotebook.Descendants().Count()); //Prints "false"
```
Therefore, to check if two objects represent the same item in OneNote compare their [Id](xref:LinqToOneNote.Abstractions.INavigable.Id) property or use [OneNoteItemEqualityComparer.Default](xref:LinqToOneNote.OneNoteItemEqualityComparer.Default).