using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Office.Interop.OneNote;
using Odotocodot.OneNote.Linq.Abstractions;
using Odotocodot.OneNote.Linq.Internal;
using Odotocodot.OneNote.Linq.Parsers;

namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// A static wrapper class around the <see cref="Application"/> class, allowing for <see cref="Lazy{T}">lazy</see> acquirement and
    /// release of a OneNote COM object. In addition to exposing the
    /// <a href="https://learn.microsoft.com/en-us/office/client-developer/onenote/application-interface-onenote"> OneNote's API</a>
    /// </summary>
    /// <remarks>A <see cref="Application">OneNote COM object</see> is required to access any of the OneNote API.</remarks>
    public static class OneNote
    {
        /// <summary>
        /// The directory separator used in <see cref="IOneNoteItem.RelativePath"/>.
        /// </summary>
        //TODO Remove
        public const char RelativePathSeparator = Constants.RelativePathSeparator;

        // You never know they might add a new one... (Press X to doubt)
        // As the docs say:
        // NOTE: We recommend specifying a version of OneNote (such as xs2013) instead of using xsCurrent or leaving it blank, because this will allow your add-in to work with future versions of OneNote.
        private const XMLSchema xmlSchema = XMLSchema.xs2013;

        private static readonly XmlParserXmlReader xmlParser = new();

        #region COM Object Management
        public static ComObjectMode ComObjectMode { get; private set; } = ComObjectMode.Lazy;

        private static Application application;

        private static readonly Lock comLock = new();

        /// <summary>
        /// Use this only if you know what you are doing.
        /// The COM Object instance of the OneNote application.
        /// </summary>
        /// <seealso cref="HasComObject"/>
        /// <seealso cref="InitComObject"/>
        /// <seealso cref="ReleaseComObject"/>
        public static Application ComObject => application;

        /// <summary>
        /// Indicates whether the class has a usable <see cref="Application">COM Object instance</see>.
        /// </summary>
        /// <remarks>When <see langword="true"/> a "Microsoft OneNote" process should be visible in the Task Manager.</remarks>
        /// <seealso cref="InitComObject"/>
        /// <seealso cref="ReleaseComObject"/>
        public static bool HasComObject => application != null;

        public static void SetComObjectMode(ComObjectMode mode) => ComObjectMode = mode;

        /// <summary>
        /// Forcible initialises the static class by acquiring a <see cref="Application">OneNote COM object</see>. Does nothing if a COM object is already accessible.
        /// </summary>
        /// <exception cref="COMException">Thrown if an error occurred when trying to get the 
        /// <see cref="Application">OneNote COM object</see>.</exception>
        /// <seealso cref="HasComObject"/>
        /// <seealso cref="ReleaseComObject"/>
        public static void InitComObject()
        {
            lock (comLock)
            {
                application ??= new Application();
            }
        }

        /// <summary>
        /// Releases the <see cref="Application">OneNote COM object</see> freeing memory.
        /// </summary>
        /// <seealso cref="InitComObject"/>
        /// <seealso cref="HasComObject"/>
        public static void ReleaseComObject()
        {
            lock (comLock)
            {
                if (application != null)
                {
                    var count = Marshal.ReleaseComObject(application);
                    Debug.Assert(count == 0, "COM Object reference count should be zero after release.");
                    application = null;
                }
            }
        }

        #endregion

        #region OneNote API Methods

        /// <summary>
        /// Get the full OneNote hierarchy.
        /// </summary>
        /// <returns>Returns a <see cref="Root"/> object which contains the OneNote hierarchy.</returns>
        public static Root GetFullHierarchy()
        {
            string xml = Run(app =>
            {
                app.GetHierarchy(null, HierarchyScope.Pages.ToInterop(), out string xml, xmlSchema);
                return xml;
            });
            return xmlParser.ParseRoot(xml);
        }

        /// <summary>
        /// Get a flattened collection of <see cref="Page">pages</see> that match the <paramref name="search"/> parameter.
        /// </summary>
        /// <param name="search">The search query. This should be exactly the same string that you would type into the search box in the OneNote UI. You can use bitwise operators, such as "AND" and "OR", which must be all uppercase.</param>
        /// <returns>An <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="Page"/>&gt; that contains <see cref="Page">pages</see> that match the <paramref name="search"/> parameter.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="search"/> is empty or only whitespace, or if the first character of <paramref name="search"/> is NOT a letter or a digit.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="search"/> is <see langword="null"/>.</exception>
        /// <seealso cref="FindPages(string, IOneNoteItem)"/>
        public static IEnumerable<Page> FindPages(string search)
        {
            Throw.IfInvalidSearch(search);
            var xml = Run(app =>
            {
                app.FindPages(null, search, out string xml, xsSchema: xmlSchema);
                return xml;
            });
            return xmlParser.ParseRoot(xml).GetAllPages();
        }

        /// <summary>
        /// <inheritdoc cref="FindPages(string)" path="/summary"/> Within the specified <paramref name="scope"/>.
        /// </summary>
        /// <param name="search"><inheritdoc cref="FindPages(string)" path="/param[@name='search']"/></param>
        /// <param name="scope">The hierarchy item to search within.</param>
        /// <returns><inheritdoc cref="FindPages(string)" path="/returns"/></returns>
        /// <seealso cref="FindPages(string)"/>
        /// <exception cref="ArgumentException"><inheritdoc cref="FindPages(string)" path="/exception[@cref='ArgumentException']"/></exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="search"/> or <paramref name="scope"/> is <see langword="null"/>.</exception>
        public static IEnumerable<Page> FindPages(string search, IOneNoteItem scope)
        {
            Throw.IfNull(scope);
            Throw.IfInvalidSearch(search);

            var xml = Run(app =>
            {
                app.FindPages(scope.Id, search, out string xml, xsSchema: xmlSchema);
                return xml;
            });
            return xmlParser.Parse(xml, scope).GetAllPages();
        }

        /// <summary>
        /// Opens the <paramref name="item"/> in OneNote. If there is no OneNote window a new one is created, else whether a new window is created is
        /// defined by <paramref name="newWindow"/>.
        /// </summary>
        /// <param name="item">The item to open</param>
        /// <param name="newWindow">Whether to create a new OneNote window or add to an existing one. Does nothing if there are no windows of OneNote.</param>
        public static void Open(INavigable item, bool newWindow = false) => Run(app => app.NavigateTo(item.Id, fNewWindow: newWindow));

        /// <summary>
        /// Forces OneNote to sync the <paramref name="item"/>.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Open" path="/param[@name='item']"/></param>
        public static void SyncItem(INavigable item) => Run(app => app.SyncHierarchy(item.Id));

        /// <summary>
        /// Gets the content of the specified <paramref name="page"/>.
        /// </summary>
        /// <param name="page">The page to retrieve content from.</param>
        /// <returns>An <see langword="string"/> in the OneNote XML format.</returns>
        public static string GetPageContent(Page page) =>
            Run(app =>
            {
                app.GetPageContent(page.Id, out string xml, xsSchema: xmlSchema);
                return xml;
            });

        /// <summary>
        /// Updates the content of a OneNote page with the provided <paramref name="xml"/>. 
        /// The chosen page depends on the ID provided in the <paramref name="xml"/>. 
        /// An example can be seen <a href="https://learn.microsoft.com/en-us/office/client-developer/onenote/application-interface-onenote#updatepagecontent-method">here</a> at the Microsoft OneNote API documentation.
        /// </summary>
        /// <remarks>The <paramref name="xml"/> must match the OneNote XML format, the schema can be
        /// found <a href="https://github.com/idvorkin/onom/blob/eb9ce52764e9ad639b2c9b4bca0622ee6221106f/OneNoteObjectModel/onenote.xsd">here</a>.</remarks>
        /// <param name="xml">An <see langword="string"/> in the OneNote XML format. </param>
        public static void UpdatePageContent(string xml) => Run(app => app.UpdatePageContent(xml, xsSchema: xmlSchema));

        /// <summary>
        /// Deletes the hierarchy <paramref name="item"/> from the OneNote notebook hierarchy. For <see cref="Notebook">notebooks</see> use
        /// <see cref="CloseNotebook"/>. Does nothing if the <paramref name="item"/> is already in the Recycle Bin.
        /// </summary>
        /// <param name="item">The item to be deleted</param>
        /// <param name="dateExpectedLastModified">The date and time that you think the object you want to delete was last modified. If you pass a
        /// non-zero value for this parameter, OneNote proceeds with the update only if the value you pass matches the actual date and time the object
        /// was last modified. Passing a value for this parameter helps prevent accidentally overwriting edits users made since the last time the
        /// object was modified.</param>
        /// <param name="deletePermanently"><see langword="true"/> to permanently delete the item; <see langword="false"/> to move the item into
        /// the OneNote recycle bin for the corresponding Notebook (the default). If the Notebook is in OneNote 2007 format, no recycle bin exists, so
        /// the content is permanently deleted.</param>
        public static void DeleteItem(IDeletable item, DateTime dateExpectedLastModified = default, bool deletePermanently = false)
        {
            Throw.IfNull(item);
            if (item.IsInRecycleBin())
            {
                return;
            }
            Run(app => app.DeleteHierarchy(item.Id, dateExpectedLastModified, deletePermanently));
            ((ReadOnlyList)item.Parent.Children).Remove(item);
        }

        /// <summary>
        /// Causes OneNote to synchronizes any offline files with the <see cref="notebook">notebook</see>, if necessary, and then closes the specified
        /// notebook. After the method returns, the notebook no longer appears in the list of open notebooks in the OneNote user interface (UI).
        /// </summary>
        /// <param name="notebook">The specified OneNote notebook.</param>
        /// <param name="force"><see langword="true"/> to close the notebook, even if there are changes in the notebook that OneNote cannot sync
        /// before closing; otherwise, <see langword="false"/>.</param>
        public static void CloseNotebook(Notebook notebook, bool force = false) => Run(app => app.CloseNotebook(notebook.Id, force));

        /// <summary>
        /// Renames the specified <paramref name="item"/> to <paramref name="newName"/>.
        /// For <see cref="Notebook">notebooks</see>, this renames its <see cref="Notebook.DisplayName">display name</see>, not the folder on disk.
        /// </summary>
        /// <param name="item">The item to be renamed</param>
        /// <param name="newName">The new name of the item</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="item"/> is in the Recycle Bin.<br/> Thrown if <paramref name="newName"></paramref>
        /// is invalid for the <paramref name="item"/> type. See <see cref="IsValidName"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is null.</exception>
        public static void RenameItem(IOneNoteItem item, string newName)
        {
            Throw.IfNull(item);
            Throw.IfNullOrWhiteSpace(newName);
            Throw.IfInRecycleBin(item, "Cannot rename items in the Recycle Bin.");
            switch (item) //Don't need to check notebook as its renaming the DisplayName which can be anything
            {
                case SectionGroup:
                    Throw.IfInvalidName<SectionGroup>(newName);
                    break;
                case Section:
                    Throw.IfInvalidName<Section>(newName);
                    break;
            }
            Run(app =>
            {
                app.GetHierarchy(item.Id, HierarchyScope.Self.ToInterop(), out string xml, xmlSchema);
                var element = XElement.Parse(xml);
                switch (item)
                {
                    case Notebook notebook:
                        element.Attribute(Constants.Attributes.NickName)!.SetValue(newName);
                        notebook.DisplayName = newName;
                        break;
                    case Page page:
                        app.GetPageContent(item.Id, out string pageContentXml, PageInfo.piBasic, xmlSchema);
                        XDocument doc = XDocument.Parse(pageContentXml);
                        XElement xTitle = doc.Descendants(XName.Get("T", Constants.NamespaceUri)).First();
                        xTitle.Value = newName;
                        page.Name = newName;
                        app.UpdatePageContent(doc.ToString());
                        break;
                    default:
                        element.Attribute(Constants.Attributes.Name)!.SetValue(newName);
                        ((OneNoteItem)item).Name = newName;
                        break;
                }
                app.UpdateHierarchy(element.ToString(), xmlSchema);
            });
        }

        #region Creating New OneNote Items Methods

        /// <summary>
        /// Creates a <see cref="Page">page</see> with a title equal to <paramref name="name"/> located in the specified <paramref name="section"/>.<br/>
        /// </summary>
        /// <param name="section">The section to create the page in.</param>
        /// <param name="name">The title of the page.</param>
        /// <param name="openMode">Specifies whether/how the newly created page should be opened.</param>
        /// <returns>The newly created <see cref="Page">page</see>.</returns>
        public static Page CreatePage(Section section, string name = "", OpenMode openMode = OpenMode.None)
        {
            Throw.IfInvalidParent(section, $"Use {nameof(OneNote)}.{nameof(CreateQuickNote)} instead.");

            return Run(app =>
            {
                app.SyncHierarchy(section.Id);
                app.CreateNewPage(section.Id, out string pageId, NewPageStyle.npsBlankPageWithTitle);
                app.GetPageContent(pageId, out string pageContentXml, PageInfo.piBasic, xmlSchema);

                XDocument doc = XDocument.Parse(pageContentXml);
                XElement xTitle = doc.Descendants(XName.Get("T", Constants.NamespaceUri)).First();
                xTitle.Value = name;
                app.UpdatePageContent(doc.ToString());
                app.GetHierarchy(pageId, HierarchyScope.Self.ToInterop(), out var pageXml, xmlSchema);
                var page = (Page)xmlParser.Parse(pageXml, section);
                section.pages.Add(page);
                UseOpenMode(app, openMode, page.Id);
                return page;
            });
        }

        private static void UseOpenMode(Application application, OpenMode openMode, string id)
        {
            switch (openMode)
            {
                case OpenMode.ExistingOrNewWindow:
                    application.NavigateTo(id);
                    break;
                case OpenMode.NewWindow:
                    application.NavigateTo(id, fNewWindow: true);
                    break;
            }
        }

        /// <summary>
        /// Creates a quick note page located at the users quick/default notes location.
        /// </summary>
        /// <param name="openMode"><inheritdoc cref="CreatePage(Section, string, OpenMode)" path="/param[@name='openMode']"/></param>
        /// <returns>The <see cref="INavigable.Id"/> of the newly created quick note page.</returns>
        public static string CreateQuickNote(OpenMode openMode = OpenMode.None)
        {
            return Run(app =>
            {
                app.GetSpecialLocation(SpecialLocation.slUnfiledNotesSection, out string path);
                app.OpenHierarchy(path, null, out string sectionId, CreateFileType.cftNone);
                app.SyncHierarchy(sectionId);
                app.CreateNewPage(sectionId, out string pageId, NewPageStyle.npsDefault);

                UseOpenMode(app, openMode, pageId);
                return pageId;
            });
        }

        /// <summary>
        /// Creates a quick note page with the title specified by <paramref name="name"/>, located at the users quick/default notes location.
        /// </summary>
        /// <param name="name"><inheritdoc cref="CreatePage(Section, string, OpenMode)" path="/param[@name='name']"/></param>
        /// <param name="openMode"><inheritdoc cref="CreatePage(Section, string, OpenMode)" path="/param[@name='openMode']"/></param>
        /// <returns>The <see cref="INavigable.Id"/> of the newly created quick note page.</returns>
        public static string CreateQuickNote(string name, OpenMode openMode = OpenMode.None)
        {
            return Run(app =>
            {
                app.GetSpecialLocation(SpecialLocation.slUnfiledNotesSection, out string path);
                app.OpenHierarchy(path, null, out string sectionId, CreateFileType.cftNone);
                app.SyncHierarchy(sectionId);
                app.CreateNewPage(sectionId, out string pageId, NewPageStyle.npsBlankPageWithTitle);
                app.GetPageContent(pageId, out string pageContentXml, PageInfo.piBasic, xmlSchema);

                XDocument doc = XDocument.Parse(pageContentXml);
                XElement xTitle = doc.Descendants(XName.Get("T", Constants.NamespaceUri)).First();
                xTitle.Value = name;

                app.UpdatePageContent(doc.ToString());
                UseOpenMode(app, openMode, pageId);
                return pageId;
            });
        }

        private static T CreateItem<T>(Application app, IOneNoteItem parent,
            string name,
            string path,
            OpenMode openMode,
            CreateFileType createFileType) where T : INameInvalidCharacters
        {
            Throw.IfInvalidName<T>(name);

            if (typeof(T) != typeof(Notebook)) //Notebooks don't have a parent
            {
                Throw.IfInvalidParent(parent);
            }

            app.OpenHierarchy(path, parent?.Id, out string newItemId, createFileType);
            app.GetHierarchy(newItemId, HierarchyScope.Self.ToInterop(), out var itemXml, xmlSchema);
            T newItem = (T)xmlParser.Parse(itemXml, parent);
            UseOpenMode(app, openMode, newItemId);
            return newItem;
        }

        /// <summary>
        /// Creates a <see cref="Section">section</see> with a title equal to <paramref name="name"/> located in the specified <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The hierarchy item to create the section in.</param>
        /// <param name="name">The name of the new section.</param>
        /// <param name="openMode">Whether to open the newly created section in OneNote immediately.</param>
        /// <typeparam name="TNotebookOrSectionGroup">Represents a <see cref="Notebook">notebook</see> or a <see cref="SectionGroup">section group</see>.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="name"/> is not a valid section name.</exception>
        /// <seealso cref="IsValidName(string)"/>
        /// <returns>The newly created <see cref="Section">section</see>.</returns>
        public static Section CreateSection<TNotebookOrSectionGroup>(TNotebookOrSectionGroup parent, string name, OpenMode openMode = OpenMode.None) where TNotebookOrSectionGroup : INotebookOrSectionGroup
        {
            return Run(app =>
            {
                var section = CreateItem<Section>(app, parent, name, $"{name}.one", openMode, CreateFileType.cftSection);
                ((ReadOnlyList<Section>)parent.Sections).Add(section);
                return section;
            });
        }

        /// <summary>
        /// Creates a <see cref="SectionGroup">section group</see> with a title equal to <paramref name="name"/> located in the specified <paramref name="parent"/>.
        /// </summary>
        /// <param name="parent">The hierarchy item to create the section group in.</param>
        /// <param name="name">The name of the new section group.</param>
        /// <param name="openMode">Whether to open the newly created section group in OneNote immediately.</param>
        /// <typeparam name="TNotebookOrSectionGroup">Represents a <see cref="Notebook">notebook</see> or a <see cref="SectionGroup">section group</see>.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="name"/> is not a valid section group name.</exception>
        /// <seealso cref="IsValidName(string)"/>
        /// <returns>The newly created <see cref="SectionGroup">section group</see>.</returns>
        public static SectionGroup CreateSectionGroup<TNotebookOrSectionGroup>(TNotebookOrSectionGroup parent, string name, OpenMode openMode = OpenMode.None) where TNotebookOrSectionGroup : INotebookOrSectionGroup
        {
            return Run(app =>
            {
                var sectionGroup = CreateItem<SectionGroup>(app, parent, name, name, openMode, CreateFileType.cftFolder);
                ((ReadOnlyList<SectionGroup>)parent.SectionGroups).Add(sectionGroup);
                return sectionGroup;
            });
        }

        /// <summary>
        /// Creates a <see cref="Notebook">notebook</see> with a title equal to <paramref name="name"/> located in the <paramref name="directory"/>. If <paramref name="directory"/> is
        /// <see langword="null"/>, the <see cref="GetDefaultNotebookLocation">default notebook location</see> is used.
        /// </summary>
        /// <param name="name">The name of the new notebook.</param>
        /// <param name="directory">The directory to create the notebook</param>
        /// <param name="openMode">Whether to open the newly created notebook in OneNote immediately.</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="name"/> is not valid for a notebook.</exception>
        /// <exception cref="IOException">Thrown if the specified <paramref name="directory"/> is not <see langword="null"/> and does not exist.</exception>
        /// <returns>The newly created <see cref="Notebook">notebook</see>.</returns>
        /// <seealso cref="IsValidName(string)"/>
        /// <seealso cref="GetDefaultNotebookLocation"/>
        /// <seealso cref="CreateNotebook(string, OpenMode)"/>
        public static Notebook CreateNotebook(string name, string directory, OpenMode openMode = OpenMode.None)
        {
            return Run(app =>
            {
                if (string.IsNullOrWhiteSpace(directory))
                {
                    app.GetSpecialLocation(SpecialLocation.slDefaultNotebookFolder, out directory);
                }
                else if (!Directory.Exists(directory))
                {
                    throw Exceptions.DirectoryDoesNotExist(directory);
                }

                return CreateItem<Notebook>(app, null, name, Path.Combine(directory, name), openMode, CreateFileType.cftNotebook);
            });
        }

        /// <summary>
        /// Creates a <see cref="Notebook">notebook</see> with a title equal to <paramref name="name"/> located in the <see cref="GetDefaultNotebookLocation">default notebook location</see>.
        /// </summary>
        /// <param name="name"><inheritdoc cref="CreateNotebook(string,string,OpenMode)"/></param>
        /// <param name="openMode"><inheritdoc cref="CreateNotebook(string,string,OpenMode)"/></param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="name"/> is not valid for a notebook.</exception>
        /// <returns>The newly created <see cref="Notebook">notebook</see>.</returns>
        /// <seealso cref="IsValidName(string)"/>
        /// <seealso cref="GetDefaultNotebookLocation"/>
        /// <seealso cref="CreateNotebook(string, string, OpenMode)"/>
        public static Notebook CreateNotebook(string name, OpenMode openMode = OpenMode.None) => CreateNotebook(name, null, openMode);

        #endregion

        #region Special Folder Locations

        /// <summary>
        /// Retrieves the path on disk to the default notebook folder location, this is where new notebooks are created and saved to.
        /// </summary>
        /// <returns>The path to the default notebook folder location.</returns>
        public static string GetDefaultNotebookLocation()
        {
            return Run(app =>
            {
                app.GetSpecialLocation(SpecialLocation.slDefaultNotebookFolder, out string path);
                return path;
            });
        }
        /// <summary>
        /// Retrieves the path on disk to the backup folder location.
        /// </summary>
        /// <returns>The path on disk to the backup folder location.</returns>
        public static string GetBackUpLocation()
        {
            return Run(app =>
            {
                app.GetSpecialLocation(SpecialLocation.slBackUpFolder, out string path);
                return path;
            });
        }
        /// <summary>
        /// Retrieves the folder path on disk to the default notes section, this is where by default quick notes are created and saved to.
        /// </summary>
        /// <returns>The folder path on disk to the default notes section.</returns>
        public static string GetDefaultNotesLocation()
        {
            return Run(app =>
            {
                app.GetSpecialLocation(SpecialLocation.slUnfiledNotesSection, out string path);
                return path;
            });
        }

        #endregion

        #endregion


        /// <summary>
        /// Returns a value that indicates whether the supplied <paramref name="name"/> is a valid for the <typeparamref name="THierarchyItem"/>.
        /// </summary>
        /// <param name="name">The potential new name/title of the <typeparamref name="THierarchyItem"/></param>
        /// <typeparam name="THierarchyItem">The type of hierarchy element to check the name of i.e. a <see cref="Notebook"/>, a <see cref="SectionGroup"/> or a <see cref="Section"/></typeparam>
        /// <returns><see langword="true"/> if the specified <paramref name="name"/> is not null, empty, whitespace or contains any characters from the <typeparamref name="THierarchyItem"/> implementation of <see cref="INameInvalidCharacters.InvalidCharacters"/>; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="Notebook.InvalidCharacters"/>
        /// <seealso cref="SectionGroup.InvalidCharacters"/>
        /// <seealso cref="Section.InvalidCharacters"/>
        public static bool IsValidName<THierarchyItem>(string name) where THierarchyItem : INameInvalidCharacters //Maybe switch to Regex
            => !string.IsNullOrWhiteSpace(name) && !THierarchyItem.InvalidCharacters.Any(name.Contains);

        // Used to make releasing and getting the COM object easier depending on the ComObjectSetting.
        // rename to UseCOM?
        internal static T Run<T>(Func<Application, T> func)
        {
            try
            {
                if (ComObjectMode == ComObjectMode.Wrap || ComObjectMode == ComObjectMode.Lazy)
                {
                    InitComObject();
                }
                else if (!HasComObject) // Only happens when on ComObjectMode.Manual
                {
                    throw Exceptions.NoComObject();
                }
                return func(application);
            }
            catch (COMException ex)
            {
                throw Exceptions.NicifiedComException(ex);
            }
            finally
            {
                if (ComObjectMode == ComObjectMode.Wrap)
                {
                    ReleaseComObject();
                }
            }
        }

        internal static void Run(Action<Application> action)
        {
            try
            {
                if (ComObjectMode == ComObjectMode.Wrap || ComObjectMode == ComObjectMode.Lazy)
                {
                    InitComObject();
                }
                else if (!HasComObject)
                {
                    throw Exceptions.NoComObject();
                }
                action(application);
            }
            catch (COMException ex)
            {
                throw Exceptions.NicifiedComException(ex);
            }
            finally
            {
                if (ComObjectMode == ComObjectMode.Wrap)
                {
                    ReleaseComObject();
                }
            }
        }


        //TODO: document side effects, like updating the values

        public static class Partial
        {
            public static Root GetHierarchy(HierarchyScope depth)
            {
                var xml = Run(app =>
                {
                    app.GetHierarchy(null, depth.ToInterop(), out string xml, xmlSchema);
                    return xml;
                });
                return xmlParser.ParseRoot(xml);
            }

            public static IReadOnlyList<IOneNoteItem> GetChildren(IOneNoteItem item)
            {
                Throw.IfNull(item);

                if (item is Page)
                {
                    return [];
                }

                var xml = Run(app =>
                {
                    app.GetHierarchy(item.Id, HierarchyScope.Children.ToInterop(), out string xml, xmlSchema);
                    return xml;
                });
                return xmlParser.Parse(xml, null).Children;
            }

            public static IOneNoteItem UpdateDescendants(IOneNoteItem item, HierarchyScope depth, bool force = false)
            {
                Throw.IfNull(item);
                if (depth == HierarchyScope.Notebooks || depth == HierarchyScope.Self || item is Page)
                {
                    return item;
                }

                if (force || item.Children.Count == 0)
                {
                    if (item is INotebookOrSectionGroup nbors)
                    {
                        ((ReadOnlyList<Section>)nbors.Sections).Clear();
                        ((ReadOnlyList<SectionGroup>)nbors.SectionGroups).Clear();
                    }
                    else if (item is Section section)
                    {
                        ((ReadOnlyList<Page>)section.Pages).Clear();
                    }

                    var xml = Run(app =>
                    {
                        app.GetHierarchy(item.Id, depth.ToInterop(), out string xml, xmlSchema);
                        return xml;
                    });
                    xmlParser.ParseExisting(xml, item);
                }
                return item;
            }

            public static IReadOnlyList<IOneNoteItem> GetAndUpdateChildren(IOneNoteItem item, bool force = false)
            {
                Throw.IfNull(item);
                if (item is Page)
                {
                    return [];
                }

                if (force || item.Children.Count == 0)
                {
                    if (item is INotebookOrSectionGroup nbors)
                    {
                        ((ReadOnlyList<Section>)nbors.Sections).Clear();
                        ((ReadOnlyList<SectionGroup>)nbors.SectionGroups).Clear();
                    }
                    else if (item is Section section)
                    {
                        ((ReadOnlyList<Page>)section.Pages).Clear();
                    }

                    var xml = Run(app =>
                    {
                        app.GetHierarchy(item.Id, HierarchyScope.Children.ToInterop(), out string xml, xmlSchema);
                        return xml;
                    });
                    xmlParser.ParseExisting(xml, item);
                }

                return item.Children;
            }

            public static IOneNoteItem GetParent(IOneNoteItem item)
            {
                Throw.IfNull(item);

                if (item is Notebook)
                {
                    return null;
                }

                var xml = Run(app =>
                {
                    app.GetHierarchyParent(item.Id, out string parentId);
                    app.GetHierarchy(parentId, HierarchyScope.Self.ToInterop(), out string xml, xmlSchema);
                    return xml;
                });
                return xmlParser.Parse(xml, null);
            }

            public static IOneNoteItem GetAndUpdateParent(IOneNoteItem item, bool force)
            {
                Throw.IfNull(item);

                if (item is Notebook)
                {
                    return null;
                }

                if (!force && item.Parent != null)
                {
                    return item.Parent;
                }

                var xml = Run(app =>
                {
                    app.GetHierarchyParent(item.Id, out string parentId);
                    app.GetHierarchy(parentId, HierarchyScope.Self.ToInterop(), out string xml, xmlSchema);
                    return xml;
                });
                var parent = xmlParser.Parse(xml, null);
                switch (item)
                {
                    case Section section:
                        section.Parent = (INotebookOrSectionGroup)parent;
                        break;
                    case SectionGroup sectionGroup:
                        sectionGroup.Parent = (INotebookOrSectionGroup)parent;
                        break;
                    case Page page:
                        page.Parent = (Section)parent;
                        break;
                }
                return parent;
            }
        }
    }
}
