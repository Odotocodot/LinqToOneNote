using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml;


namespace Odotocodot.OneNote.Linq.Parsers
{
    using static Constants;

    /// Faster than XElement parser, that is if you use don't care about lazy IEnumerables
    internal class XmlParserXmlReader : IXmlParser
    {
        public IOneNoteItem ParseUnknown(string xml, IOneNoteItem parent)
        {
            using (var stringReader = new StringReader(xml))
            {
                using (var reader = XmlReader.Create(stringReader))
                {
                    reader.MoveToContent();
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.LocalName)
                        {
                            case Elements.Notebook:
                                return ParseNotebook(reader);
                            case Elements.Section:
                                return ParseSection(reader, parent);
                            case Elements.SectionGroup:
                                return ParseSectionGroup(reader, parent);
                            case Elements.Page:
                                return ParsePage(reader, (Section)parent);
                            default:
                                return null;
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<Notebook> ParseNotebooks(string xml)
        {
            using (var stringReader = new StringReader(xml))
            {
                using (var reader = XmlReader.Create(stringReader))
                {
                    reader.MoveToContent();
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == Elements.NotebookList)
                    {
                        return ParseNotebooks(reader);
                    }
    
                    return Array.Empty<Notebook>();
                }
            }
        }

        private List<Notebook> ParseNotebooks(XmlReader reader)
        {
            var notebooks = new List<Notebook>();

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return notebooks;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == Elements.Notebook)
                {
                    notebooks.Add(ParseNotebook(reader));
                }
                else
                {
                    reader.Read();
                }
            }

            reader.ReadEndElement();
            return notebooks;
        }
        private Notebook ParseNotebook(XmlReader reader)
        {
            var notebook = new Notebook();
            // reader.MoveToContent();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case Attributes.ID:
                        notebook.Id = reader.Value;
                        break;
                    case Attributes.Name:
                        notebook.Name = reader.Value;
                        break;
                    case Attributes.NickName:
                        notebook.NickName = reader.Value;
                        break;
                    case Attributes.Path:
                        notebook.Path = reader.Value;
                        break;
                    case Attributes.Color:
                        notebook.Color = GetColor(reader.Value);
                        break;
                    case Attributes.IsUnread:
                        notebook.IsUnread = bool.Parse(reader.Value);
                        break;
                    case Attributes.LastModifiedTime:
                        notebook.LastModified = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;
                }
            }

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                notebook.Children = Array.Empty<IOneNoteItem>();
                return notebook;
            }

            var children = new List<IOneNoteItem>();
            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == Elements.Section)
                    {
                        children.Add(ParseSection(reader, notebook));
                    }
                    else if (reader.LocalName == Elements.SectionGroup)
                    {
                        children.Add(ParseSectionGroup(reader, notebook));
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                else
                {
                    reader.Read();
                }
                reader.MoveToContent();
            }

            notebook.Children = children;
            reader.ReadEndElement();
            return notebook;
        }

        private SectionGroup ParseSectionGroup(XmlReader reader, IOneNoteItem parent)
        {
            var sectionGroup = new SectionGroup();
            sectionGroup.Parent = parent;
            sectionGroup.Notebook = parent.Notebook;
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case Attributes.ID:
                        sectionGroup.Id = reader.Value;
                        break;
                    case Attributes.Name:
                        sectionGroup.Name = reader.Value;
                        break;
                    case Attributes.LastModifiedTime:
                        sectionGroup.LastModified = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;
                    case Attributes.IsUnread:
                        sectionGroup.IsUnread = bool.Parse(reader.Value);
                        break;
                    case Attributes.Path:
                        sectionGroup.Path = reader.Value;
                        break;
                    case Attributes.IsRecycleBin:
                        sectionGroup.IsRecycleBin = bool.Parse(reader.Value);
                        break;
                }
            }

            sectionGroup.RelativePath = $"{parent.RelativePath}{RelativePathSeparatorString}{sectionGroup.Name}";

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                sectionGroup.Children = Array.Empty<IOneNoteItem>();
                return sectionGroup;
            }

            var children = new List<IOneNoteItem>();
            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == Elements.Section)
                    {
                        children.Add(ParseSection(reader, sectionGroup));
                    }
                    else if (reader.LocalName == Elements.SectionGroup)
                    {
                        children.Add(ParseSectionGroup(reader, sectionGroup));
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                else
                {
                    reader.Read();
                }
                reader.MoveToContent();
            }

            sectionGroup.Children = children;
            reader.ReadEndElement();
            return sectionGroup;
        }


        private Section ParseSection(XmlReader reader, IOneNoteItem parent)
        {
            var section = new Section();
            section.Parent = parent;
            section.Notebook = parent.Notebook;
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case Attributes.ID:
                        section.Id = reader.Value;
                        break;
                    case Attributes.Name:
                        section.Name = reader.Value;
                        break;
                    case Attributes.LastModifiedTime:
                        section.LastModified = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;
                    case Attributes.IsUnread:
                        section.IsUnread = bool.Parse(reader.Value);
                        break;
                    case Attributes.Path:
                        section.Path = reader.Value;
                        break;
                    case Attributes.Color:
                        section.Color = ColorTranslator.FromHtml(reader.Value);
                        break;
                    case Attributes.Encrypted:
                        section.Encrypted = bool.Parse(reader.Value);
                        break;
                    case Attributes.Locked:
                        section.Locked = bool.Parse(reader.Value);
                        break;
                    case Attributes.IsInRecycleBin:
                        section.IsInRecycleBin = bool.Parse(reader.Value);
                        break;
                    case Attributes.IsDeletedPages:
                        section.IsDeletedPages = bool.Parse(reader.Value);
                        break;
                }
            }

            section.RelativePath = $"{parent.RelativePath}{RelativePathSeparatorString}{section.Name}";

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                section.Children = Array.Empty<IOneNoteItem>();
                return section;
            }

            var pages = new List<Page>();
            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == Elements.Page)
                    {
                        pages.Add(ParsePage(reader, section));
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                else
                {
                    reader.Read();
                }
                reader.MoveToContent();
            }
            section.Children = pages;
            reader.ReadEndElement();
            return section;
        }

        private Page ParsePage(XmlReader reader, Section parent)
        {
            var page = new Page();
            page.Parent = parent;
            page.Notebook = parent.Notebook;

            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case Attributes.ID:
                        page.Id = reader.Value;
                        break;
                    case Attributes.Name:
                        page.Name = reader.Value;
                        break;
                    case Attributes.LastModifiedTime:
                        page.LastModified = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;
                    case Attributes.IsUnread:
                        page.IsUnread = bool.Parse(reader.Value);
                        break;
                    case Attributes.DateTime:
                        page.Created = DateTime.Parse(reader.Value);
                        break;
                    case Attributes.PageLevel:
                        page.Level = int.Parse(reader.Value);
                        break;
                    case Attributes.IsInRecycleBin:
                        page.IsInRecycleBin = bool.Parse(reader.Value);
                        break;
                }
            }

            page.RelativePath = $"{parent.RelativePath}{RelativePathSeparatorString}{page.Name}";

            reader.Skip();
            return page;
        }
    }
}