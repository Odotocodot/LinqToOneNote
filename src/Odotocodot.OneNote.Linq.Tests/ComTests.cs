using System.Runtime.InteropServices;
using NUnit.Framework;
using Shouldly;

namespace Odotocodot.OneNote.Linq.Tests
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

			Should.NotThrow(OneNote.ReleaseComObject);
		}

		[Test]
		public void HasComObject_WhenNotInit_ReturnsFalse()
		{
			OneNote.HasComObject.ShouldBeFalse();
		}

		[Test]
		public void ComObjectMode_Lazy()
		{
			OneNote.SetComObjectMode(ComObjectMode.Lazy);
			OneNote.HasComObject.ShouldBeFalse();

			Should.NotThrow(OneNote.GetDefaultNotebookLocation);

			OneNote.HasComObject.ShouldBeTrue();
		}

		[Test]
		public void HasComObject_WhenInit()
		{
			OneNote.InitComObject();

			OneNote.HasComObject.ShouldBeTrue();
		}

		[Test]
		public void HasComObject_WhenInitAndRelease()
		{
			OneNote.InitComObject();
			OneNote.ReleaseComObject();

			OneNote.HasComObject.ShouldBeFalse();
		}

		[Test]
		public void HasComObject_WithComObjectModeWrap()
		{
			OneNote.SetComObjectMode(ComObjectMode.Wrap);

			OneNote.HasComObject.ShouldBeFalse();

			Should.NotThrow(OneNote.GetDefaultNotebookLocation);

			OneNote.HasComObject.ShouldBeFalse();
		}

		[Test]
		public void ComObjectMode_Manual()
		{
			OneNote.SetComObjectMode(ComObjectMode.Manual);
			OneNote.HasComObject.ShouldBeFalse();

			Should.Throw<InvalidComObjectException>(OneNote.GetDefaultNotebookLocation);
		}
	}
}