using System;
using System.Collections.Generic;

namespace Odotocodot.OneNote.Linq.Parsers
{
    internal interface IXmlParser
    {
        Root ParseFullHierarchy(string xml);
        IOneNoteItem Parse(string xml, IOneNoteItem parent);

        [Obsolete]
        IEnumerable<Notebook> ParseNotebooks(string xml);
        [Obsolete]
        IOneNoteItem ParseUnknown(string xml, IOneNoteItem parent);
    }
}