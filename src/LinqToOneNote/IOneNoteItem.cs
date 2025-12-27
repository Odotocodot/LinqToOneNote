using System;
using System.Collections.Generic;
using LinqToOneNote.Abstractions;

namespace LinqToOneNote
{
    /// <summary>
    /// The base interface of OneNote hierarchy items types.
    /// </summary>
    /// <seealso cref="Notebook"/>
    /// <seealso cref="SectionGroup"/>
    /// <seealso cref="Section"/>
    /// <seealso cref="Page"/>
    public interface IOneNoteItem : INavigable
    {
        /// <summary>
        /// The name of the OneNote hierarchy item.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Indicates whether the OneNote hierarchy item has unread information.
        /// </summary>
        bool IsUnread { get; }
        /// <summary>
        /// The time when the OneNote hierarchy item was last modified.
        /// </summary>
        DateTime LastModified { get; }
        /// <summary>
        /// The direct children of the OneNote hierarchy <see cref="IOneNoteItem">item</see>, e.g. for a <see cref="Notebook">notebook</see> it could contain <see cref="Section">sections</see> and/or <see cref="SectionGroup">section groups</see>. <br/>
        /// If the <see cref="IOneNoteItem">item</see> has no children an empty <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; is returned. For instance, this property is an empty enumerable for a <see cref="Page">page</see>.
        /// </summary>
        IReadOnlyList<IOneNoteItem> Children { get; }
        /// <summary>
        /// The parent of the OneNote hierarchy item. <br/>
        /// <see langword="null"/> if the item has no parent e.g. a <see cref="Notebook">notebook</see>.
        /// </summary>
        IOneNoteItem Parent { get; }
    }
}
