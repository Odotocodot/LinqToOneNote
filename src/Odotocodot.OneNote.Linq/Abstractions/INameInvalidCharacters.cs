using System.Collections.Generic;

namespace Odotocodot.OneNote.Linq.Abstractions
{
    /// <summary>
    /// Represents an OneNote hierarchy item that has a collection of characters that are invalid when naming an item.
    /// </summary>
    /// <seealso cref="Notebook"/>
    /// <seealso cref="SectionGroup"/>
    /// <seealso cref="Section"/>
    public interface INameInvalidCharacters
    {
        static abstract IReadOnlyList<char> InvalidCharacters { get; }
    }
}
