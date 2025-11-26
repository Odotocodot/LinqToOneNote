using System;
using System.Collections.Generic;
using System.Linq;

namespace Odotocodot.OneNote.Linq.Internal
{
    /// <summary>
    /// Use <see cref="IOneNoteItem"/> instead.
    /// </summary>
    /// <seealso cref="IOneNoteItem"/>
    public abstract class OneNoteItem// : IOneNoteItem
    {
        internal OneNoteItem() { }

        /// <inheritdoc/>
        public string Id { get; internal set; }
        /// <inheritdoc/>
        public string Name { get; internal set; }
        /// <inheritdoc/>
        public bool IsUnread { get; internal set; }
        /// <inheritdoc/>
        public DateTime LastModified { get; internal set; }
        // /// <inheritdoc/>
        // public IEnumerable<IOneNoteItem> Children { get; internal set; } = Enumerable.Empty<IOneNoteItem>();
        // /// <inheritdoc/>
        // public virtual IOneNoteItem Parent { get; internal set; }
        // /// <inheritdoc/>
        // public virtual string RelativePath { get; internal set; }
        // /// <inheritdoc/>
        // public virtual Notebook Notebook { get; internal set; }

        // #nullable enable
        // internal OneNoteItem? parent;
        // internal Notebook? notebook;
        // #nullable restore
        // internal IEnumerable<OneNoteItem> children = [];

        public static IEqualityComparer<OneNoteItem> IdComparer { get; } = new IdEqualityComparer();

        private sealed class IdEqualityComparer : IEqualityComparer<OneNoteItem>
        {
            public bool Equals(OneNoteItem x, OneNoteItem y)
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

            public int GetHashCode(OneNoteItem obj)
            {
                return obj.Id.GetHashCode();
            }
        }

    }
}
