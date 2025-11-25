using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents a notebook in OneNote.
    /// </summary>
    public class Notebook : OneNoteItem, IOneNoteItem, IWritablePath, IWritableColor
    {
        internal Notebook() { }
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

        Color? IWritableColor.Color { set => Color = value; }
        string IWritablePath.Path { set => Path = value; }
    }


    public class NotebookFull : Notebook, IOneNoteItemFull
    {
        OneNoteItem IOneNoteItemFull.Parent { get; } = null;
        public string RelativePath => Name;  //TODO: Implement
        
        /// <summary>
        /// The sections that this notebook contains (direct children only). 
        /// </summary>
        public IEnumerable<SectionFull> Sections => children.OfType<SectionFull>();
        
        /// <summary>
        /// The section groups that this notebook contains (direct children only).
        /// </summary>
        public IEnumerable<SectionGroupFull> SectionGroups => children.OfType<SectionGroupFull>();
        
        /// <summary>
        /// The direct children of the OneNote hierarchy <see cref="IOneNoteItem">item</see>, e.g. for a <see cref="Notebook">notebook</see> it could contain <see cref="Section">sections</see> and/or <see cref="SectionGroup">section groups</see>. <br/>
        /// If the <see cref="IOneNoteItem">item</see> has no children an empty <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; is returned. For instance, this property is an empty enumerable for a <see cref="Page">page</see>.
        /// </summary>
        public IEnumerable<IOneNoteItemFull> Children => children.Cast<IOneNoteItemFull>();

        public NotebookFull Notebook => this;
    }
}