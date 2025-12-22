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
        internal readonly ReadOnlyList<Section> sections = [];
        internal readonly ReadOnlyList<SectionGroup> sectionGroups = [];
        internal Notebook() { }

        /// <summary>
        /// A collection containing the characters that are not allowed in a <see cref="Notebook">notebook</see> name.<br/>
        /// These are:&#009;<b>\ / * ? " | &lt; &gt; : % # .</b>
        /// </summary>
        /// <seealso cref="OneNote.IsValidName{T}(string)"/>
        public static IReadOnlyList<char> InvalidCharacters { get; } = Array.AsReadOnly(['\\', '/', '*', '?', '"', '|', '<', '>', ':', '%', '#', '.']);

        /// <summary>
        /// The display name of the notebook. Differs from <see cref="Notebook.Name"/> as it may contain characters that are invalid for folder names. Furthermore,
        /// <see cref="Notebook.Name"/> represents the folder name of the notebook on disk.
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

        /// <summary>
        /// The direct children of the notebook, containing its <see cref="Notebook.Sections">sections</see> and <see cref="Notebook.SectionGroups">section groups</see>.
        /// </summary>
        public IReadOnlyList<IOneNoteItem> Children
        {
            get
            {
                field ??= new ChildrenCollection(sections, sectionGroups);
                return field;
            }
        }

        IOneNoteItem IOneNoteItem.Parent { get; } = null;
    }
}