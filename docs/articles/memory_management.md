# Memory Management

A COM object is required to use the OneNote Interop API, by default this is acquired lazily (see [ComObjectMode](xref:LinqToOneNote.ComObjectMode)), i.e. the first time you call a method that requires a COM object, the library gets one.

However, acquiring a COM object is _slow_ and once retrieved, it is visible in the Task Manager as shown below.

![task manager screenshot](~/images/task_manager.png)

If you want to choose when this operation occurs, you can call `OneNote.InitComObject()` to forcible acquire the COM object (it does nothing if one has already been attained).

To free up the memory that the COM object takes up, rather than wait for your application to exit you can call  `OneNote.ReleaseComObject()`.

See below for an example.

```csharp
//Get the COM object
OneNote.InitComObject();

//Do stuff e.g.
var notebooks = OneNote.GetFullHierarchy().Notebooks;

foreach (var notebook in notebooks)
{
    Console.WriteLine(notebook.Name)
}

var pages = notebooks.Descendants(n => n.Children.Count > 3).GetAllPages();

foreach (var page in pages)
{
    Console.WriteLine(page.Parent.Name);
}

//Release the COM object to free memory
OneNote.ReleaseComObject()
```

Alternatively you can set `OneNote.ComObjectMode` to `ComObjectMode.Wrap`. This changes to the [](xref:LinqToOneNote.OneNote) class functionality to essentially call `OneNote.ReleaseComObject()` after every method that uses the COM, useful for one-off operations.

```csharp
OneNote.SetComObjectMode(ComObjectMode.Wrap);
Console.WriteLine(OneNote.HasComObject); //false
var path = OneNote.GetDefaultNotebookLocation();
Console.WriteLine(OneNote.HasComObject); //false


OneNote.SetComObjectMode(ComObjectMode.Lazy);
Console.WriteLine(OneNote.HasComObject); //false
var path = OneNote.GetDefaultNotebookLocation();
Console.WriteLine(OneNote.HasComObject); //true
```
