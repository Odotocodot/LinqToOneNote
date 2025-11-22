using Odotocodot.OneNote.Linq.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Odotocodot.OneNote.Linq.Parsers
{
    using static Constants;

    internal class XmlParserXElement : IXmlParser
    {
        // XName are atomic, this is for ease
        private static readonly XName NotebookXName = XName.Get(Elements.Notebook, NamespaceUri);
        private static readonly XName SectionGroupXName = XName.Get(Elements.SectionGroup, NamespaceUri);
        private static readonly XName SectionXName = XName.Get(Elements.Section, NamespaceUri);
        private static readonly XName PageXName = XName.Get(Elements.Page, NamespaceUri);

        private static readonly Dictionary<XName, Func<XElement, IOneNoteItem, IOneNoteItem>> runtimeParser =
            new Dictionary<XName, Func<XElement, IOneNoteItem, IOneNoteItem>>
        {
            { NotebookXName, (element, parent) => Parse(new Notebook(), element, parent) },
            { SectionGroupXName, (element, parent) => Parse(new SectionGroup(), element, parent) },
            { SectionXName, (element, parent) => Parse(new Section(), element, parent) },
            { PageXName, (element, parent) => Parse(new Page(), element, parent) }
        };

        public IEnumerable<Notebook> ParseNotebooks(string xml) => XElement.Parse(xml).Elements()
                                                                                             .Select(e => Parse(new Notebook(), e, null));

        public IOneNoteItem ParseUnknown(string xml, IOneNoteItem parent)
        {
            var root = XElement.Parse(xml);
            return runtimeParser[root.Name](root, parent);
        }

        private static T Parse<T>(T item, XElement element, IOneNoteItem parent) where T : OneNoteItem
        {
            SetAttributes(item, element.Attributes());
            item.Parent = parent;
            item.Notebook = parent?.Notebook;
            item.RelativePath = $"{parent?.RelativePath}{RelativePathSeparatorString}{item.Name}";
            item.Children = element.Elements().Select(e => runtimeParser[e.Name](e, item));
            return item;
        }

        private static void SetAttributes(OneNoteItem item, IEnumerable<XAttribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                switch (attribute.Name.LocalName)
                {
                    case Attributes.ID:
                        item.Id = attribute.Value;
                        break;
                    case Attributes.Name:
                        item.Name = attribute.Value;
                        break;
                    case Attributes.IsUnread:
                        item.IsUnread = (bool)attribute;
                        break;
                    case Attributes.LastModifiedTime:
                        item.LastModified = (DateTime)attribute;
                        break;
                    case Attributes.Path:
                        ((IWritablePath)item).Path = attribute.Value;
                        break;
                    case Attributes.Color:
                        ((IWritableColor)item).Color = GetColor(attribute.Value);
                        break;
                    case Attributes.IsInRecycleBin:
                        ((IWritableIsInRecycleBin)item).IsInRecycleBin = (bool)attribute;
                        break;
                    case Attributes.NickName:
                        ((Notebook)item).NickName = attribute.Value;
                        break;
                    case Attributes.IsRecycleBin:
                        ((SectionGroup)item).IsRecycleBin = (bool)attribute;
                        break;
                    case Attributes.Encrypted:
                        ((Section)item).Encrypted = (bool)attribute;
                        break;
                    case Attributes.Locked:
                        ((Section)item).Locked = (bool)attribute;
                        break;
                    case Attributes.IsDeletedPages:
                        ((Section)item).IsDeletedPages = (bool)attribute;
                        break;
                    case Attributes.PageLevel:
                        ((Page)item).Level = (int)attribute;
                        break;
                    case Attributes.DateTime:
                        ((Page)item).Created = (DateTime)attribute;
                        break;
                }
            }
        }
    }
}
