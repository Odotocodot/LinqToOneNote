namespace Odotocodot.OneNote.Linq.Abstractions
{
    /// <summary>
    /// Represents an object that can be opened in OneNote.
    /// </summary>
    /// <seealso cref="OneNote.Open(INavigable, bool)"/>
    public interface INavigable
    {
        /// <summary>
        /// The id of the OneNote hierarchy item.
        /// </summary>
        string Id { get; }
    }
}