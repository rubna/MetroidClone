using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    //The definition of a special tile.
    //For example, a special tile might be "50% chance to get a normal tile, 30% chance to get a ladder, 20% chance to get nothing.".
    class SpecialTileDefinition
    {
        public WeightedChanceCollection<char> PossibleTiles;

        //Get a tile defined by the definition.
        public char GetTile(int blockSeed = 0)
        {

            return '.'; //Return an empty tile by default.
        }
    }
}
