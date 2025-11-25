using System;
using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// The base interface of OneNote hierarchy items types.
    /// </summary>
    /// <seealso cref="Notebook"/>
    /// <seealso cref="SectionGroup"/>
    /// <seealso cref="Section"/>
    /// <seealso cref="Page"/>
    public interface IOneNoteItem
    {
        /// <summary>
        /// The ID of the OneNote hierarchy item.
        /// </summary>
        string Id { get; }
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
        // /// <summary>
        // /// The direct children of the OneNote hierarchy <see cref="IOneNoteItem">item</see>, e.g. for a <see cref="Notebook">notebook</see> it could contain <see cref="Section">sections</see> and/or <see cref="SectionGroup">section groups</see>. <br/>
        // /// If the <see cref="IOneNoteItem">item</see> has no children an empty <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; is returned. For instance, this property is an empty enumerable for a <see cref="Page">page</see>.
        // /// </summary>
        // IEnumerable<IOneNoteItem> Children { get; }
        // /// <summary>
        // /// The parent of the OneNote hierarchy item. <br/>
        // /// <see langword="null"/> if the OneNote item has no parent i.e. a <see cref="Notebook">notebook</see>.
        // /// </summary>
        // IOneNoteItem Parent { get; }
        // /// <summary>
        // /// The path of the OneNote hierarchy item relative to and inclusive of its <see cref="Notebook">notebook</see>.
        // /// </summary>
        // string RelativePath { get; }
        // /// <summary>
        // /// The <see cref="Notebook">notebook</see> that contains this OneNote hierarchy item.
        // /// </summary>
        // Notebook Notebook { get; }
    }

    public interface IOneNoteItemFull : IOneNoteItem
    {
        /// <summary>
        /// The <see cref="Notebook">notebook</see> that contains this OneNote hierarchy item.
        /// </summary>
        NotebookFull Notebook { get; }
        /// <summary>
        /// The parent of the OneNote hierarchy item. <br/>
        /// <see langword="null"/> if the OneNote item has no parent i.e. a <see cref="Notebook">notebook</see>.
        /// </summary>
        OneNoteItem Parent { get; }

        /// <summary>
        /// The direct children of the OneNote hierarchy <see cref="OneNoteItem">item</see>, e.g. for a <see cref="Notebook">notebook</see> it could contain <see cref="Section">sections</see> and/or <see cref="SectionGroup">section groups</see>. <br/>
        /// If the <see cref="OneNoteItem">item</see> has no children an empty <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="IOneNoteItem"/>&gt; is returned. For instance, this property is an empty enumerable for a <see cref="Page">page</see>.
        /// </summary>
        IEnumerable<IOneNoteItemFull> Children { get; }

        /// <summary>
        /// The path of the OneNote hierarchy item relative to and inclusive of its <see cref="Notebook">notebook</see>.
        /// </summary>
        string RelativePath { get; }
    }

    // public static class FullHierarchyExtensions
    // {
    // 	public static string GetRelativePath<T>() where T : IFullHierarchy
    // 	{
    // 		
    // 	}
    // }
}
