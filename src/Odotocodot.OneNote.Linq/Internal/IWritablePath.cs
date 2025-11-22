using Odotocodot.OneNote.Linq.Abstractions;

namespace Odotocodot.OneNote.Linq.Internal
{
    internal interface IWritablePath : IHasPath
    {
        new string Path { set; }
    }
}