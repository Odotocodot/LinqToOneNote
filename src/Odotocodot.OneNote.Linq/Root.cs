using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
	
	public class Root
	{
		internal Dictionary<string, OneNoteItem> cache; //?
		public IEnumerable<Notebook> Notebooks { get; internal set; }

		//public UnfiledNotes UnfiledNotes { get; internal set; }

		//public OpenSections OpenSections { get; internal set; }
	}
}