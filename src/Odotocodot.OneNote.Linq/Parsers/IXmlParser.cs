namespace Odotocodot.OneNote.Linq.Parsers
{
    internal interface IXmlParser
    {
        Root ParseRoot(string xml);
        IOneNoteItem Parse(string xml, IOneNoteItem parent);
    }
}