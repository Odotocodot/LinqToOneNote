using System.Collections;
using System.Collections.Generic;
using LinqToOneNote.Internal;

namespace LinqToOneNote
{
	/// <summary>
	/// The root of object containing the OneNote hierarchy.<br/>
	/// Enumerating this will yield all the notebooks in the property <see cref="Notebooks"/> followed by all the sections in <see cref="Root.OpenSections"/> (if not <see langword="null"/>).
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
		public OpenSections? OpenSections { get; internal set; } //Could make empty rather than null when no open sections.
#nullable restore

		/// <summary>
		/// Returns an enumerator that will iterate the notebooks in the property <see cref="Notebooks"/> followed by the sections in <see cref="Root.OpenSections"/> (if it is not <see langword="null"/>).
		/// </summary>
		/// <returns>An enumerator that will iterate the notebooks in the property <see cref="Notebooks"/> followed by the sections in <see cref="Root.OpenSections"/> (if it is not <see langword="null"/>).</returns>
		public Enumerator GetEnumerator() => new Enumerator(this);
		IEnumerator<IOneNoteItem> IEnumerable<IOneNoteItem>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <exclude/>
		/// <summary>
		/// Enumerates the notebooks and open sections of a <see cref="Root"/>.
		/// </summary>
		public struct Enumerator : IEnumerator<IOneNoteItem>
		{
			private List<Notebook>.Enumerator notebookEnumerator;
			private List<Section>.Enumerator openSectionsEnumerator;
			private readonly bool hasOpenSections;
			internal Enumerator(Root root)
			{
				notebookEnumerator = root.notebooks.GetEnumerator();
				if (root.OpenSections == null)
					return;
				openSectionsEnumerator = root.OpenSections.sections.GetEnumerator();
				hasOpenSections = true;
			}
			///<inheritdoc/>
			public IOneNoteItem Current { get; private set; }
			readonly object IEnumerator.Current => Current;
			///<inheritdoc/>
			public bool MoveNext()
			{
				while (notebookEnumerator.MoveNext())
				{
					Current = notebookEnumerator.Current;
					return true;
				}

				if (!hasOpenSections)
					return false;

                while (openSectionsEnumerator.MoveNext())
				{
					Current = openSectionsEnumerator.Current;
					return true;
				}
				return false;
			}
			///<inheritdoc/>
			public readonly void Reset() { }
			///<inheritdoc/>
			public readonly void Dispose() { }
		}
	}
}