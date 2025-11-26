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
    public class Notebook : OneNoteItem, IOneNoteItem, INotebookOrSectionGroup, INameInvalidCharacters, IWritablePath, IWritableColor
    {
        internal Notebook() { }

        /// <summary>
        /// A collection containing the characters that are not allowed in a <see cref="Notebook">notebook</see> name.<br/>
        /// These are:&#009;<b>\ / * ? " | &lt; &gt; : % # .</b>
        /// </summary>
        /// <seealso cref="OneNoteApplication.IsValidName{T}(string)"/>
        public static IReadOnlyList<char> InvalidCharacters { get; } = Array.AsReadOnly(['\\', '/', '*', '?', '"', '|', '<', '>', ':', '%', '#', '.']);

        /// <summary>
        /// The nickname of the notebook.
        /// </summary>
        public string NickName { get; internal set; }
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
        public IReadOnlyList<Section> Sections { get; internal set; }

        /// <summary>
        /// The section groups that this notebook contains (direct children only).
        /// </summary>
        public IReadOnlyList<SectionGroup> SectionGroups { get; internal set; }

        // TODO: Could make Children an extension method 
        public IReadOnlyList<IOneNoteItem> Children { get; internal set; }

        IOneNoteItem IOneNoteItem.Parent { get; } = null;

        Color? IWritableColor.Color { set => Color = value; }
        string IWritablePath.Path { set => Path = value; }
    }
}