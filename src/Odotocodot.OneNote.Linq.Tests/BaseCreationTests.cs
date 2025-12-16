using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Odotocodot.OneNote.Linq.Abstractions;

namespace Odotocodot.OneNote.Linq.Tests
{
    //Used when a class Test class creates items
    //NOTE: Needs a pre-created notebook named "TempNotebook" with a section named "Section"
    public abstract class BaseCreationTests
    {
        // <one:Notebook name="TempNotebook" nickname="TempNotebook" ID="{218DDC0A-B9A1-4C4A-A868-A54BD15ABBFE}{1}{B0}" path="C:\Users\User\Documents\OneNote Notebooks\TempNotebook" lastModifiedTime="2025-12-14T13:20:55.000Z" color="#BA7575">
        //    <one:Section name="Section" ID="{5EFE88C4-7913-4B88-A777-4C5855D61696}{1}{B0}" path="C:\Users\User\Documents\OneNote Notebooks\TempNotebook\Section.one" lastModifiedTime="2025-12-14T13:20:55.000Z" color="#8AA8E4" />
        // </one:Notebook>
        protected Notebook notebook;
        protected Section section;
        private Random random;
        private readonly List<string> createdIds = [];
        private readonly List<(string id, string path)> createdNotebooks = [];

        protected const string ExpectedWildcardPattern = "*names cannot empty, only whitespace or contain the symbols";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            OneNote.InitComObject();
            notebook = OneNote.Partial.GetHierarchy(HierarchyScope.Sections).Notebooks.First(nb => nb.Name == "TempNotebook");
            section = notebook.Sections.First(s => s.Name == "Section");
            random = new Random();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            OneNote.InitComObject();
            for (var i = 0; i < createdIds.Count; i++)
            {
                var id = createdIds[i];
                try
                {
                    OneNote.ComObject.DeleteHierarchy(id, deletePermanently: true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            createdIds.Clear();
            for (var i = 0; i < createdNotebooks.Count; i++)
            {
                var (notebookId, notebookPath) = createdNotebooks[i];
                try
                {
                    OneNote.ComObject.CloseNotebook(notebookId, true);
                    Directory.Delete(notebookPath, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            createdNotebooks.Clear();
            OneNote.ReleaseComObject();
        }

        protected static string GenerateName() => Guid.NewGuid().ToString();
        protected string GenerateInvalidName<T>() where T : INameInvalidCharacters
        {
            var name = GenerateName();
            for (int i = 0; i < 3; i++)
            {
                name = name.Insert(random.Next(0, name.Length), T.InvalidCharacters[random.Next(0, T.InvalidCharacters.Count)].ToString());
            }
            return name;
        }
        protected void TrackCreatedItem(IOneNoteItem item)
        {
            switch (item)
            {
                case null:
                    return;
                case Notebook notebook:
                    createdNotebooks.Add((notebook.Id, notebook.Path));
                    break;
                default:
                    createdIds.Add(item.Id);
                    break;
            }
        }
    }
}