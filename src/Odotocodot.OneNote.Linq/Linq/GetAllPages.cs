using System.Collections.Generic;
using System.Linq;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
    public static partial class LinqExtensions
    {
        /// <summary>
        /// Returns a flattened collection of all the <see cref="Page">pages</see> present in the <paramref name="source"/>.
        /// </summary>
        /// <param name="source"><inheritdoc cref="Linq.LinqExtensions.Traverse(Odotocodot.OneNote.Linq.IOneNoteItem)" path="/param[@name='source']"/></param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="Page"/>&gt; containing all the 
        /// <see cref="Page">pages</see> present in the <paramref name="source"/>.</returns>
        public static IEnumerable<Page> GetPages(this IOneNoteItem source)
            => source.Traverse(i => i is Page).Cast<Page>();

        /// <inheritdoc cref="GetPages(IOneNoteItem)"/>
        public static IEnumerable<Page> GetPages(this IEnumerable<IOneNoteItem> source)
            => source.Traverse(i => i is Page).Cast<Page>();



        /// <summary>
        /// Returns a flattened collection of all the <see cref="Page">pages</see> present in the <paramref name="source"/>.
        /// </summary>
        /// <param name="source"><inheritdoc cref="Descendants(IOneNoteItem)" path="/param[@name='source']"/></param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="Page"/>&gt; containing all the
        /// <see cref="Page">pages</see> present in the <paramref name="source"/>.</returns>
        public static IEnumerable<Page> GetAllPages(this IOneNoteItem source)
        {
            Throw.IfNull(source);

            return GetAllPages();

            IEnumerable<Page> GetAllPages()
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

        /// <inheritdoc cref="GetAllPages(IOneNoteItem)"/>
        public static IEnumerable<Page> GetAllPages(this IEnumerable<IOneNoteItem> source)
        {
            Throw.IfNull(source);

            return GetAllPages();

            IEnumerable<Page> GetAllPages()
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
}