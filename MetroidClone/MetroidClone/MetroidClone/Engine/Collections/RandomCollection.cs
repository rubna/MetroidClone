using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    //A collection which can return a random item.
    //It also has a Take method which returns and removes the item.

    class RandomCollection<T> : IRandomCollection<T>
    {
        Random random;

        public bool IsReadOnly { get { return false; } }
        public int Count { get { return values.Count; } }

        List<T> values;

        public RandomCollection()
        {
            random = new Random();
            values = new List<T>();
        }

        public void Add(T item)
        {
            values.Add(item);
        }

        public void Add(T item, int times)
        {
            for (int i = 0; i < times; i++)
            {
                values.Add(item);
            }
        }

        public void AddItems(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                values.Add(item);
            }
        }

        //Get a random item
        public T Get()
        {
            if (values.Count == 0) throw new Exception("You can't get an item from an empty RandomCollection!");
            return values.GetRandomItem();
        }

        //Get a random item and remove it
        public T Take()
        {
            if (values.Count == 0) throw new Exception("You can't take an item from an empty RandomCollection!");
            T item = values.GetRandomItem();
            values.Remove(item);
            return item;
        }

        public bool Contains(T item)
        {
            return values.Contains(item);
        }

        public bool Remove(T item)
        {
            return values.Remove(item);
        }

        public void Clear()
        {
            values.Clear();
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

        //Convert from a list to a RandomCollection
        static public explicit operator RandomCollection<T>(List<T> list)
        {
            RandomCollection<T> collection = new RandomCollection<T>();
            collection.AddItems(list);
            return collection;
        }
    }
}
