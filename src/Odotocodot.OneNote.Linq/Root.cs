using System.Collections;
using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{

	public class Root : IEnumerable<IOneNoteItem>
	{
		internal ReadOnlyList<Notebook> notebooks;

		internal Root() { notebooks = []; }

		public IReadOnlyList<Notebook> Notebooks => notebooks;

#nullable enable
		public OpenSections? OpenSections { get; internal set; }
#nullable restore

		IEnumerator<IOneNoteItem> IEnumerable<IOneNoteItem>.GetEnumerator() => new Enumerator(this);
		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

		private struct Enumerator(Root root) : IEnumerator<IOneNoteItem>
		{
			public IOneNoteItem Current { get; private set; }
			readonly object IEnumerator.Current => Current;
			public bool MoveNext()
			{
				var notebookEnumerator = root.notebooks.GetEnumerator();
				while (notebookEnumerator.MoveNext())
				{
					Current = notebookEnumerator.Current;
					return true;
				}
				if (root.OpenSections is not null)
				{
					var openSectionsEnumerator = root.OpenSections.sections.GetEnumerator();
					while (openSectionsEnumerator.MoveNext())
					{
						Current = openSectionsEnumerator.Current;
						return true;
					}
				}
				return false;
			}
			public readonly void Reset() { }
			public readonly void Dispose() { }
		}

	}
}