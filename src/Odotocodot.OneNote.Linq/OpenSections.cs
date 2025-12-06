using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
    public class OpenSections : INavigable
    {
        internal ReadOnlyList<Section> sections;
        internal OpenSections() { sections = []; }
        public IReadOnlyList<Section> Sections => sections;
        public string Id { get; internal set; }
    }
}