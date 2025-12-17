using System;
using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a page in OneNote.
    /// </summary>
    public class Page : OneNoteItem, IOneNoteItem, IHasIsInRecycleBin, IDeletable
    {
        internal Page() { }
        /// <summary>
        /// The page level.
        /// </summary>
        public int Level { get; internal set; }
        /// <summary>
        /// The time when the page was created.
        /// </summary>
        public DateTime Created { get; internal set; }
        /// <summary>
        /// Indicates whether the page is in the recycle bin.
        /// </summary>
        /// <seealso cref="SectionGroup.IsRecycleBin"/>
        /// <seealso cref="Section.IsInRecycleBin"/>
        /// <seealso cref="Section.IsDeletedPages"/>
        public bool IsInRecycleBin { get; internal set; }

        public Section Parent { get; internal set; }

        IOneNoteItem IOneNoteItem.Parent => Parent;
        IReadOnlyList<IOneNoteItem> IOneNoteItem.Children { get; } = ReadOnlyList.Empty;
    }
}