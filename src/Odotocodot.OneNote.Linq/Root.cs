using System.Collections.Generic;

namespace Odotocodot.OneNote.Linq
{
	
	public class Root
	{
		public IEnumerable<Notebook> Notebooks { get; internal set; }

		//public UnfiledNotes UnfiledNotes { get; internal set; }

		//public OpenSections OpenSections { get; internal set; }
	}

	public class RootFull 
	{
		public IEnumerable<NotebookFull> Notebooks { get; internal set; }
	}
}