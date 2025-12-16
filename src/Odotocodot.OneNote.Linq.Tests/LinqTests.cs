using System;
using System.IO;
using AwesomeAssertions;
using NUnit.Framework;
using Odotocodot.OneNote.Linq.Parsers;

namespace Odotocodot.OneNote.Linq.Tests
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
			var xml = File.ReadAllText(@"Input.xml");
			root = xmlParser.ParseRoot(xml);
		}

		[Test]
		[TestCase(typeof(Notebook), 4)]
		[TestCase(typeof(SectionGroup), 7)]
		[TestCase(typeof(Section), 20 + 2)] // + 2 is from OpenSections
		[TestCase(typeof(Page), 28 + 3)] // + 3 is from OpenSections
		public void FulHierarchy_CorrectNumberOfItems(Type itemType, int expectedCount)
		{
			var items = root.Descendants(item => item.GetType() == itemType);

			items.Should().HaveCount(expectedCount);
		}

		[Test]
		public void FullHierarchy_GetAllPages()
		{
			var pages = root.GetAllPages();

			pages.Should().HaveCount(28 + 3);
		}
	}
}