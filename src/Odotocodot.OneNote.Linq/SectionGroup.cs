using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a section group in OneNote.
    /// </summary>
    public class SectionGroup : OneNoteItem, IOneNoteItem, INotebookOrSectionGroup, INameInvalidCharacters, IHasPath
    {
        internal SectionGroup() { }

        /// <summary>
        /// An array containing the characters that are not allowed in a <see cref="SectionGroup">section group</see> name.<br/>
        /// These are:&#009;<b>\ / * ? " | &lt; &gt; : % # &amp;</b>
        /// </summary>
        /// <seealso cref="OneNoteApplication.IsValidName{T}(string)"/>
        public static IReadOnlyList<char> InvalidCharacters { get; } = Section.InvalidCharacters;

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
        public IReadOnlyList<Section> Sections { get; internal set; }
        /// <summary>
        /// The section groups that this section group contains (direct children only).
        /// </summary>
        public IReadOnlyList<SectionGroup> SectionGroups { get; internal set; }
        public IReadOnlyList<IOneNoteItem> Children { get; internal set; }
        public INotebookOrSectionGroup Parent { get; internal set; }
        IOneNoteItem IOneNoteItem.Parent => Parent;
    }
}
