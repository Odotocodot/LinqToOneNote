using System.Collections.Generic;

namespace LinqToOneNote
{
	/// <summary>
	/// Use to check if two <see cref="IOneNoteItem"/>s have the same <see cref="Abstractions.INavigable.Id">id</see>, i.e. they represent the same
	/// item in OneNote.
	/// </summary>
	/// <seealso cref="Extensions.ItemEquals"/>
	public class OneNoteItemEqualityComparer : IEqualityComparer<IOneNoteItem>
	{
		/// <summary>
		/// Returns a default equality comparer for <see cref="IOneNoteItem"/>s.
		/// </summary>
		public static readonly OneNoteItemEqualityComparer Default = new();
		
		/// <inheritdoc/>
		public bool Equals(IOneNoteItem x, IOneNoteItem y)
		{
			if (ReferenceEquals(x, y))
				return true;
			if (x is null || y is null)
				return false;
			return x.Id == y.Id;
		}
		
		/// <inheritdoc/>
		public int GetHashCode(IOneNoteItem obj) => obj.Id.GetHashCode();
	}
}