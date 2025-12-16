using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using NUnit.Framework;

namespace Odotocodot.OneNote.Linq.Tests
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
        public void CreateSectionGroup()
        {
            var newSectionGroup = OneNote.CreateSectionGroup(notebook, GenerateName());
            Check(newSectionGroup, notebook);
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