using System;
using System.Collections.Generic;
using System.Drawing;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a notebook in OneNote.
    /// </summary>
    public class Notebook : OneNoteItem, IOneNoteItem, INotebookOrSectionGroup, INameInvalidCharacters, IHasPath, IHasColor
    {
        internal ReadOnlyList<Section> sections;
        internal ReadOnlyList<SectionGroup> sectionGroups;
        internal Notebook()
        {
            sections = [];
            sectionGroups = [];
        }

        /// <summary>
        /// A collection containing the characters that are not allowed in a <see cref="Notebook">notebook</see> name.<br/>
        /// These are:&#009;<b>\ / * ? " | &lt; &gt; : % # .</b>
        /// </summary>
        /// <seealso cref="OneNote.IsValidName{T}(string)"/>
        public static IReadOnlyList<char> InvalidCharacters { get; } = Array.AsReadOnly(['\\', '/', '*', '?', '"', '|', '<', '>', ':', '%', '#', '.']);

        /// <summary>
        /// The display name of the notebook. Differs from <see cref="IOneNoteItem.Name"/> as it may contain characters that are invalid for folder names. Furthermore,
        /// <see cref="IOneNoteItem.Name"/> represents the folder name of the notebook on disk.
        /// </summary>
        public string DisplayName { get; internal set; }
        /// <summary>
        /// The full path to the notebook.
        /// </summary>
        public string Path { get; internal set; }
        /// <summary>
        /// The color of the notebook.
        /// </summary>
        public Color? Color { get; internal set; }

        /// <summary>
        /// The sections that this notebook contains (direct children only).
        /// </summary>
        public IReadOnlyList<Section> Sections => sections;

        /// <summary>
        /// The section groups that this notebook contains (direct children only).
        /// </summary>
        public IReadOnlyList<SectionGroup> SectionGroups => sectionGroups;

        public IReadOnlyList<IOneNoteItem> Children
        {
            get
            {
                field ??= new ChildrenCollection(sections.list, sectionGroups.list);
                return field;
            }
        }

        IOneNoteItem IOneNoteItem.Parent { get; } = null;
    }
}