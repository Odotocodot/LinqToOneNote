using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;


namespace Odotocodot.OneNote.Linq.Parsers
{
    using static Constants;

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
                    Elements.Notebook => ParseNotebook(reader),
                    Elements.Section => ParseSection(reader, (INotebookOrSectionGroup)parent),
                    Elements.SectionGroup => ParseSectionGroup(reader, (INotebookOrSectionGroup)parent),
                    Elements.Page => ParsePage(reader, (Section)parent),
                    _ => null,
                };
            }
            return null;
        }

        private static UnfiledNotes ParseUnfiledNotes(XmlReader reader)
        {
            var unfiledNotes = new UnfiledNotes();
            while (reader.MoveToNextAttribute())
            {
                if (reader.LocalName == Attributes.ID)
                {
                    unfiledNotes.Id = reader.Value;
                }
            }

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                unfiledNotes.Section = null;
                return unfiledNotes;
            }
            reader.ReadStartElement();
            reader.MoveToContent();
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == Elements.Section)
            {
                unfiledNotes.Section = ParseSection(reader, null);
            }
            reader.ReadEndElement();
            return unfiledNotes;
        }

        private static OpenSections ParseOpenSections(XmlReader reader)
        {
            var openSections = new OpenSections();
            var sections = new List<Section>();
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
                openSections.Sections = [];
                return openSections;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == Elements.Section)
                {
                    sections.Add(ParseSection(reader, null));
                }
                else
                {
                    reader.Read();
                }
                reader.MoveToContent();
            }

            reader.ReadEndElement();
            openSections.Sections = sections;
            return openSections;
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

            var notebooks = new List<Notebook>();
            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case Elements.Notebook:
                            notebooks.Add(ParseNotebook(reader));
                            break;
                        case Elements.UnfiledNotes:
                            root.UnfiledNotes = ParseUnfiledNotes(reader);
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
            root.Notebooks = notebooks;
            return root;
        }

        private static Notebook ParseNotebook(XmlReader reader)
        {
            var notebook = new Notebook();
            // reader.MoveToContent();
            SetAttributes(notebook, reader);

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                notebook.Children = [];
                notebook.Sections = [];
                notebook.SectionGroups = [];
                return notebook;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            ParseChildren(reader, notebook, out List<Section> sections, out List<SectionGroup> sectionGroups, out List<IOneNoteItem> children);
            notebook.Sections = sections;
            notebook.SectionGroups = sectionGroups;
            notebook.Children = children;
            reader.ReadEndElement();
            return notebook;
        }

        private static void ParseChildren(XmlReader reader, INotebookOrSectionGroup parent, out List<Section> sections, out List<SectionGroup> sectionGroups, out List<IOneNoteItem> children)
        {
            sections = [];
            sectionGroups = [];
            while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case Elements.Section:
                            sections.Add(ParseSection(reader, parent));
                            break;
                        case Elements.SectionGroup:
                            sectionGroups.Add(ParseSectionGroup(reader, parent));
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

            children = [.. sections, .. sectionGroups];
        }

        private static SectionGroup ParseSectionGroup(XmlReader reader, INotebookOrSectionGroup parent)
        {
            var sectionGroup = new SectionGroup();
            sectionGroup.Parent = parent;
            //sectionGroup.Notebook = parent.Notebook;
            SetAttributes(sectionGroup, reader);

            //sectionGroup.RelativePath = $"{parent.RelativePath}{RelativePathSeparatorString}{sectionGroup.Name}";

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                sectionGroup.Children = [];
                sectionGroup.Sections = [];
                sectionGroup.SectionGroups = [];
                return sectionGroup;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            ParseChildren(reader, sectionGroup, out List<Section> sections, out List<SectionGroup> sectionGroups, out List<IOneNoteItem> children);
            sectionGroup.Sections = sections;
            sectionGroup.SectionGroups = sectionGroups;
            sectionGroup.Children = children;
            reader.ReadEndElement();
            return sectionGroup;
        }

        private static Section ParseSection(XmlReader reader, INotebookOrSectionGroup parent)
        {
            var section = new Section();
            section.Parent = parent;
            //section.Notebook = parent.Notebook;
            SetAttributes(section, reader);

            //section.RelativePath = $"{parent.RelativePath}{RelativePathSeparatorString}{section.Name}";

            reader.MoveToElement();
            if (reader.IsEmptyElement)
            {
                reader.Skip();
                section.Pages = [];
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
            section.Pages = pages;
            reader.ReadEndElement();
            return section;
        }

        private static Page ParsePage(XmlReader reader, Section parent)
        {
            var page = new Page();
            page.Section = parent;
            //page.Notebook = parent.Notebook;
            SetAttributes(page, reader);

            //page.RelativePath = $"{parent.RelativePath}{RelativePathSeparatorString}{page.Name}";

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
                        item.Id = reader.Value;
                        break;
                    case Attributes.Name:
                        item.Name = reader.Value;
                        break;
                    case Attributes.LastModifiedTime:
                        item.LastModified = DateTime.Parse(reader.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        break;
                    case Attributes.IsUnread:
                        item.IsUnread = bool.Parse(reader.Value);
                        break;
                    case Attributes.Path:
                        Unsafe.As<IWritablePath>(item).Path = reader.Value;
                        break;
                    case Attributes.Color:
                        Unsafe.As<IWritableColor>(item).Color = GetColor(reader.Value);
                        break;
                    case Attributes.IsInRecycleBin:
                        Unsafe.As<IWritableIsInRecycleBin>(item).IsInRecycleBin = bool.Parse(reader.Value);
                        break;
                    case Attributes.NickName:
                        Unsafe.As<Notebook>(item).NickName = reader.Value;
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
                        Unsafe.As<Page>(item).Created = DateTime.Parse(reader.Value);
                        break;
                }

            }
        }
    }
}