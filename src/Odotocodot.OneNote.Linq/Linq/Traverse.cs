using System;
using System.Collections.Generic;
using System.Linq;

namespace Odotocodot.OneNote.Linq
{
	public static partial class LinqExtensions
	{
        /// <summary>
        /// Returns a flattened collection of OneNote items, which contain the children of every OneNote item from the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source OneNote item.</param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; containing the 
        /// child items of the <paramref name="source"/>.</returns>
        /// <remarks>This method uses a non-recursive depth first traversal algorithm.</remarks>
        public static IEnumerable<IOneNoteItem> Traverse(this IOneNoteItem source)
        {
            var stack = new Stack<IOneNoteItem>();
            stack.Push(source);
            while (stack.Count > 0)
            {
                var current = stack.Pop();

                yield return current;

                foreach (var child in current.Children)
                    stack.Push(child);
            }
        }

        /// <summary>
        /// Returns a filtered flattened collection of OneNote items, which contain the children of every OneNote item 
        /// from the <paramref name="source"/>.<br/>
        /// Only items that successfully pass the <paramref name="predicate"/> are returned.
        /// </summary>
        /// <param name="source"><inheritdoc cref="Traverse(IOneNoteItem)" path="/param[@name='source']"/></param>
        /// <param name="predicate">A function to test each item for a condition.</param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; containing the 
        /// child items of the <paramref name="source"/> that pass the <paramref name="predicate"/>.</returns>
        /// <remarks><inheritdoc cref="Traverse(IOneNoteItem)" path="/remarks"/></remarks>
        public static IEnumerable<IOneNoteItem> Traverse(this IOneNoteItem source, Func<IOneNoteItem, bool> predicate)
        {
            var stack = new Stack<IOneNoteItem>();
            stack.Push(source);
            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (predicate(current))
                    yield return current;

                foreach (var child in current.Children)
                    stack.Push(child);
            }
        }

        /// <inheritdoc cref="Traverse(IOneNoteItem)"/>
        public static IEnumerable<IOneNoteItem> Traverse(this IEnumerable<IOneNoteItem> source)
            => source.SelectMany(item => item.Traverse());

        /// <inheritdoc cref="Traverse(IOneNoteItem,Func{IOneNoteItem, bool})"/>
        public static IEnumerable<IOneNoteItem> Traverse(this IEnumerable<IOneNoteItem> source, Func<IOneNoteItem, bool> predicate)
            => source.SelectMany(item => item.Traverse(predicate));
    }
}