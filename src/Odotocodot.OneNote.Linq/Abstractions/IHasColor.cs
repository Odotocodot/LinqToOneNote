using System.Drawing;

namespace Odotocodot.OneNote.Linq.Abstractions
{
	/// <summary>
	/// Represents an OneNote hierarchy item that has a color.
	/// </summary>
	/// <seealso cref="Notebook"/>
	/// <seealso cref="Section"/>
	public interface IHasColor : IOneNoteItem
	{
		/// <summary>
		/// The color of the notebook.
		/// </summary>
		Color? Color { get; }
	}
}