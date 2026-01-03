namespace LinqToOneNote.Abstractions
{
	/// <summary>
	/// Represents an OneNote hierarchy item that has a file path.
	/// </summary>
	/// <seealso cref="Notebook"/>
	/// <seealso cref="SectionGroup"/>
	/// <seealso cref="Section"/>
	public interface IHasPath : IOneNoteItem
	{
		/// <summary>
		/// The full path to the item.
		/// </summary>
		string Path { get; }
	}
}