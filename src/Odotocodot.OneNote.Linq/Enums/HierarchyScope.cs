namespace Odotocodot.OneNote.Linq
{
	/// <summary>
	/// When passed to the <see cref="OneNote.Partial"/> methods, specifies the lowest level to get in the notebook node hierarchy.
	/// </summary>
	public enum HierarchyScope
	{
		/// <summary>
		/// Gets just the start node specified and no descendants.
		/// </summary>
		Self = Microsoft.Office.Interop.OneNote.HierarchyScope.hsSelf,
		/// <summary>
		/// Gets the direct child nodes of the start node, and no descendants in higher or lower subsection groups.
		/// </summary>
		Children = Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren,
		/// <summary>
		/// Gets all notebooks below the start node, or root.
		/// </summary>
		Notebooks = Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks,
		/// <summary>
		/// Gets all sections below the start node, including sections in section groups and subsection groups.
		/// </summary>
		Sections = Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections,
		/// <summary>
		/// Gets all pages below the start node, including all pages in section groups and subsection groups.
		/// </summary>
		Pages = Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages,
	}

	internal static class HierarchyScopeExtensions
	{
		internal static Microsoft.Office.Interop.OneNote.HierarchyScope ToInterop(this HierarchyScope scope) => (Microsoft.Office.Interop.OneNote.HierarchyScope)scope;
	}
}