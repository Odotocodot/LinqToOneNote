using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Abstractions;

namespace Odotocodot.OneNote.Linq
{
    public class OpenSections : INavigable
    {
        internal OpenSections() { }
        public IReadOnlyList<Section> Sections { get; internal set; }
        public string Id { get; internal set; }
    }
}