namespace Odotocodot.OneNote.Linq.Abstractions
{
	/// <summary>
	/// Indicates that the OneNote item can be deleted with <see cref="OneNote.DeleteItem(IDeletable)"/>.
	/// </summary>
	public interface IDeletable : IOneNoteItem {}
}