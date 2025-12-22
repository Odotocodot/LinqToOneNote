using System.Collections;
using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Internal;

namespace Odotocodot.OneNote.Linq
{
	/// <summary>
	/// The root of object containing the OneNote hierarchy.<br/>
	/// Enumerating this will yield all the notebooks in the property <see cref="Notebooks"/> followed by all the sections in <see cref="OpenSections"/> (if not null).
	/// </summary>
	public class Root : IEnumerable<IOneNoteItem>
	{
		internal ReadOnlyList<Notebook> notebooks = [];

		internal Root() { }

		/// <summary>
		/// The notebooks in the OneNote hierarchy.
		/// </summary>
		public IReadOnlyList<Notebook> Notebooks => notebooks;

#nullable enable
		/// <summary>
		/// The open sections in OneNote that are not contained in any notebook. <br/>
		/// Can be <see langword="null"/> if there are no open sections.
		/// </summary>
		public OpenSections? OpenSections { get; internal set; } //TODO make empty rather than null
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