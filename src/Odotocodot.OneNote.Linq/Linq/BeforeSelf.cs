using System;
using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
	public static partial class LinqExtensions
	{
		//Returns a collection of the sibling nodes before this node, in document order.
		/// <summary>
		/// Returns a collection of sibling items that appear before the specified <paramref name="origin"/> in its parent.
		/// </summary>
		/// <param name="origin">The item for which to retrieve the preceding sibling items of.</param>
		/// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; containing all the preceding sibling items of the <paramref name="origin"/>.</returns>
		public static IEnumerable<IOneNoteItem> BeforeSelf(this IOneNoteItem origin)
		{
			Throw.IfNull(origin);
			var parent = origin.Parent;
			if (parent != null)
			{
				return InternalBeforeSelf(origin, parent.Children);
			}

			if (origin is Notebook { root: not null } notebook)
			{
				return InternalBeforeSelf(notebook, notebook.root.notebooks);
			}

			return [];
		}

		/// <summary>
		/// Returns a filtered collection of sibling items that appear before the specified <paramref name="origin"/> and satisfy the given <paramref name="predicate"/>.
		/// </summary>
		/// <param name="origin">The item for which to retrieve the preceding sibling items of.</param>
		/// <param name="predicate">The predicate to filter the preceding sibling items.</param>
		/// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; containing all the preceding sibling items of the <paramref name="origin"/> that pass the <paramref name="predicate"/>.</returns>
		public static IEnumerable<IOneNoteItem> BeforeSelf(this IOneNoteItem origin, Func<IOneNoteItem, bool> predicate)
		{
			Throw.IfNull(origin);
			Throw.IfNull(predicate);
			var parent = origin.Parent;
			if (parent != null)
			{
				return InternalBeforeSelf(origin, parent.Children, predicate);
			}

			if (origin is Notebook { root: not null } notebook)
			{
				return InternalBeforeSelf(notebook, notebook.root.notebooks, predicate);
			}

			return [];
		}

		private static IEnumerable<T> InternalBeforeSelf<T>(this T origin, IReadOnlyList<T> collection, Func<T, bool> predicate = null) where T : class, IOneNoteItem
		{
			foreach (var current in collection)
			{
				if (current == origin)
					yield break;
				if (predicate == null || predicate(current))
					yield return current;
			}
		}
	}
}