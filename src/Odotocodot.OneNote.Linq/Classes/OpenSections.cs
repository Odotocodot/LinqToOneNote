using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Represents the root of open sections in OneNote. These are sections that are not contained in a notebook.
    /// </summary>
    public class OpenSections : INavigable
    {
        internal ReadOnlyList<Section> sections = [];
        internal OpenSections() { }
        /// <summary>
        /// The sections that are open in OneNote but not contained in any notebook.
        /// </summary>
        public IReadOnlyList<Section> Sections => sections;

        ///<inheritdoc/>
        public string Id { get; internal set; }
    }
}