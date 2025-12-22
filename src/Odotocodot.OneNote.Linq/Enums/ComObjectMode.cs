namespace Odotocodot.OneNote.Linq
{
    /// <summary>
    /// Defines the mode of COM object handling when using the methods in <see cref="OneNote"/>.
    /// </summary>
    /// <seealso cref="OneNote.ComObjectMode"/>
    /// <seealso cref="OneNote.SetComObjectMode"/>
    /// <seealso cref="OneNote.InitComObject"/>
    /// <seealso cref="OneNote.ReleaseComObject"/>
    public enum ComObjectMode
    {
        /// <summary>
        /// A COM object is only acquired when first needed, then persists till <see cref="OneNote.ReleaseComObject"/> is called or the program exits. (Default)
        /// </summary>
        Lazy,
        /// <summary>
        /// A COM object is only acquired when <see cref="OneNote.InitComObject"/> is called and released when <see cref="OneNote.ReleaseComObject"/> is called.
        /// </summary>
        Manual,
        /// <summary>
        /// A COM object is acquired when a method that requires it is called and released immediately after the method completes.<br/>
        /// Equivalent to calling <see cref="OneNote.InitComObject"/> before a method and <see cref="OneNote.ReleaseComObject"/> after the method, when <see cref="OneNote.ComObjectMode"/> is set to <see cref="Manual"/>.
        /// </summary>
        Wrap,
    }
}
