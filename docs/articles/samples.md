# Samples and Examples

The library's original purpose was for the OneNote [Flow Launcher](https://www.flowlauncher.com/) plugin available [here](https://github.com/Odotocodot/Flow.Launcher.Plugin.OneNote). This plugin itself has several good examples of how to use the library.

The examples below are not exactly best practices, but they should give you a good starting point!

They can also be found in the free and paid version of [LinqPad](https://www.linqpad.net/) for easy viewing! (Though be weary of the [Create Page](#create-pages-in-sections-with-less-than-2-pages) example as it will create a pages in your OneNote!)
### Get Recent Pages

[!code-csharp[](../../linqpad-samples/RecentPages.linq#L7-L18)]

### Get All Items in Recycle Bins

[!code-csharp[](../../linqpad-samples/RecycleBinItems.linq#L7-L26)]

### Search for a Page and Open Its Section

[!code-csharp[](../../linqpad-samples/OpenSection.linq#L7-L17)]

### Create a Page in a Sections With More Than 2 Pages
```csharp
var root = OneNote.GetFullHierarchy();
var section = root.Notebooks
                  .Descendants(x => x is Section { Pages.Count: >= 2 }) //Search
                  .Select(x => (Section)x)
                  .FirstOrDefault();
if(section == null)
{
    Console.WriteLine("No section found");
    return;
}
var newPage = OneNote.CreatePage(section, "New Page Name!", OpenMode.None); //or section.CreatePage

foreach (var page in newPage.BeforeSelf())
{
    Console.WriteLine(page.Name);
}
OneNote.DeleteItem(newPage, deletePermanently: true); //or newPage.Delete
```




