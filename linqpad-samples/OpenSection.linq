<Query Kind="Statements">
  <NuGetReference>LinqToOneNote</NuGetReference>
  <Namespace>LinqToOneNote</Namespace>
  <IncludeUncapsulator>false</IncludeUncapsulator>
</Query>

using LinqToOneNote;

var page = OneNote.FindPages("This specific search").MaxBy(p => p.LastModified);
if(page == null)
{
    Console.WriteLine("No page found with that search, try changing it!");
}
else
{
    OneNote.Open(page.Parent); //or page.Parent.Open()
}