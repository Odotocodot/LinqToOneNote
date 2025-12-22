using System;
using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Linq extension methods for a <see cref="IOneNoteItem"/>.
    /// </summary>
    public static partial class LinqExtensions
    {
        internal static readonly SimplePool<Stack2> StackPool = new(5);
        internal class Stack2 : Stack<IOneNoteItem>, IDisposable
        {
            public void Dispose()
            {
                Clear();
                StackPool.Return(this);
            }
        }

        /// <summary>
        /// Returns a flattened collection of OneNote items, which contains the descendants of the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source OneNote item.</param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; containing all the descendants
        /// of the <paramref name="source"/>.</returns>
        /// <remarks>This method uses a non-recursive depth first traversal algorithm.</remarks>
        public static IEnumerable<IOneNoteItem> Descendants(this IOneNoteItem source)
        {
            Throw.IfNull(source);

            return Descendants();

            IEnumerable<IOneNoteItem> Descendants()
            {
                using var stack = StackPool.Rent();
                stack.Push(source);
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    yield return current;
                    foreach (var child in current.Children)
                    {
                        stack.Push(child);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a filtered flattened collection of OneNote items, which contains the descendants of the <paramref name="source"/>.<br/>
        /// Only items that successfully pass the <paramref name="predicate"/> are returned.
        /// </summary>
        /// <param name="source">The source OneNote item.</param>
        /// <param name="predicate">The predicate to filter the descendants.</param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; containing all the descendants
        /// of the <paramref name="source"/> that pass the <paramref name="predicate"/>.</returns>
        /// <remarks><inheritdoc cref="Traverse(IOneNoteItem)" path="/remarks"/></remarks>
        public static IEnumerable<IOneNoteItem> Descendants(this IOneNoteItem source, Func<IOneNoteItem, bool> predicate)
        {
            Throw.IfNull(source);
            Throw.IfNull(predicate);

            return Descendants();

            IEnumerable<IOneNoteItem> Descendants()
            {
                using var stack = StackPool.Rent();
                stack.Push(source);
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    if (predicate(current))
                        yield return current;
                    foreach (var child in current.Children)
                    {
                        stack.Push(child);
                    }
                }
            }
        }

        /// <inheritdoc cref="Descendants(IOneNoteItem)"/>
        public static IEnumerable<IOneNoteItem> Descendants(this IEnumerable<IOneNoteItem> source)
        {
            Throw.IfNull(source);

            return Descendants();

            IEnumerable<IOneNoteItem> Descendants()
            {
                using var stack = StackPool.Rent();
                foreach (var item in source)
                {
                    stack.Push(item);
                    while (stack.Count > 0)
                    {
                        var current = stack.Pop();
                        yield return current;
                        foreach (var child in current.Children)
                        {
                            stack.Push(child);
                        }
                    }
                }
            }
        }

        /// <inheritdoc cref="Descendants(IOneNoteItem,Func{IOneNoteItem, bool})"/>
        public static IEnumerable<IOneNoteItem> Descendants(this IEnumerable<IOneNoteItem> source, Func<IOneNoteItem, bool> predicate)
        {
            Throw.IfNull(source);
            Throw.IfNull(predicate);

            return Descendants();

            IEnumerable<IOneNoteItem> Descendants()
            {
                using var stack = StackPool.Rent();
                foreach (var item in source)
                {
                    stack.Push(item);
                    while (stack.Count > 0)
                    {
                        var current = stack.Pop();
                        if (predicate(current))
                            yield return current;
                        foreach (var child in current.Children)
                        {
                            stack.Push(child);
                        }
                    }
                }
            }
        }
    }
}