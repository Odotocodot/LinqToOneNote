namespace Odotocodot.OneNote.Linq
{
	public enum HierarchyScope
	{
		Self = Microsoft.Office.Interop.OneNote.HierarchyScope.hsSelf,
		Children = Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren,
		Notebooks = Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks,
		Sections = Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections,
		Pages = Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages,
	}
	
	internal static class HierarchyScopeExtensions
	{
		internal static Microsoft.Office.Interop.OneNote.HierarchyScope ToInterop(this HierarchyScope scope) => (Microsoft.Office.Interop.OneNote.HierarchyScope)scope;
	}
}