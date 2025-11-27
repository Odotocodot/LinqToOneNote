using System;
using System.Collections.Generic;
using System.Linq;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq.Extensions
{
    /// <summary>
    /// A static class containing extension methods for the <see cref="IOneNoteItem"/> object.
    /// </summary>
    public static class OneNoteItemExtensions
    {
        /// <inheritdoc cref="OneNoteApplication.OpenInOneNote(INavigable)"/>
        public static void OpenInOneNote(this INavigable item) => OneNoteApplication.OpenInOneNote(item);

        /// <inheritdoc cref="OneNoteApplication.SyncItem(INavigable)"/>
        public static void Sync(this INavigable item) => OneNoteApplication.SyncItem(item);

        /// <inheritdoc cref="OneNoteApplication.GetPageContent(Page)"/>
        public static string GetPageContent(this Page page) => OneNoteApplication.GetPageContent(page);

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
