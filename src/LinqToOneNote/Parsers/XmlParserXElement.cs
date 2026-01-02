using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using LinqToOneNote.Abstractions;
using LinqToOneNote.Internal;

namespace LinqToOneNote.Parsers
{
    using static Constants;

    internal class XmlParserXElement : IXmlParser
    {
        // XName are atomic, this is for ease
        private static readonly XName NotebookXName = XName.Get(Elements.Notebook, NamespaceUri);
        private static readonly XName SectionGroupXName = XName.Get(Elements.SectionGroup, NamespaceUri);
        private static readonly XName SectionXName = XName.Get(Elements.Section, NamespaceUri);
        private static readonly XName PageXName = XName.Get(Elements.Page, NamespaceUri);
        private static readonly XName OpenSectionsXName = XName.Get(Elements.OpenSections, NamespaceUri);

        public Root ParseRoot(string xml)
        {
            XElement rootElement = XElement.Parse(xml);
            IEnumerable<XElement> notebookElements = rootElement.Elements(NotebookXName);
            var root = new Root();
            root.notebooks = [.. notebookElements.Select(e =>
                {
                    var notebook = Parse(new Notebook(), e, null);
                    notebook.root = root;
                    return notebook;
                })];
            XElement openSectionsElement = rootElement.Element(OpenSectionsXName);
            root.OpenSections = openSectionsElement == null
                ? null
                : new OpenSections()
                {
                    Id = openSectionsElement.Attribute(Attributes.ID).Value,
                    sections =
                    [
                        .. openSectionsElement.Elements()
                                              .Select(e => Parse(new Section(), e, null))
                    ]
                };
            return root;
        }

        public IOneNoteItem Parse(string xml, IOneNoteItem parent)
        {
            var element = XElement.Parse(xml);
            return element.Name switch
            {
                _ when element.Name == NotebookXName => Parse(new Notebook(), element, parent),
                _ when element.Name == SectionGroupXName => Parse(new SectionGroup(), element, parent),
                _ when element.Name == SectionXName => Parse(new Section(), element, parent),
                _ when element.Name == PageXName => Parse(new Page(), element, parent),
                _ => throw Exceptions.InvalidXmlElement(element.Name.LocalName),
            };
        }

        public void ParseExisting(string xml, IOneNoteItem item)
        {
            var element = XElement.Parse(xml);
            switch (item)
            {
                case Notebook notebook:
                    Parse(notebook, element, null);
                    break;
                case SectionGroup sectionGroup:
                    Parse(sectionGroup, element, sectionGroup.Parent);
                    break;
                case Section section:
                    Parse(section, element, section.Parent);
                    break;
                case Page page:
                    Parse(page, element, page.Parent);
                    break;
                default:
                    throw Exceptions.InvalidIOneNoteItem(item);
            }
        }

        private static T Parse<T>(T item, XElement element, IOneNoteItem parent) where T : OneNoteItem
        {
            SetAttributes(item, element.Attributes());
            if (typeof(T) == typeof(Notebook))
            {
                var notebook = Unsafe.As<Notebook>(item);
                foreach (var child in element.Elements())
                {
                    if (child.Name == SectionXName)
                        notebook.sections.Add(Parse(new Section(), child, notebook));
                    else if (child.Name == SectionGroupXName)
                        notebook.sectionGroups.Add(Parse(new SectionGroup(), child, notebook));
                }
            }
            else if (typeof(T) == typeof(SectionGroup))
            {
                var sectionGroup = Unsafe.As<SectionGroup>(item);
                foreach (var child in element.Elements())
                {
                    if (child.Name == SectionXName)
                        sectionGroup.sections.Add(Parse(new Section(), child, sectionGroup));
                    else if (child.Name == SectionGroupXName)
                        sectionGroup.sectionGroups.Add(Parse(new SectionGroup(), child, sectionGroup));
                }
                sectionGroup.Parent = (INotebookOrSectionGroup)parent;
            }
            else if (typeof(T) == typeof(Section))
            {
                var section = Unsafe.As<Section>(item);
                section.pages = [.. element.Elements().Select(e => Parse(new Page(), e, section))];
                section.Parent = (INotebookOrSectionGroup)parent;
            }
            else if (typeof(T) == typeof(Page))
            {
                var page = Unsafe.As<Page>(item);
                page.Parent = (Section)parent;
            }
            return item;
        }

        private static void SetAttributes<T>(T item, IEnumerable<XAttribute> attributes) where T : OneNoteItem
        {
            foreach (var attribute in attributes)
            {
                switch (attribute.Name.LocalName)
                {
                    case Attributes.ID:
                        item.id = attribute.Value;
                        break;
                    case Attributes.Name:
                        item.name = attribute.Value;
                        break;
                    case Attributes.IsUnread:
                        item.isUnread = (bool)attribute;
                        break;
                    case Attributes.LastModifiedTime:
                        item.lastModified = (DateTime)attribute;
                        break;
                    case Attributes.Path:
                        if (typeof(T) == typeof(Section))
                            Unsafe.As<Section>(item).Path = attribute.Value;
                        else if (typeof(T) == typeof(SectionGroup))
                            Unsafe.As<SectionGroup>(item).Path = attribute.Value;
                        else if (typeof(T) == typeof(Notebook))
                            Unsafe.As<Notebook>(item).Path = attribute.Value;
                        break;
                    case Attributes.Color:
                        if (typeof(T) == typeof(Section))
                            Unsafe.As<Section>(item).Color = GetColor(attribute.Value);
                        else if (typeof(T) == typeof(Notebook))
                            Unsafe.As<Notebook>(item).Color = GetColor(attribute.Value);
                        break;
                    case Attributes.IsInRecycleBin:
                        if (typeof(T) == typeof(Page))
                            Unsafe.As<Page>(item).IsInRecycleBin = bool.Parse(attribute.Value);
                        else if (typeof(T) == typeof(Section))
                            Unsafe.As<Section>(item).IsInRecycleBin = bool.Parse(attribute.Value);
                        break;
                    case Attributes.NickName:
                        Unsafe.As<Notebook>(item).DisplayName = attribute.Value;
                        break;
                    case Attributes.IsRecycleBin:
                        Unsafe.As<SectionGroup>(item).IsRecycleBin = (bool)attribute;
                        break;
                    case Attributes.Encrypted:
                        Unsafe.As<Section>(item).Encrypted = (bool)attribute;
                        break;
                    case Attributes.Locked:
                        Unsafe.As<Section>(item).Locked = (bool)attribute;
                        break;
                    case Attributes.IsDeletedPages:
                        Unsafe.As<Section>(item).IsDeletedPages = (bool)attribute;
                        break;
                    case Attributes.PageLevel:
                        Unsafe.As<Page>(item).Level = (int)attribute;
                        break;
                    case Attributes.DateTime:
                        Unsafe.As<Page>(item).Created = (DateTime)attribute;
                        break;
                }
            }
        }
    }
}
