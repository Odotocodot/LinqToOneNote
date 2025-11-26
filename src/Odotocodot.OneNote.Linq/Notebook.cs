using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;

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

        /// <summary>
        /// The sections that this notebook contains (direct children only). 
        /// </summary>
        public IReadOnlyList<Section> Sections { get; internal set; }

        /// <summary>
        /// The section groups that this notebook contains (direct children only).
        /// </summary>
        public IReadOnlyList<SectionGroup> SectionGroups { get; internal set; }

        // TODO: make Children an extension method 
        // /// <summary>
        // /// The direct children of the OneNote hierarchy <see cref="IOneNoteItem">item</see>, e.g. for a <see cref="Notebook">notebook</see> it could contain <see cref="Section">sections</see> and/or <see cref="SectionGroup">section groups</see>. <br/>
        // /// If the <see cref="IOneNoteItem">item</see> has no children an empty <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; is returned. For instance, this property is an empty enumerable for a <see cref="Page">page</see>.
        // /// </summary>
        // public IReadOnlyList<IOneNoteItem> Children => children.Cast<IOneNoteItemFull>();

        Color? IWritableColor.Color { set => Color = value; }
        string IWritablePath.Path { set => Path = value; }
    }
}