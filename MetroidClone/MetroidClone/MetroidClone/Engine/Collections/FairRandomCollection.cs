using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    //A collection which can return a random item.
    //It'll try to make sure each item is returned as often as each other item.
    //So, for example, if you put three items into it (pear, apple and banana), it'll always returns each item once before it's going
    //to repeat items.
    //So, it could return {pear, banana, apple} but not {pear, banana, pear}.

    class FairRandomCollection<T> : IRandomCollection<T>
    {
        Random random;

        public bool IsReadOnly { get { return false; } }
        public int Count { get { return values.Count; } }

        List<T> values;
        List<int> timesReturned;

        public FairRandomCollection()
        {
            random = new Random();
            values = new List<T>();
            timesReturned = new List<int>();
        }

        public void Add(T item)
        {
            values.Add(item);
            timesReturned.Add(0);
        }

        public void Add(T item, int times)
        {
            for (int i = 0; i < times; i++)
            {
                values.Add(item);
                timesReturned.Add(0);
            }
        }

        public void AddItems(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                values.Add(item);
                timesReturned.Add(0);
            }
        }

        //Get a random item (keeping things fair as much as possible)
        public T Get()
        {
            if (values.Count == 0) throw new Exception("You can't get an item from an empty FairRandomCollection!");
            List<int> possibleItemPositions = new List<int>();
            int minimumTimesReturned = int.MaxValue;

            for (int i = 0; i < values.Count; i++)
            {
                if (timesReturned[i] <= minimumTimesReturned)
                {
                    if (timesReturned[i] < minimumTimesReturned)
                        possibleItemPositions.Clear();
                    possibleItemPositions.Add(i);
                    minimumTimesReturned = timesReturned[i];
                }
            }

            int index = possibleItemPositions.GetRandomItem();
            timesReturned[index]++;
            return values[index];
        }

        public bool Contains(T item)
        {
            return values.Contains(item);
        }

        public bool Remove(T item)
        {
            bool hasRemovedSomething = false;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].Equals(item))
                {
                    values.RemoveAt(i);
                    timesReturned.RemoveAt(i);
                    hasRemovedSomething = true;
                }
            }
            return hasRemovedSomething;
        }

        public void Clear()
        {
            values.Clear();
            timesReturned.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }

        void ICollection<T>.CopyTo(T[] items, int index)
        {
            values.CopyTo(items, index);
        }

        //Convert from a list to a FairRandomCollection
        static public explicit operator FairRandomCollection<T>(List<T> list)
        {
            FairRandomCollection<T> collection = new FairRandomCollection<T>();
            collection.AddItems(list);
            return collection;
        }
    }
}
