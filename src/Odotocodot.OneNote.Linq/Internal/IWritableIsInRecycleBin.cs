using Odotocodot.OneNote.Linq.Abstractions;

namespace Odotocodot.OneNote.Linq.Internal
{
	internal interface IWritableIsInRecycleBin : IHasIsInRecycleBin
	{
		new bool IsInRecycleBin { set; }
	}
}