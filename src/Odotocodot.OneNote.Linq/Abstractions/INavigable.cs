namespace Odotocodot.OneNote.Linq.Abstractions
{
    /// <summary>
    /// Represents an object that can be opened in OneNote.
    /// </summary>
    /// <seealso cref="OneNote.Open"/>
    public interface INavigable
    {
        /// <summary>
        /// The id of the OneNote hierarchy item.
        /// </summary>
        string Id { get; }
    }
}