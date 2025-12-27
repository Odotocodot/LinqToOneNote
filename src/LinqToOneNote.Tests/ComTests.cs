using System.Runtime.InteropServices;
using AwesomeAssertions;
using NUnit.Framework;
using static AwesomeAssertions.FluentActions;

namespace LinqToOneNote.Tests
{
	[TestFixture]
	[TestOf(typeof(OneNote))]
	public class ComTests
	{
		[SetUp]
		public void Setup()
		{
			OneNote.ReleaseComObject();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			OneNote.ReleaseComObject();
		}

		[Test]
		public void ReleaseComObject_WhenInit_DoesNotThrowException()
		{
			OneNote.InitComObject();

			Invoking(OneNote.ReleaseComObject).Should().NotThrow();
		}

		[Test]
		public void ComObjectMode_Lazy()
		{
			OneNote.SetComObjectMode(ComObjectMode.Lazy);
			OneNote.HasComObject.Should().BeFalse();

			Invoking(OneNote.GetDefaultNotebookLocation).Should().NotThrow();

			OneNote.HasComObject.Should().BeTrue();
		}

		[Test]
		public void HasComObject_WhenInit()
		{
			OneNote.InitComObject();

			OneNote.HasComObject.Should().BeTrue();
		}

		[Test]
		public void HasComObject_WhenInitAndRelease()
		{
			OneNote.InitComObject();
			OneNote.ReleaseComObject();

			OneNote.HasComObject.Should().BeFalse();
		}

		[Test]
		public void ComObjectMode_Wrap()
		{
			OneNote.SetComObjectMode(ComObjectMode.Wrap);

			OneNote.HasComObject.Should().BeFalse();

			Invoking(OneNote.GetDefaultNotebookLocation).Should().NotThrow();

			OneNote.HasComObject.Should().BeFalse();
		}

		[Test]
		public void ComObjectMode_Manual()
		{
			OneNote.SetComObjectMode(ComObjectMode.Manual);
			OneNote.HasComObject.Should().BeFalse();

			Invoking(OneNote.GetDefaultNotebookLocation).Should().Throw<InvalidComObjectException>();
		}
	}
}