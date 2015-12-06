using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    //A random collection is a collection with a Get() method that returns a random item
    interface IRandomCollection<T> : ICollection<T>
    {
        T Get();
    }
}
