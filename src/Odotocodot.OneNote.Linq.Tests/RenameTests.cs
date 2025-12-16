using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using NUnit.Framework;
using static AwesomeAssertions.FluentActions;

namespace Odotocodot.OneNote.Linq.Tests
{
	[TestFixture]
	public class RenameTests : BaseCreationTests
	{

		[Test]
		public void Rename_Throw_Null()
		{
			Invoking(() => OneNote.RenameItem(null, "Should not matter")).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void Rename_Throw_WhiteSpaceOrEmpty()
		{
			var section = new Section();
			Invoking(() => OneNote.RenameItem(section, " ")).Should().Throw<ArgumentException>();
		}


		[Test]
		public void Rename_Page()
		{
			var page = OneNote.CreatePage(section, GenerateName());
			Rename(page);
		}

		[Test]
		public void Rename_Section_Valid()
		{
			var section = OneNote.CreateSection(notebook, GenerateName());
			Rename(section);
		}

		[Test]
		public void Rename_Section_Invalid()
		{
			var section = OneNote.CreateSection(notebook, GenerateName());
			TrackCreatedItem(section);
			Invoking(() => OneNote.RenameItem(section, GenerateInvalidName<Section>())).Should().Throw<ArgumentException>().WithMessage("*names cannot empty, only whitespace or contain the symbols");
		}

		[Test]
		public void Rename_SectionGroup_Valid()
		{
			var sectionGroup = OneNote.CreateSectionGroup(notebook, GenerateName());
			Rename(sectionGroup);
		}

		[Test]
		public void Rename_SectionGroup_Invalid()
		{
			var sectionGroup = OneNote.CreateSectionGroup(notebook, GenerateName());
			TrackCreatedItem(sectionGroup);
			Invoking(() => OneNote.RenameItem(sectionGroup, GenerateInvalidName<SectionGroup>())).Should().Throw<ArgumentException>().WithMessage("*names cannot empty, only whitespace or contain the symbols");
		}

		[Test]
		public void Rename_Notebook_Valid()
		{
			var notebook = OneNote.CreateNotebook(GenerateName());
			Rename(notebook);
		}

		[Test]
		public void Rename_Notebook_Invalid()
		{
			var notebook = OneNote.CreateNotebook(GenerateName());
			TrackCreatedItem(notebook);
			Invoking(() => OneNote.RenameItem(notebook, GenerateInvalidName<Notebook>())).Should().Throw<ArgumentException>().WithMessage("*names cannot empty, only whitespace or contain the symbols");
		}

		private void Rename<T>(T item) where T : IOneNoteItem
		{
			TrackCreatedItem(item);

			string newName = GenerateName();
			OneNote.RenameItem(item, newName);

			IReadOnlyList<IOneNoteItem> children;
			if (item is Notebook)
			{
				children = OneNote.Partial.GetHierarchy(HierarchyScope.Notebooks).Notebooks;
			}
			else
			{
				children = OneNote.Partial.GetChildren(item.Parent);
			}


			var expected = children.FirstOrDefault(x => x.Id == item.Id);

			using var scope = new AssertionScope();
			if (item is Notebook notebook)
			{
				notebook.DisplayName.Should().Be(newName);
			}
			else
			{
				item.Name.Should().Be(newName);
			}
			expected.Should().NotBeNull();
			item.Should().BeEquivalentTo(expected, options => options.WithoutRecursing().ExcludingMembersNamed(nameof(IOneNoteItem.Parent), nameof(IOneNoteItem.LastModified)));
		}
	}
}