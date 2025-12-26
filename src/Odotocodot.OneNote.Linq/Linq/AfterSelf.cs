using System;
using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
	public static partial class LinqExtensions
	{
		/// <summary>
		/// Returns a collection of sibling items that appear after the specified <paramref name="origin"/> in its parent.
		/// </summary>
		/// <param name="origin">The item for which to retrieve the following sibling items of.</param>
		/// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; containing all the following sibling items of the <paramref name="origin"/>.</returns>
		public static IEnumerable<IOneNoteItem> AfterSelf(this IOneNoteItem origin)
		{
			Throw.IfNull(origin);
			var parent = origin.Parent;
			if (parent != null)
			{
				return InternalAfterSelf(origin, parent.Children);
			}

			if (origin is Notebook { root: not null } notebook)
			{
				return InternalAfterSelf(notebook, notebook.root.notebooks);
			}

			return [];
		}

		/// <summary>
		/// Returns a filtered collection of sibling items that appear after the specified <paramref name="origin"/> and satisfy the given <paramref name="predicate"/>.
		/// </summary>
		/// <param name="origin">The item for which to retrieve the following sibling items of.</param>
		/// <param name="predicate">The predicate to filter the following sibling items.</param>
		/// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; containing all the following sibling items of the <paramref name="origin"/> that pass the <paramref name="predicate"/>.</returns>
		public static IEnumerable<IOneNoteItem> AfterSelf(this IOneNoteItem origin, Func<IOneNoteItem, bool> predicate)
		{
			Throw.IfNull(origin);
			Throw.IfNull(predicate);
			var parent = origin.Parent;
			if (parent != null)
			{
				return InternalAfterSelf(origin, parent.Children, predicate);
			}

			if (origin is Notebook { root: not null } notebook)
			{
				return InternalAfterSelf(notebook, notebook.root.notebooks, predicate);
			}

			return [];
		}

		private static IEnumerable<T> InternalAfterSelf<T>(this T origin, IReadOnlyList<T> collection, Func<T, bool> predicate = null) where T : class, IOneNoteItem
		{
			var canYield = false;
			foreach (var current in collection)
			{

				if (canYield && (predicate == null || predicate(current)))
				{
					yield return current;
				}

				if (!canYield && origin == current)
				{
					canYield = true;
				}
			}
		}
	}
}