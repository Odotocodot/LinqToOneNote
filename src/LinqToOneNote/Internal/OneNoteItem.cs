using System;
using System.Collections.Generic;
using LinqToOneNote.Abstractions;

namespace LinqToOneNote.Internal
{
    /// <summary>
    /// Use <see cref="IOneNoteItem"/> instead.
    /// </summary>
    /// <seealso cref="IOneNoteItem"/>
    public abstract class OneNoteItem // : IOneNoteItem 
    {
        internal OneNoteItem() { }

        /// <inheritdoc cref="INavigable.Id"/>
        public string Id { get; internal set; }
        /// <inheritdoc cref="IOneNoteItem.Name"/>
        public string Name { get; internal set; }
        /// <inheritdoc cref="IOneNoteItem.IsUnread"/>
        public bool IsUnread { get; internal set; }
        /// <inheritdoc cref="IOneNoteItem.LastModified"/>
        public DateTime LastModified { get; internal set; }

        // /// <inheritdoc/>
        // public IEnumerable<IOneNoteItem> Children { get; internal set; } = ReadOnlyList.Empty;
        // /// <inheritdoc/>
        // public virtual IOneNoteItem Parent { get; internal set; }

        public static IEqualityComparer<IOneNoteItem> IdComparer { get; } = new IdEqualityComparer();

        private sealed class IdEqualityComparer : IEqualityComparer<IOneNoteItem>
        {
            public bool Equals(IOneNoteItem x, IOneNoteItem y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (x is null)
                    return false;
                if (y is null)
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(IOneNoteItem obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
