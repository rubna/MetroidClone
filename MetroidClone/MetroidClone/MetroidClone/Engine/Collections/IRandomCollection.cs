using System.Collections.Generic;

namespace MetroidClone.Engine
{
    //A random collection is a collection with a Get() method that returns a random item
    interface IRandomCollection<T> : ICollection<T>
    {
        T Get();
    }
}
