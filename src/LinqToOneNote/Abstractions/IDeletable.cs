namespace LinqToOneNote.Abstractions
{
	/// <summary>
	/// Indicates that the OneNote item can be deleted with <see cref="OneNote.DeleteItem"/>.
	/// </summary>
	public interface IDeletable : IOneNoteItem;
}