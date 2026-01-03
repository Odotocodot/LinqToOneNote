using System;
using System.IO;
using System.Linq;
using AwesomeAssertions;
using NUnit.Framework;
using LinqToOneNote.Parsers;

namespace LinqToOneNote.Tests
{
	[TestFixture(typeof(XmlParserXElement))]
	[TestFixture(typeof(XmlParserXmlReader))]
	internal class LinqTests<TXmlParser> where TXmlParser : IXmlParser
	{
		private Root root;
		private TXmlParser xmlParser;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			xmlParser = Activator.CreateInstance<TXmlParser>();
			var xml = File.ReadAllText("Input.xml");
			root = xmlParser.ParseRoot(xml);
		}

		[Test]
		[TestCase(typeof(Notebook), 4)]
		[TestCase(typeof(SectionGroup), 7)]
		[TestCase(typeof(Section), 20 + 2)] // + 2 is from OpenSections
		[TestCase(typeof(Page), 28 + 3)] // + 3 is from OpenSections
		public void Descendants_CorrectNumberOfItems(Type itemType, int expectedCount)
		{
			var items = root.Descendants(item => item.GetType() == itemType);

			items.Should().HaveCount(expectedCount);
		}

		[Test]
		public void GetAllPages_CorrectCount()
		{
			var pages = root.GetAllPages();

			pages.Should().HaveCount(28 + 3);
		}

		[Test]
		public void AfterSelf_CorrectCount()
		{
			var notebook = root.Notebooks[1];
			notebook.AfterSelf().ToArray().Should().HaveCount(2);
		}

		[Test]
		public void BeforeSelf_CorrectCount()
		{
			//one:Notebooks/one:Notebook[4]/one:SectionGroup[1]
			var sectionGroup = root.Notebooks[3].SectionGroups[0];
			sectionGroup.BeforeSelf().Should().HaveCount(4);
		}

		[Test]
		public void Ancestors_CorrectCount()
		{
			var page = root.Notebooks[3].SectionGroups[1].SectionGroups[0].SectionGroups[0].SectionGroups[0].Sections[0].Pages[0];
			page.Ancestors().Should().HaveCount(6);
		}
	}
}