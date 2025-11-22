using Odotocodot.OneNote.Linq.Internal;
using System;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a page in OneNote.
    /// </summary>
    public class Page : OneNoteItem, IOneNoteItem, IWritableIsInRecycleBin
    {
        internal Page() { }

        /// <summary>
        /// The section that owns this page.
        /// </summary>
        public Section Section => (Section)Parent;
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
        /// <seealso cref="Linq.Section.IsInRecycleBin"/>
        /// <seealso cref="Linq.Section.IsDeletedPages"/>
        public bool IsInRecycleBin { get; internal set; }

        bool IWritableIsInRecycleBin.IsInRecycleBin { set => IsInRecycleBin = value; }
    }
}