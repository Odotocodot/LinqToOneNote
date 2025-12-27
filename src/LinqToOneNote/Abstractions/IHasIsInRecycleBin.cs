namespace LinqToOneNote.Abstractions
{
	/// <summary>
	/// Represents an OneNote hierarchy item that can be <b>in</b> a OneNote recycle bin. <br/>
	/// For ease, use <see cref="Extensions.IsInRecycleBin"/> instead.
	/// </summary>
	/// <seealso cref="Section"/>
	/// <seealso cref="Page"/>
	/// <seealso cref="Extensions.IsInRecycleBin"/>
	public interface IHasIsInRecycleBin : IOneNoteItem
	{
		/// <summary>
		/// Indicates whether the item is in recycle bin.
		/// </summary>
		bool IsInRecycleBin { get; }
	}
}