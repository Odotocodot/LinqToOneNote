using System;
using System.IO;
using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using NUnit.Framework;
using static AwesomeAssertions.FluentActions;

namespace LinqToOneNote.Tests
{
    [TestFixture]
    public class CreationTests : BaseCreationTests
    {

        [Test]
        public void CreatePage()
        {
            Page newPage = OneNote.CreatePage(section, GenerateName());
            Check(newPage, section);
        }

        [Test]
        public void CreateSection()
        {
            var newSection = OneNote.CreateSection(notebook, GenerateName());
            Check(newSection, notebook);
        }

        [Test]
        public void CreateSection_InvalidName()
        {
            var invalidName = GenerateInvalidName<Section>();
            Invoking(() =>
            {
                var section = OneNote.CreateSection(notebook, invalidName);
                TrackCreatedItem(section);
                return section;
            }).Should().Throw<ArgumentException>().WithMessage(ExpectedWildcardPattern);
        }

        [Test]
        public void CreateSectionGroup()
        {
            var newSectionGroup = OneNote.CreateSectionGroup(notebook, GenerateName());
            Check(newSectionGroup, notebook);
        }

        [Test]
        public void CreateSectionGroup_InvalidName()
        {
            var invalidName = GenerateInvalidName<SectionGroup>();
            Invoking(() =>
            {
                var sectionGroup = OneNote.CreateSectionGroup(notebook, invalidName);
                TrackCreatedItem(sectionGroup);
                return sectionGroup;
            }).Should().Throw<ArgumentException>().WithMessage(ExpectedWildcardPattern);
        }

        [Test]
        public void CreateNotebook()
        {
            var newNotebook = OneNote.CreateNotebook(GenerateName());

            TrackCreatedItem(newNotebook);

            var notebooks = OneNote.Partial.GetHierarchy(HierarchyScope.Notebooks).Notebooks;

            var expected = notebooks.FirstOrDefault(x => x.Id == newNotebook.Id);

            using var scope = new AssertionScope();
            expected.Should().NotBeNull();
            newNotebook.Should().BeEquivalentTo(expected, options => options.WithoutRecursing());
        }

        [Test]
        public void CreateNotebook_InDirectory()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), GenerateName());
            Directory.CreateDirectory(tempDir);
            try
            {
                var newNotebook = OneNote.CreateNotebook(GenerateName(), tempDir);

                TrackCreatedItem(newNotebook);

                var notebooks = OneNote.Partial.GetHierarchy(HierarchyScope.Notebooks).Notebooks;

                var expected = notebooks.FirstOrDefault(x => x.Id == newNotebook.Id);

                using var scope = new AssertionScope();
                expected.Should().NotBeNull();
                newNotebook.Path.Should().StartWith(tempDir);
                newNotebook.Should().BeEquivalentTo(expected, options => options.WithoutRecursing());
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Test]
        public void CreateNotebook_InvalidDirectory()
        {
            Invoking(() =>
            {
                var notebook = OneNote.CreateNotebook(GenerateName(), $"C:\\{GenerateName()}\\DoesNotExist\\{GenerateName()}");
                TrackCreatedItem(notebook);
                return notebook;
            }).Should().Throw<IOException>();
        }

        [Test]
        public void CreateNotebook_InvalidName()
        {
            var invalidName = GenerateInvalidName<Notebook>();
            Invoking(() =>
            {
                var notebook = OneNote.CreateNotebook(invalidName);
                TrackCreatedItem(notebook);
                return notebook;
            }).Should().Throw<ArgumentException>().WithMessage(ExpectedWildcardPattern);
        }

        private void Check<T, TParent>(T newItem, TParent parent) where T : IOneNoteItem where TParent : IOneNoteItem
        {
            TrackCreatedItem(newItem);

            var children = OneNote.Partial.GetChildren(parent);

            var expected = children.FirstOrDefault(x => x.Id == newItem.Id);

            using var scope = new AssertionScope();
            expected.Should().NotBeNull();
            newItem.Should().BeEquivalentTo(expected, options => options.WithoutRecursing().ExcludingMembersNamed(nameof(IOneNoteItem.Parent)));
            newItem.Parent.Should().BeEquivalentTo(parent, options => options.WithoutRecursing().ExcludingMembersNamed(nameof(IOneNoteItem.Parent)));
        }
    }
}