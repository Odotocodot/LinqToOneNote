namespace LinqToOneNote
{
    /// <summary>
    /// Specifies how/whether to open a newly created OneNote item.
    /// </summary>
    public enum OpenMode
    {
        /// <summary>
        /// Do not open the newly created item.
        /// </summary>
        None,
        /// <summary>
        /// Open the newly created item in the existing OneNote window, or create a new window if none exist.
        /// </summary>
        ExistingOrNewWindow,
        /// <summary>
        /// Open the newly created item in a new OneNote window.
        /// </summary>
        NewWindow
    }
}
