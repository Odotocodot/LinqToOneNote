using System.Collections.Generic;

namespace LinqToOneNote.Abstractions
{
    /// <summary>
    /// Represents an OneNote hierarchy item that has a collection of characters that are invalid when naming an item.
    /// </summary>
    /// <seealso cref="Notebook"/>
    /// <seealso cref="SectionGroup"/>
    /// <seealso cref="Section"/>
    public interface INameInvalidCharacters
    {
        /// <summary>
        /// A collection of characters that are invalid for the item's name.
        /// </summary>
        static abstract IReadOnlyList<char> InvalidCharacters { get; }
    }
}
