using System;
using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
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