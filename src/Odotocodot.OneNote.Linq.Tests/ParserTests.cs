using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Odotocodot.OneNote.Linq.Parsers;
using Shouldly;

namespace Odotocodot.OneNote.Linq.Tests
{
	[TestFixture(typeof(XmlParserXElement))]
	[TestFixture(typeof(XmlParserXmlReader))]
	internal class ParserTests<TXmlParser> where TXmlParser : IXmlParser
	{
		private Root root;
		private TXmlParser xmlParser;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			xmlParser = Activator.CreateInstance<TXmlParser>();
			var xml = File.ReadAllText(@"Input.xml");
			root = xmlParser.ParseRoot(xml);
		}

		[Test]
		[TestCase(typeof(Notebook), 4)]
		[TestCase(typeof(SectionGroup), 7)]
		[TestCase(typeof(Section), 20 + 2)]
		[TestCase(typeof(Page), 28 + 3)]
		//TODO: Move to LinqTests
		public void FulHierarchy_CorrectNumberOfItems(Type itemType, int expectedCount)
		{
			var items = root.Descendants(item => item.GetType() == itemType);

			items.Count().ShouldBe(expectedCount);
		}

		[Test]
		public void Notebook_Properties()
		{
			var notebook = root.Notebooks[0];
			var expectedNotebook = new Notebook
			{
				Name = "Its A Notebook",
				Id = "{81B591F0-CB49-4F8C-BFB1-98DA213B93FC}{1}{B0}",
				DisplayName = "It's A Notebook",
				Path = @"C:\Users\User\Desktop\Its A Notebook",
				LastModified = new DateTime(2023, 10, 04, 15, 15, 45),
				Color = ColorTranslator.FromHtml("#EE9597"),
				IsUnread = false,
				sections = notebook.sections,
				sectionGroups = notebook.sectionGroups
			};
			notebook.ShouldBeEquivalentTo(expectedNotebook);
		}


		[Test]
		public void Notebook_ChildrenCount()
		{
			var notebook = root.Notebooks[0];

			notebook.sections.Count.ShouldBe(4);
			notebook.sectionGroups.Count.ShouldBe(1);
			notebook.Children.Count.ShouldBe(5);
		}

		[Test]
		public void SectionGroup_Properties()
		{
			var sectionGroup = root.Notebooks[3].SectionGroups[1];
			var expectedSectionGroup = new SectionGroup
			{
				Name = "Section Group 1",
				Id = "{C55815E0-8F65-4790-8408-2E2C1EC74AB2}{1}{B0}",
				Path = @"C:\Users\User\Documents\OneNote Notebooks\Test Notebook\Section Group 1",
				LastModified = new DateTime(2023, 10, 04, 20, 48, 19),
				IsUnread = false,
				sections = sectionGroup.sections,
				sectionGroups = sectionGroup.sectionGroups,
				Parent = root.Notebooks[3],
				IsRecycleBin = false,
			};
			sectionGroup.ShouldBeEquivalentTo(expectedSectionGroup);
		}

		[Test]
		public void SectionGroup_ChildrenCount()
		{
			var sectionGroup = root.Notebooks[3].SectionGroups[1];
			sectionGroup.Sections.Count.ShouldBe(4);
			sectionGroup.SectionGroups.Count.ShouldBe(1);
			sectionGroup.Children.Count.ShouldBe(5);
		}

		[Test]
		[TestCaseSource(nameof(Section_Properties_Cases))]
		public void Section_Properties(PropertiesTestCase<Section> testCase)
		{
			var (section, expected) = testCase.GetData(root);

			section.ShouldBeEquivalentTo(expected);
		}

		private static IEnumerable<PropertiesTestCase<Section>> Section_Properties_Cases()
		{
			yield return new(root => root.Notebooks[3].Sections[3],
			root => new Section
			{
				Name = "Locked Section",
				Id = "{6BB816F6-D431-4430-B7A2-F9DEB7A28F67}{1}{B0}",
				Path = @"C:\Users\User\Documents\OneNote Notebooks\Test Notebook\Locked Section.one",
				LastModified = new DateTime(2023, 06, 17, 11, 00, 52),
				Parent = root.Notebooks[3],
				Locked = true,
				Encrypted = true,
				Color = ColorTranslator.FromHtml("#BA7575"),
				IsInRecycleBin = false,
				IsDeletedPages = false,
			});
			yield return new(root => root.OpenSections.Sections[1],
			root => new Section
			{
				Name = "If a section is open in OneNote is it an OpenSection",
				Id = "{5A75942A-9BA8-0C6B-01C5-E53DF7ED726F}{1}{B0}",
				Path = @"C:\Users\User\Desktop\Another Folder\If a section is open in OneNote is it an OpenSection.one",
				LastModified = new DateTime(2025, 11, 26, 17, 28, 35),
				Color = ColorTranslator.FromHtml("#9BBBD2"),
				IsInRecycleBin = false,
				IsDeletedPages = false,
				Locked = false,
				Encrypted = false,
				Parent = null,
			});
		}

		[Test]
		[TestCase(3, 3, 0)]
		[TestCase(3, 0, 3)]
		public void Section_ChildrenCount(int i1, int i2, int expectedCount)
		{
			var section = root.Notebooks[i1].Sections[i2];
			section.Children.Count.ShouldBe(expectedCount);
		}

		[Test]
		[TestCaseSource(nameof(Page_Properties_Cases))]
		public void Page_Properties(PropertiesTestCase<Page> testCase)
		{
			var (page, expected) = testCase.GetData(root);

			page.ShouldBeEquivalentTo(expected);
		}
		private static IEnumerable<PropertiesTestCase<Page>> Page_Properties_Cases()
		{
			//one:Notebooks/one:Notebook[4]/one:SectionGroup[2]/one:SectionGroup/one:SectionGroup/one:SectionGroup/one:Section/one:Page
			yield return new(root => root.Notebooks[3].SectionGroups[1].SectionGroups[0].SectionGroups[0].Sections[0].Pages[0],
			root => new Page
			{
				Name = "Very nested",
				Id = "{748017F5-15E8-40D3-A1FF-2DCEF2D7A895}{1}{E19558058794535511298120172155500410846478691}",
				LastModified = new DateTime(2023, 06, 06, 14, 24, 11),
				Created = new DateTime(2023, 06, 06, 14, 23, 56),
				Parent = root.Notebooks[3].SectionGroups[1].SectionGroups[0].SectionGroups[0].Sections[0],
				Level = 1,
				IsUnread = false,
				IsInRecycleBin = false,
			});
			yield return new(root => root.Notebooks[0].Sections[0].Pages[0],
			root => new Page
			{
				Name = "Important Info",
				Id = "{1B9CDD3C-6836-4DC6-9C44-0EDC06A9B8CB}{1}{E19481616267573963101920151005250203326127411}",
				IsUnread = true,
				Parent = root.Notebooks[0].Sections[0],
				IsInRecycleBin = false,
				Created = new DateTime(2022, 12, 01, 18, 10, 02),
				LastModified = new DateTime(2022, 12, 01, 18, 10, 34),
				Level = 1,
			});
		}

		[Test]
		public void OpenSections_Properties()
		{
			var openSections = root.OpenSections;
			var expectedOpenSections = new OpenSections
			{
				Id = "{2CFD5279-E2F3-4544-9878-0F1CB3609489}{1}{B0}",
				sections = openSections.sections,
			};
			openSections.ShouldBeEquivalentTo(expectedOpenSections);
		}

		[Test]
		public void OpenSections_ChildrenCount()
		{
			var openSections = root.OpenSections;
			openSections.Sections.Count.ShouldBe(2);
		}

		public class PropertiesTestCase<T>(Func<Root, T> getter, Func<Root, T> expectedGetter)
		{
			public (T value, T expected) GetData(Root root) => (getter(root), expectedGetter(root));
		}
	}
}