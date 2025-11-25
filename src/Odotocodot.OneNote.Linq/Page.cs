using Odotocodot.OneNote.Linq.Internal;
using System;
using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Abstractions;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a page in OneNote.
    /// </summary>
    public class Page : OneNoteItem, IOneNoteItem, IWritableIsInRecycleBin
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

        bool IWritableIsInRecycleBin.IsInRecycleBin { set => IsInRecycleBin = value; }
    }

    public class PageFull : Page, IOneNoteItemFull
    {
        /// <summary>
        /// The section that owns this page.
        /// </summary>
        public SectionFull Section => (SectionFull)parent;

        public NotebookFull Notebook => (NotebookFull)notebook;
        public OneNoteItem Parent { get; }
        IEnumerable<IOneNoteItemFull> IOneNoteItemFull.Children { get; } = [];
        public string RelativePath { get; }
    }
}