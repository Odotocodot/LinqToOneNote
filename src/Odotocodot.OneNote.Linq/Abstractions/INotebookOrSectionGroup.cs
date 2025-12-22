using System.Collections.Generic;

namespace Odotocodot.OneNote.Linq.Abstractions
{
	/// <summary>
	/// Represents a OneNote hierarchy item that can have <see cref="Section">sections</see> and/or <see cref="SectionGroup">section groups</see> as children, i.e. a <see cref="Notebook">notebook</see> or a <see cref="SectionGroup">section group</see>.
	/// </summary>
	/// <seealso cref="Notebook"/>
	/// <seealso cref="SectionGroup"/>
	public interface INotebookOrSectionGroup : IOneNoteItem
	{
		/// <summary>
		/// The sections that this item contains (direct children only).
		/// </summary>
		IReadOnlyList<Section> Sections { get; }

		/// <summary>
		/// The section groups that this item contains (direct children only).
		/// </summary>
		IReadOnlyList<SectionGroup> SectionGroups { get; }
	}
}