using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using LinqToOneNote.Abstractions;
using LinqToOneNote.Internal;
using static LinqToOneNote.Parsers.Constants;


namespace LinqToOneNote.Parsers
{
    internal class XmlParserXmlReader : IXmlParser
    {
        public Root ParseRoot(string xml)
        {
            using var stringReader = new StringReader(xml);
            using var reader = XmlReader.Create(stringReader);
            reader.MoveToContent();
            return ParseRoot(reader);
        }

        public IOneNoteItem Parse(string xml, IOneNoteItem parent)
        {
            using var stringReader = new StringReader(xml);
            using var reader = XmlReader.Create(stringReader);
            reader.MoveToContent();
            if (reader.NodeType == XmlNodeType.Element)
            {
                return reader.LocalName switch
                {
                    Elements.Notebook => ParseNotebook(reader, new Notebook(), null),
                    Elements.Section => ParseSection(reader, new Section(), (INotebookOrSectionGroup)parent),
                    Elements.SectionGroup => ParseSectionGroup(reader, new SectionGroup(), (INotebookOrSectionGroup)parent),
                    Elements.Page => ParsePage(reader, new Page(), (Section)parent),
                    _ => throw Exceptions.InvalidXmlElement(reader.LocalName),
                };
            }
            throw Exceptions.InvalidXmlNodeType(reader.NodeType);
        }

        public void ParseExisting(string xml, IOneNoteItem item)
        {
            using var stringReader = new StringReader(xml);
            using var reader = XmlReader.Create(stringReader);
            reader.MoveToContent();
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (item)
                {
                    case Notebook notebook:
                        ParseNotebook(reader, notebook, notebook.root);
                        break;
                    case Section section:
                        ParseSection(reader, section, section.Parent);
                        break;
                    case SectionGroup sectionGroup:
                        ParseSectionGroup(reader, sectionGroup, sectionGroup.Parent);
                        break;
                    case Page page:
                        ParsePage(reader, page, page.Parent);
                        break;
                    default:
                        throw Exceptions.InvalidIOneNoteItem(item);
                }
            }
        }

        private static Root ParseRoot(XmlReader reader)
        {
            var root = new Root();
            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return root;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case Elements.Notebook:
                            root.notebooks.Add(ParseNotebook(reader, new Notebook(), root));
                            break;
                        case Elements.OpenSections:
                            root.OpenSections = ParseOpenSections(reader);
                            break;
                        default:
                            reader.Read();
                            break;
                    }
                }
                else
                {
                    reader.Read();
                }
            }

            reader.ReadEndElement();
            return root;
        }

        private static OpenSections ParseOpenSections(XmlReader reader)
        {
            var openSections = new OpenSections();
            while (reader.MoveToNextAttribute())
            {
                if (reader.LocalName == Attributes.ID)
                {
                    openSections.Id = reader.Value;
                }
            }

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return openSections;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == Elements.Section)
                {
                    openSections.sections.Add(ParseSection(reader, new Section(), null));
                }
                else
                {
                    reader.Read();
                }
                reader.MoveToContent();
            }

            reader.ReadEndElement();
            return openSections;
        }

        private static Notebook ParseNotebook(XmlReader reader, Notebook notebook, Root root)
        {
            // reader.MoveToContent();
            notebook.root = root;
            SetAttributes(notebook, reader);

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return notebook;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            ParseChildren(reader, notebook, notebook.sections, notebook.sectionGroups);
            reader.ReadEndElement();
            return notebook;
        }

        private static void ParseChildren(XmlReader reader, INotebookOrSectionGroup parent, ReadOnlyList<Section> sections, ReadOnlyList<SectionGroup> sectionGroups)
        {
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case Elements.Section:
                            sections.Add(ParseSection(reader, new Section(), parent));
                            break;
                        case Elements.SectionGroup:
                            sectionGroups.Add(ParseSectionGroup(reader, new SectionGroup(), parent));
                            break;
                        default:
                            reader.Read();
                            break;
                    }
                }
                else
                {
                    reader.Read();
                }
                reader.MoveToContent();
            }
        }

        private static SectionGroup ParseSectionGroup(XmlReader reader, SectionGroup sectionGroup, INotebookOrSectionGroup parent)
        {
            sectionGroup.Parent = parent;
            SetAttributes(sectionGroup, reader);

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return sectionGroup;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            ParseChildren(reader, sectionGroup, sectionGroup.sections, sectionGroup.sectionGroups);
            reader.ReadEndElement();
            return sectionGroup;
        }

        private static Section ParseSection(XmlReader reader, Section section, INotebookOrSectionGroup parent)
        {
            section.Parent = parent;
            SetAttributes(section, reader);

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                return section;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == Elements.Page)
                {
                    section.pages.Add(ParsePage(reader, new Page(), section));
                }
                else
                {
                    reader.Read();
                }
                reader.MoveToContent();
            }
            reader.ReadEndElement();
            return section;
        }

        private static Page ParsePage(XmlReader reader, Page page, Section parent)
        {
            page.Parent = parent;
            SetAttributes(page, reader);
            reader.Skip();
            return page;
        }

        private static void SetAttributes<T>(T item, XmlReader reader) where T : OneNoteItem
        {
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case Attributes.ID:
                        item.id = reader.Value;
                        break;
                    case Attributes.Name:
                        item.name = reader.Value;
                        break;
                    case Attributes.LastModifiedTime:
                        item.lastModified = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;
                    case Attributes.IsUnread:
                        item.isUnread = bool.Parse(reader.Value);
                        break;
                    case Attributes.Path:
                        if (typeof(T) == typeof(Section))
                            Unsafe.As<Section>(item).Path = reader.Value;
                        else if (typeof(T) == typeof(SectionGroup))
                            Unsafe.As<SectionGroup>(item).Path = reader.Value;
                        else if (typeof(T) == typeof(Notebook))
                            Unsafe.As<Notebook>(item).Path = reader.Value;
                        break;
                    case Attributes.Color:
                        if (typeof(T) == typeof(Section))
                            Unsafe.As<Section>(item).Color = GetColor(reader.Value);
                        else if (typeof(T) == typeof(Notebook))
                            Unsafe.As<Notebook>(item).Color = GetColor(reader.Value);
                        break;
                    case Attributes.IsInRecycleBin:
                        if (typeof(T) == typeof(Page))
                            Unsafe.As<Page>(item).IsInRecycleBin = bool.Parse(reader.Value);
                        else if (typeof(T) == typeof(Section))
                            Unsafe.As<Section>(item).IsInRecycleBin = bool.Parse(reader.Value);
                        break;
                    case Attributes.NickName:
                        Unsafe.As<Notebook>(item).DisplayName = reader.Value;
                        break;
                    case Attributes.IsRecycleBin:
                        Unsafe.As<SectionGroup>(item).IsRecycleBin = bool.Parse(reader.Value);
                        break;
                    case Attributes.Encrypted:
                        Unsafe.As<Section>(item).Encrypted = bool.Parse(reader.Value);
                        break;
                    case Attributes.Locked:
                        Unsafe.As<Section>(item).Locked = bool.Parse(reader.Value);
                        break;
                    case Attributes.IsDeletedPages:
                        Unsafe.As<Section>(item).IsDeletedPages = bool.Parse(reader.Value);
                        break;
                    case Attributes.PageLevel:
                        Unsafe.As<Page>(item).Level = int.Parse(reader.Value);
                        break;
                    case Attributes.DateTime:
                        Unsafe.As<Page>(item).Created = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;
                }
            }
        }
    }
}