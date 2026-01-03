<Query Kind="Statements">
  <NuGetReference>LinqToOneNote</NuGetReference>
  <Namespace>LinqToOneNote</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

using LinqToOneNote;

var pages = OneNote.GetFullHierarchy()
                   .GetAllPages()
                   .Where(p => !p.IsInRecycleBin)
                   .OrderByDescending(p => p.LastModified)
                   .Take(5);

foreach (var page in pages)
{
    Console.WriteLine(page.Name);
}