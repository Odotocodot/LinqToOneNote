using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a section group in OneNote.
    /// </summary>
    public class SectionGroup : OneNoteItem, IOneNoteItem, INotebookOrSectionGroup, IWritablePath
    {
        internal SectionGroup() { }

        /// <summary>
        /// The full path to the section group.
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// Indicates whether this is a special section group which contains all the recently deleted sections as well as the "Deleted Pages" section (see <see cref="Section.IsDeletedPages"/>).
        /// </summary>
        /// <seealso cref="Section.IsInRecycleBin"/>
        /// <seealso cref="Section.IsDeletedPages"/>
        /// <seealso cref="Page.IsInRecycleBin"/>
        public bool IsRecycleBin { get; internal set; }

        /// <summary>
        /// The sections that this section group contains (direct children only). 
        /// </summary>
        public IEnumerable<Section> Sections => Children.OfType<Section>();
        /// <summary>
        /// The section groups that this section group contains (direct children only).
        /// </summary>
        public IEnumerable<SectionGroup> SectionGroups => Children.OfType<SectionGroup>();

        string IWritablePath.Path { set => Path = value; }
    }
}
