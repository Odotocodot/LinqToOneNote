using System;
using System.Collections.Generic;
using LinqToOneNote.Internal;

namespace LinqToOneNote
{
    public static partial class LinqExtensions
    {
        /// <summary>
        /// Returns a flattened collection of all the <see cref="Page">pages</see> present in the <paramref name="source"/>.
        /// </summary>
        /// <param name="source"><inheritdoc cref="Descendants(IOneNoteItem)" path="/param[@name='source']"/></param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="Page"/>&gt; containing all the
        /// <see cref="Page">pages</see> present in the <paramref name="source"/>.</returns>
        public static IEnumerable<Page> GetAllPages(this IOneNoteItem source)
        {
            Throw.IfNull(source);

            return InternalGetAllPages(source);
        }


        /// <summary>
        /// Returns a filtered flattened collection of all the <see cref="Page">pages</see> present in the <paramref name="source"/>.<br/>
        /// Only items that successfully pass the <paramref name="predicate"/> are returned.
        /// </summary>
        /// <param name="source">The item for which to retrieve the pages of.</param>
        /// <param name="predicate">The predicate to filter the pages.</param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="Page"/>&gt; containing all the
        /// <see cref="Page">pages</see> present in the <paramref name="source"/> that pass the <paramref name="predicate"/>.</returns>
        public static IEnumerable<Page> GetAllPages(this IOneNoteItem source, Func<IOneNoteItem, bool> predicate)
        {
            Throw.IfNull(source);
            Throw.IfNull(predicate);

            return InternalGetAllPages(source, predicate);
        }

        private static IEnumerable<Page> InternalGetAllPages(IOneNoteItem source, Func<IOneNoteItem, bool> predicate = null)
        {
            using var stack = StackPool.Rent();
            stack.Push(source);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current is Section section)
                {
                    foreach (var page in section.Pages)
                    {
                        if (predicate == null || predicate(page))
                            yield return page;
                    }
                }
                else
                {
                    foreach (var child in current.Children)
                    {
                        stack.Push(child);
                    }
                }
            }
        }

        /// <inheritdoc cref="GetAllPages(IOneNoteItem)"/>
        public static IEnumerable<Page> GetAllPages(this IEnumerable<IOneNoteItem> source)
        {
            Throw.IfNull(source);

            return InternalGetAllPages(source);
        }

        /// <inheritdoc cref="GetAllPages(IOneNoteItem,Func{IOneNoteItem, bool})"/>
        public static IEnumerable<Page> GetAllPages(this IEnumerable<IOneNoteItem> source, Func<IOneNoteItem, bool> predicate)
        {
            Throw.IfNull(source);
            Throw.IfNull(predicate);
            return InternalGetAllPages(source, predicate);
        }

        private static IEnumerable<Page> InternalGetAllPages(IEnumerable<IOneNoteItem> source, Func<IOneNoteItem, bool> predicate = null)
        {
            using var stack = StackPool.Rent();
            foreach (var item in source)
            {
                stack.Push(item);
                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    if (current is Section section)
                    {
                        foreach (var page in section.Pages)
                        {
                            if (predicate == null || predicate(page))
                                yield return page;
                        }
                    }
                    else
                    {
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