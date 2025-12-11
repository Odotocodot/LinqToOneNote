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
			private List<Notebook>.Enumerator notebookEnumerator = root.notebooks.GetEnumerator();
			private readonly List<Section>.Enumerator? openSectionsEnumerator = root.OpenSections?.sections.GetEnumerator();
			public IOneNoteItem Current { get; private set; }
			readonly object IEnumerator.Current => Current;
			public bool MoveNext()
			{
				while (notebookEnumerator.MoveNext())
				{
					Current = notebookEnumerator.Current;
					return true;
				}

				if (openSectionsEnumerator is null)
					return false;

				var enumerator = openSectionsEnumerator.Value;
				while (enumerator.MoveNext())
				{
					Current = enumerator.Current;
					return true;
				}
				return false;
			}
			public readonly void Reset() { }
			public readonly void Dispose() { }
		}
	}
}