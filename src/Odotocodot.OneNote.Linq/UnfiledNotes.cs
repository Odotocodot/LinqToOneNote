using Odotocodot.OneNote.Linq.Abstractions;

namespace Odotocodot.OneNote.Linq
{
    public class UnfiledNotes : INavigable
    {
        internal UnfiledNotes() { }
        public Section Section { get; internal set; }
        public string Id { get; internal set; }
    }
}