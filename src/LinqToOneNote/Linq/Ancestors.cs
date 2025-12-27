using System;
using System.Collections.Generic;
using LinqToOneNote.Internal;

namespace LinqToOneNote
{
	public static partial class LinqExtensions
	{
		/// <summary>
		/// Returns a collection of the ancestor elements of the specified <paramref name="origin"/>.
		/// </summary>
		/// <param name="origin">The item for which to retrieve the ancestor elements of.</param>
		/// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; containing all the ancestor elements of the <paramref name="origin"/>.</returns>
		public static IEnumerable<IOneNoteItem> Ancestors(this IOneNoteItem origin)
		{
			Throw.IfNull(origin);
			return InternalAncestors(origin);
		}

		/// <summary>
		/// Returns a collection of the ancestor elements of the specified <paramref name="origin"/> that satisfy the given <paramref name="predicate"/>.
		/// </summary>
		/// <param name="origin">The item for which to retrieve the ancestor elements of.</param>
		/// <param name="predicate">The predicate to filter the ancestor elements.</param>
		/// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; containing all the ancestor elements of the <paramref name="origin"/> that pass the <paramref name="predicate"/>.</returns>
		public static IEnumerable<IOneNoteItem> Ancestors(this IOneNoteItem origin, Func<IOneNoteItem, bool> predicate)
		{
			Throw.IfNull(origin);
			return InternalAncestors(origin, predicate);
		}

		private static IEnumerable<IOneNoteItem> InternalAncestors(this IOneNoteItem origin, Func<IOneNoteItem, bool> predicate = null)
		{
			var current = origin.Parent;
			while (current != null)
			{
				if (predicate == null || predicate(current))
					yield return current;
				current = current.Parent;
			}
		}
	}
}