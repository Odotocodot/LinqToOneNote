using Odotocodot.OneNote.Linq.Abstractions;
using System.Drawing;

namespace Odotocodot.OneNote.Linq.Internal
{
	internal interface IWritableColor : IHasColor
	{
		new Color? Color { set; }
	}
}