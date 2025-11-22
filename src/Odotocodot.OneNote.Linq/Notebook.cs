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
    public class Notebook : OneNoteItem, IOneNoteItem, IWritablePath, INotebookOrSectionGroup, IWritableColor
    {
        internal Notebook() { }

        /// <inheritdoc/>
        public override IOneNoteItem Parent { get => null; internal set { } }
        /// <inheritdoc/>
        public override string RelativePath { get => Name; internal set { } }
        // /// <inheritdoc/>
        // public override Notebook Notebook { get => this; internal set { } }

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
        public IEnumerable<Section> Sections => Children.OfType<Section>();
        /// <summary>
        /// The section groups that this notebook contains (direct children only).
        /// </summary>
        public IEnumerable<SectionGroup> SectionGroups => Children.OfType<SectionGroup>();

        Color? IWritableColor.Color { set => Color = value; }
        string IWritablePath.Path { set => Path = value; }
    }


    public class NotebookFull : Notebook
    {
        
    }
}