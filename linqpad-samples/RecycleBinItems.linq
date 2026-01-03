<Query Kind="Statements">
  <NuGetReference>LinqToOneNote</NuGetReference>
  <Namespace>LinqToOneNote</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

using LinqToOneNote;

var items = OneNote.GetFullHierarchy()
                   .Notebooks
                   .Descendants(i => i.IsInRecycleBin()) // use an extension method to check if the item is in the recycle bin
                   .Where(i => i switch
                   {
	                   // skip the special recycle bin section group
	                   SectionGroup { IsRecycleBin: true } => false,
	                   // skip the special deleted pages section in a recycle bin
	                   Section { IsDeletedPages: true } => false, 
	                   _ => true,
                   })
                   .ToArray();

Console.WriteLine($"Number of items in recycle bins: {items.Length}");
foreach (var item in items)
{
	Console.WriteLine($"Name: {item.Name} | Parent Name: {item.Parent.Name}");
}