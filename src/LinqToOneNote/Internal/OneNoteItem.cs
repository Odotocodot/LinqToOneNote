using System;
using System.Collections.Generic;

namespace LinqToOneNote.Internal
{
    /// <exclude/>
    /// <summary>
    /// Use <see cref="IOneNoteItem"/> instead.
    /// </summary>
    /// <seealso cref="IOneNoteItem"/>
    public abstract class OneNoteItem // : IOneNoteItem 
    {
        internal bool isUnread;
        internal DateTime lastModified;
        internal string name;
        internal string id;
        internal OneNoteItem() { }
    }
}
