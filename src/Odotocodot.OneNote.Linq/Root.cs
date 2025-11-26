using System.Collections;
using System.Collections.Generic;

namespace Odotocodot.OneNote.Linq
{

	public class Root : IEnumerable<Notebook>
	{
		public IReadOnlyList<Notebook> Notebooks { get; internal set; }

		//public UnfiledNotes UnfiledNotes { get; internal set; }

		//public OpenSections OpenSections { get; internal set; }

		IEnumerator<Notebook> IEnumerable<Notebook>.GetEnumerator() => Notebooks.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Notebooks).GetEnumerator();

	}
}