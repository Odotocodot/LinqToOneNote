using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;
using Odotocodot.OneNote.Linq.Parsers;

namespace Odotocodot.OneNote.Linq.Extensions
{
    /// <summary>
    /// A static class containing extension methods for the <see cref="IOneNoteItem"/> object.
    /// </summary>
    public static class OneNoteItemExtensions
    {
        /// <inheritdoc cref="OneNote.Open"/>
        public static void Open(this INavigable item) => OneNote.Open(item);

        /// <inheritdoc cref="OneNote.SyncItem(INavigable)"/>
        public static void Sync(this INavigable item) => OneNote.SyncItem(item);

        /// <inheritdoc cref="OneNote.GetPageContent(Page)"/>
        public static string GetPageContent(this Page page) => OneNote.GetPageContent(page);

        ///<inheritdoc cref="OneNote.RenameItem"/>
        public static void Rename(this IOneNoteItem item, string newName) => OneNote.RenameItem(item, newName);

        /// <inheritdoc cref="OneNote.DeleteItem"/>
        public static void Delete(this IDeletable item, DateTime dateExpectedLastModified = default, bool deletePermanently = false)
            => OneNote.DeleteItem(item, dateExpectedLastModified, deletePermanently);

        /// <inheritdoc cref="OneNote.CloseNotebook"/>
        public static void Close(this Notebook notebook, bool force = false) => OneNote.CloseNotebook(notebook, force);

        /// <inheritdoc cref="OneNote.CreateSectionGroup"/>
        public static SectionGroup CreateSectionGroup(this INotebookOrSectionGroup parent, string name, OpenMode openMode = OpenMode.None)
            => OneNote.CreateSectionGroup(parent, name, openMode);

        /// <inheritdoc cref="OneNote.CreateSection"/>
        public static Section CreateSection(this INotebookOrSectionGroup parent, string name, OpenMode openMode = OpenMode.None)
            => OneNote.CreateSection(parent, name, openMode);

        /// <inheritdoc cref="OneNote.CreatePage"/>
        public static Page CreatePage(this Section parent, string name = "", OpenMode openMode = OpenMode.None)
            => OneNote.CreatePage(parent, name, openMode);

        /// <summary>
        /// Returns a value that indicates whether the <paramref name="item"/> is in or is a recycle bin.
        /// </summary>
        /// <param name="item">The OneNote item to check.</param>
        /// <returns><see langword="true"/> if the <paramref name="item"/> is in or is a recycle bin; otherwise, <see langword="false"/>.</returns>
        /// <remarks>Checks whether the <paramref name="item"/> is a recycle bin <see cref="SectionGroup">section group</see>,
        /// a deleted <see cref="Page">page</see>, a deleted <see cref="Section">section</see>, or the deleted pages 
        /// <see cref="Section">section</see> within a recycle bin.</remarks>
        /// <seealso cref="SectionGroup.IsRecycleBin"/>
        /// <seealso cref="Section.IsInRecycleBin"/>
        /// <seealso cref="Section.IsDeletedPages"/>
        /// <seealso cref="Page.IsInRecycleBin"/>
        public static bool IsInRecycleBin(this IOneNoteItem item)
        {
            return item switch
            {
                SectionGroup sectionGroup => sectionGroup.IsRecycleBin,
                Section section => section.IsInRecycleBin || section.IsDeletedPages,//If IsDeletedPages is true IsInRecycleBin is always true
                Page page => page.IsInRecycleBin,
                _ => false,
            };
        }

        /// <summary>
        /// Get the recycle bin <see cref="SectionGroup">section group</see> for the specified <paramref name="notebook"/> if it exists.
        /// </summary>
        /// <param name="notebook">The notebook to get the recycle bin of.</param>
        /// <param name="sectionGroup">When this method returns, <paramref name="sectionGroup"/> contains the recycle bin of 
        /// the <paramref name="notebook"/> if it was found;
        /// otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the <paramref name="notebook"/> contains a recycle bin; otherwise, <see langword="false"/>.</returns>
        public static bool GetRecycleBin(this Notebook notebook, out SectionGroup sectionGroup)
        {
            sectionGroup = notebook.SectionGroups.FirstOrDefault(sg => sg.IsRecycleBin);
            return sectionGroup != null;
        }

        public static bool TryGetNotebook(this IOneNoteItem item, out Notebook notebook)
        {
            var current = item.Parent;
            while (current.Parent != null)
            {
                current = current.Parent;
            }
            if (current is Notebook nb)
            {
                notebook = nb;
                return true;
            }
            notebook = null;
            return false;
        }

        private static readonly SimplePool<StringBuilder> StringBuilderPool = new(10);
        private const string DefaultRelativePathSeparator = "\\";
        public static string GetRelativePath(this IOneNoteItem item, bool useNotebookDisplayName = true, string separator = DefaultRelativePathSeparator)
        {
            Throw.IfNull(item);
            StringBuilder sb = StringBuilderPool.Rent();
            IOneNoteItem current = item;
            while (true)
            {
                if (current is Notebook notebook)
                {
                    sb.Insert(0, useNotebookDisplayName ? notebook.DisplayName : notebook.Name);
                    break;
                }

                if (current is null)
                {
                    break;
                }
                
                sb.Insert(0, current.Name);
                sb.Insert(0, separator);
                current = current.Parent;

            }
            string relativePath = sb.ToString();
            sb.Clear();
            StringBuilderPool.Return(sb);
            return relativePath;
        }

        /// <summary>
        /// Extension that method that combines <see cref="OneNote.DeleteItem"/> and <see cref="OneNote.CloseNotebook"/>,
        /// i.e. deletes the item if it is a <see cref="IDeletable"/> or closes it if it is a <see cref="Notebook">notebook</see>.
        /// </summary>
        /// <param name="item">The item to be deleted or closed if it is a <see cref="Notebook">notebook</see></param>
        /// <param name="dateExpectedLastModified"><inheritdoc cref="OneNote.DeleteItem" path="/param[@name='dateExpectedLastModified']"/></param>
        /// <param name="deletePermanently"><inheritdoc cref="OneNote.DeleteItem" path="/param[@name='deletePermanently']"/></param>
        /// <param name="force"><inheritdoc cref="OneNote.CloseNotebook" path="/param[@name='force']"/></param>
        /// <exception cref="ArgumentException"></exception>
        public static void DeleteOrClose(this IOneNoteItem item, DateTime dateExpectedLastModified = default, bool deletePermanently = false, bool force = false)
        {
            if (item is IDeletable deletable)
            {
                OneNote.DeleteItem(deletable, dateExpectedLastModified, deletePermanently);
            }
            else if (item is Notebook notebook)
            {
                OneNote.CloseNotebook(notebook, force);
            }
            else
            {
                throw Exceptions.InvalidIOneNoteItem(item);
            }
        }
        
        /// <summary>
        /// Checks if two <see cref="IOneNoteItem"/>s represent the same item in OneNote.<br/>
        /// Shorthand for comparing the <see cref="IOneNoteItem.Id">ID</see> of OneNote hierarchy items. E.g.
        /// <code lang="C#">
        /// if(left.ID == right.ID)
        /// {
        ///     Console.WriteLine("Equal")
        /// }
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool ItemEquals<T>(this T left, T right) where T : IOneNoteItem
        {
            return OneNoteItem.IdComparer.Equals(left, right);
        }
    }
}
