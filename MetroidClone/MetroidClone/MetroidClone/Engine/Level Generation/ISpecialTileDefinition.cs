using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    //The interface for special tiles and special tile groups.
    interface ISpecialTileDefinition
    {
        char GetTile(int blockID = 0);
    }
}
