using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    //A Collection which can return a random item based on a weighted chance.

    class WeightedRandomCollection<T> : IRandomCollection<T>
    {
        static Random random = World.Random;

        public bool IsReadOnly { get { return false; } }
        public int Count { get { return values.Count; } }

        List<T> values;
        List<double> chances;

        public WeightedRandomCollection()
        {
            values = new List<T>();
            chances = new List<double>();
        }

        //Add an item with a chance that tries to fill the total chance. If this isn't possible, add an item with chance '1' instead.
        public void Add(T item)
        {
            values.Add(item);
            if (getTotalChance() < 1)
                chances.Add(1 - getTotalChance());
            else
                chances.Add(1);
        }

        //Add an item with the specified chance
        public void Add(T item, double chance)
        {
            values.Add(item);
            chances.Add(chance);
        }

        //Get a random item (based on the chances of the items)
        public T Get()
        {
            if (getTotalChance() == 0) throw new Exception("You can't get an item from an empty WeightedChanceCollection (or one with only items with chance 0)!");

            double randomValue = random.NextDouble() * getTotalChance(), totalToNow = 0;

            for (int i = 0; i < values.Count; i++)
            {
                double thisChance = chances[i];
                totalToNow += thisChance;

                if (randomValue < totalToNow)
                    return values[i];
            }

            throw new Exception("Something's gone wrong in the implementation of Get() from the WeightedChanceCollection!");
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
                    chances.RemoveAt(i);
                    hasRemovedSomething = true;
                }
            }
            return hasRemovedSomething;
        }

        public void Clear()
        {
            values.Clear();
            chances.Clear();
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

        double getTotalChance()
        {
            return chances.Sum();
        }
    }
}
