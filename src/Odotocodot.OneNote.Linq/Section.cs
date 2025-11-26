using System;
using System.Collections.Generic;
using System.Drawing;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a section in OneNote.
    /// </summary>
    public class Section : OneNoteItem, IOneNoteItem, INameInvalidCharacters, IWritablePath, IWritableIsInRecycleBin, IWritableColor
    {
        internal Section() { }

        /// <summary>
        /// An array containing the characters that are not allowed in a <see cref="Section">section</see> name.<br/>
        /// These are:&#009;<b>\ / * ? " | &lt; &gt; : % # &amp;</b>
        /// </summary>
        /// <seealso cref="OneNoteApplication.IsValidName{T}(string)"/>
        public static IReadOnlyList<char> InvalidCharacters { get; } = Array.AsReadOnly(['\\', '/', '*', '?', '"', '|', '<', '>', ':', '%', '#', '&']);

        /// <summary>
        /// The full path to the section.
        /// </summary>
        public string Path { get; internal set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public bool IsReadOnly { get; internal set; }
        /// <summary>
        /// Indicates whether an encrypted section has been unlocked allowing access, otherwise <see langword="false"/>. <br/>
        /// </summary>
        /// <seealso cref="Encrypted"/>
        public bool Locked { get; internal set; }
        /// <summary>
        /// Indicates whether the section is encrypted.
        /// </summary>
        /// <seealso cref="Locked"/>
        public bool Encrypted { get; internal set; }
        /// <summary>
        /// Indicates whether the section is in recycle bin.
        /// </summary>
        /// <seealso cref="IsDeletedPages"/>
        /// <seealso cref="SectionGroup.IsRecycleBin"/>
        /// <seealso cref="Page.IsInRecycleBin"/>
        public bool IsInRecycleBin { get; internal set; }
        /// <summary>
        /// Indicates whether this section is a special section that contains all the recently deleted pages in this section's notebook.
        /// </summary>
        /// <seealso cref="IsInRecycleBin"/>
        /// <seealso cref="SectionGroup.IsRecycleBin"/>
        /// <seealso cref="Page.IsInRecycleBin"/>
        public bool IsDeletedPages { get; internal set; }
        /// <summary>
        /// The color of the section.
        /// </summary>
        public Color? Color { get; internal set; }

        /// <summary>
        /// The collection of pages within this section, equal to <see cref="IOneNoteItem.Children"/> for a section.
        /// </summary>
        public IReadOnlyList<Page> Pages { get; internal set; }
        public IReadOnlyList<IOneNoteItem> Children => Pages;
        public INotebookOrSectionGroup Parent { get; internal set; }
        IOneNoteItem IOneNoteItem.Parent => Parent;
        Color? IWritableColor.Color { set => Color = value; }
        string IWritablePath.Path { set => Path = value; }
        bool IWritableIsInRecycleBin.IsInRecycleBin { set => IsInRecycleBin = value; }
    }
}