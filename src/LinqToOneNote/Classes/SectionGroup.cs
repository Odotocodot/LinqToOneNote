using System.Collections.Generic;
using LinqToOneNote.Abstractions;
using LinqToOneNote.Internal;

namespace LinqToOneNote
{
    /// <summary>
    /// Represents a section group in OneNote.
    /// </summary>
    public class SectionGroup : OneNoteItem, IOneNoteItem, INotebookOrSectionGroup, INameInvalidCharacters, IHasPath, IDeletable
    {
        internal readonly ReadOnlyList<Section> sections = [];
        internal readonly ReadOnlyList<SectionGroup> sectionGroups = [];
        private readonly ChildrenCollection children;

        internal SectionGroup() => children = new ChildrenCollection(sections, sectionGroups);

        /// <summary>
        /// An array containing the characters that are not allowed in a <see cref="SectionGroup">section group</see> name.<br/>
        /// These are:&#009;<b>\ / * ? " | &lt; &gt; : % # &amp;</b>
        /// </summary>
        /// <seealso cref="OneNote.IsValidName{T}(string)"/>
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
        public IReadOnlyList<Section> Sections => sections;
        /// <summary>
        /// The section groups that this section group contains (direct children only).
        /// </summary>
        public IReadOnlyList<SectionGroup> SectionGroups => sectionGroups;

        /// <summary>
        /// The direct children of the section group, containing its <see cref="Sections">sections</see> and <see cref="SectionGroups">section groups</see>.
        /// </summary>
        public IReadOnlyList<IOneNoteItem> Children => children;
        /// <summary>
        /// The parent notebook or section group of the section group.
        /// </summary>
        public INotebookOrSectionGroup Parent { get; internal set; }
        IOneNoteItem IOneNoteItem.Parent => Parent;
    }
}
