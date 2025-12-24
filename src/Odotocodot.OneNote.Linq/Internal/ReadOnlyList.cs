using System.Collections;
using System.Collections.Generic;
using Odotocodot.OneNote.Linq.Abstractions;

namespace Odotocodot.OneNote.Linq.Internal
{
    internal abstract class ReadOnlyList //: IReadOnlyList<IOneNoteItem>
    {
        internal static readonly ReadOnlyList<IOneNoteItem> Empty = [];
        public abstract int Count { get; }
        internal abstract void Clear();
        internal abstract bool Remove(IDeletable item);
    }

    internal class ReadOnlyList<T> : ReadOnlyList, IReadOnlyList<T> where T : IOneNoteItem
    {
        private readonly List<T> list = [];
        public T this[int index] => list[index];
        internal void Add(T item) => list.Add(item);
        internal override bool Remove(IDeletable item) => list.Remove((T)item);
        internal override void Clear() => list.Clear();
        public override int Count => list.Count;
        public List<T>.Enumerator GetEnumerator() => list.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal class ChildrenCollection(ReadOnlyList<Section> sections, ReadOnlyList<SectionGroup> sectionGroups) : ReadOnlyList, IReadOnlyList<IOneNoteItem>
    {
        public IOneNoteItem this[int index] => index < sections.Count ? sections[index] : sectionGroups[index - sections.Count];
        public override int Count => sections.Count + sectionGroups.Count;
        internal override void Clear()
        {
            sections.Clear();
            sectionGroups.Clear();
        }

        internal override bool Remove(IDeletable item)
        {
            return item switch
            {
                Section => sections.Remove(item),
                SectionGroup => sectionGroups.Remove(item),
                _ => false,
            };
        }
        public Enumerator GetEnumerator() => new(sections, sectionGroups);
        IEnumerator<IOneNoteItem> IEnumerable<IOneNoteItem>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator(ReadOnlyList<Section> sections, ReadOnlyList<SectionGroup> sectionGroups) : IEnumerator<IOneNoteItem>
        {
            private List<Section>.Enumerator sectionEnumerator = sections.GetEnumerator();
            private List<SectionGroup>.Enumerator sectionGroupEnumerator = sectionGroups.GetEnumerator();

            public IOneNoteItem Current { get; private set; }
            readonly object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                while (sectionEnumerator.MoveNext())
                {
                    Current = sectionEnumerator.Current;
                    return true;
                }

                while (sectionGroupEnumerator.MoveNext())
                {
                    Current = sectionGroupEnumerator.Current;
                    return true;
                }
                return false;
            }

            public readonly void Reset() { }
            public readonly void Dispose() { }
        }
    }
}
