using System.Collections;
using System.Collections.Generic;

namespace Odotocodot.OneNote.Linq.Internal
{
    internal abstract class ReadOnlyListBase<T, TEnumerator> : IReadOnlyList<T> where T : IOneNoteItem where TEnumerator : struct, IEnumerator<T>
    {
        public abstract T this[int index] { get; }

        public abstract int Count { get; }

        public abstract TEnumerator GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal class ReadOnlyList<T> : ReadOnlyListBase<T, List<T>.Enumerator> where T : IOneNoteItem
    {
        internal readonly List<T> list;
        public ReadOnlyList() => list = [];

        public override T this[int index] => list[index];
        public void Add(T item) => list.Add(item);
        public bool Remove(T item) => list.Remove(item);
        public override int Count => list.Count;
        public override List<T>.Enumerator GetEnumerator() => list.GetEnumerator();
    }

    internal class ChildrenCollection(List<Section> sections, List<SectionGroup> sectionGroups) : ReadOnlyListBase<IOneNoteItem, ChildrenCollection.Enumerator>
    {
        public override IOneNoteItem this[int index] => index < sections.Count ? sections[index] : sectionGroups[index - sections.Count];
        public override int Count => sections.Count + sectionGroups.Count;

        public override Enumerator GetEnumerator() => new(sections, sectionGroups);

        public struct Enumerator(List<Section> sections, List<SectionGroup> sectionGroups) : IEnumerator<IOneNoteItem>
        {
            public IOneNoteItem Current { get; private set; }
            readonly object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                var sectionEnumerator = sections.GetEnumerator();
                while (sectionEnumerator.MoveNext())
                {
                    Current = sectionEnumerator.Current;
                    return true;
                }
                var sectionGroupEnumerator = sectionGroups.GetEnumerator();
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
