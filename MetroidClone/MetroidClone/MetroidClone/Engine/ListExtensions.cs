using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    public static class ListExtensions
    {
        static Random random = new Random();

        public static T GetRandomItem<T>(this List<T> list)
        {
            if (list.Count == 0)
                throw new ArgumentException("It isn't possible to choose an item from an empty list!");
            return list[random.Next(list.Count)];
        }
    }
}
