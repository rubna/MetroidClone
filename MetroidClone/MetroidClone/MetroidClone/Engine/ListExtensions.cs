using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    //Some useful methods Lists can use
    public static class ListExtensions
    {
        static Random random = new Random();

        public static T GetRandomItem<T>(this List<T> list)
        {
            if (list.Count == 0)
                throw new ArgumentException("It isn't possible to choose an item from an empty list!");
            return list[random.Next(list.Count)];
        }

        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count; i > 1; i--)
            {
                int k = random.Next(i + 1);
                T value = list[k];
                list[k] = list[i];
                list[i] = value;
            }
        }

        public static List<T> Clone<T>(this List<T> list) where T : ICloneable
        {
            return list.Select(item => (T)item.Clone()).ToList();
        }

        public static List<T> ShallowClone<T>(this List<T> list)
        {
            return list.Select(item => item).ToList();
        }
    }
}
