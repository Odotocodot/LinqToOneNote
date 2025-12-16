using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AwesomeAssertions;
using NUnit.Framework;

namespace Odotocodot.OneNote.Linq.Tests
{
    //NOTE: Needs a pre-created notebook named "TempNotebook" with a section named "Section"
    [TestFixture]
    public class CreationTests
    {
        // <one:Notebook name="TempNotebook" nickname="TempNotebook" ID="{218DDC0A-B9A1-4C4A-A868-A54BD15ABBFE}{1}{B0}" path="C:\Users\User\Documents\OneNote Notebooks\TempNotebook" lastModifiedTime="2025-12-14T13:20:55.000Z" color="#BA7575">
        // <one:Section name="Section" ID="{5EFE88C4-7913-4B88-A777-4C5855D61696}{1}{B0}" path="C:\Users\User\Documents\OneNote Notebooks\TempNotebook\Section.one" lastModifiedTime="2025-12-14T13:20:55.000Z" color="#8AA8E4" />
        // </one:Notebook>

        private Notebook notebook;
        private Section section;
        private readonly List<string> createdIds = new();
        private readonly List<(string id, string path)> createdNotebooks = new();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            OneNote.InitComObject();
            notebook = OneNote.Partial.GetHierarchy(HierarchyScope.Sections).Notebooks.First(nb => nb.Name == "TempNotebook");
            section = notebook.Sections.First(s => s.Name == "Section");
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

        private static string GenerateName() => Guid.NewGuid().ToString();

        [Test]
        public void CreatePage()
        {
            Page newPage = OneNote.CreatePage(section, GenerateName());
            Create(newPage, section);
        }

        [Test]
        public void CreateSection()
        {
            var newSection = OneNote.CreateSection(notebook, GenerateName());
            Create(newSection, notebook);
        }

        [Test]
        public void CreateSectionGroup()
        {
            var newSectionGroup = OneNote.CreateSectionGroup(notebook, GenerateName());
            Create(newSectionGroup, notebook);
        }

        [Test]
        public void CreateNotebook()
        {
            var newNotebook = OneNote.CreateNotebook(GenerateName());
            createdNotebooks.Add((newNotebook.Id, newNotebook.Path));

            var notebooks = OneNote.Partial.GetHierarchy(HierarchyScope.Notebooks).Notebooks;

            var expected = notebooks.FirstOrDefault(x => x.Id == newNotebook.Id);

            expected.Should().NotBeNull();
            newNotebook.Should().BeEquivalentTo(expected, options => options.WithoutRecursing());
        }

        private void Create<T, TParent>(T newItem, TParent parent) where T : IOneNoteItem where TParent : IOneNoteItem
        {
            createdIds.Add(newItem.Id);

            var children = OneNote.Partial.GetChildren(parent);

            var expected = children.FirstOrDefault(x => x.Id == newItem.Id);

            expected.Should().NotBeNull();
            newItem.Should().BeEquivalentTo(expected, options => options.WithoutRecursing().ExcludingMembersNamed(nameof(IOneNoteItem.Parent)));
            newItem.Parent.Should().BeEquivalentTo(parent, options => options.WithoutRecursing().ExcludingMembersNamed(nameof(IOneNoteItem.Parent)));
        }
    }
}