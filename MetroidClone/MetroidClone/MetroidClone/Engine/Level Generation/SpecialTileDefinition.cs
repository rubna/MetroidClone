using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    //The definition of a special tile.
    //For example, a special tile might be "50% chance to get a normal tile, 30% chance to get a ladder, 20% chance to get nothing.".

    class SpecialTileDefinition : ISpecialTileDefinition
    {
        WeightedRandomCollection<char> PossibleTiles;

        public SpecialTileDefinition()
        {
            PossibleTiles = new WeightedRandomCollection<char>();
        }

        //Get a tile defined by the definition.
        public char GetTile(int blockID = 0)
        {
            return PossibleTiles.Get();
        }

        //Add a tile with the specified chance to the definition.
        public void Add(char tile, double chance)
        {
            PossibleTiles.Add(tile, chance);
        }
    }
}
