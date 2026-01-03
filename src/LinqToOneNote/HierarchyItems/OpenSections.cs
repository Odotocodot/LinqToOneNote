using System.Collections;
using System.Collections.Generic;
using LinqToOneNote.Abstractions;
using LinqToOneNote.Internal;

namespace LinqToOneNote
{
    /// <summary>
    /// Represents the root of open sections in OneNote. These are sections that are not contained in a notebook.
    /// </summary>
    public class OpenSections : INavigable, IReadOnlyList<Section>
    {
        internal ReadOnlyList<Section> sections = [];
        internal OpenSections() { }
        /// <summary>
        /// The sections that are open in OneNote but not contained in any notebook.
        /// </summary>
        public IReadOnlyList<Section> Sections => sections;

        /// <summary>
        /// The id of this object in OneNote.
        /// </summary>
        public string Id { get; internal set; }
        
        IEnumerator<Section> IEnumerable<Section>.GetEnumerator() => sections.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => sections.GetEnumerator();
        
        /// <summary>
        /// Gets the number of open sections in the OneNote.
        /// </summary>
        public int Count => sections.Count;

        ///<inheritdoc/>
        public Section this[int index] => sections[index];
    }
}