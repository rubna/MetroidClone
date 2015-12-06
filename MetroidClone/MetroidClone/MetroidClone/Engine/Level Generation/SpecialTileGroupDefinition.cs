using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    //The definition of a special tile group.
    //Groups are used to specify that multiple tiles should or shouldn't appear in a level. For example, it could be used to define that two blocks
    //should or should not appear.
    //It also can be used for groups of multiple tiles which should be distributed fairly. For example, if a group contains '1' and '.' and is used twice,
    //both '1' and '.' will be placed exactly once.

    class SpecialTileGroupDefinition : ISpecialTileDefinition
    {
        WeightedRandomCollection<List<char>> PossibleSubgroups;
        FairRandomCollection<char> CurrentSubGroup;
        int previousBlockID = 0;

        public SpecialTileGroupDefinition()
        {
            PossibleSubgroups = new WeightedRandomCollection<List<char>>();
        }

        //Get a tile defined by the definition.
        public char GetTile(int blockID = 0)
        {
            if (blockID != previousBlockID) //Reset the data of the special group if needed.
            {
                CurrentSubGroup = (FairRandomCollection<char>) PossibleSubgroups.Get();
            }

            return CurrentSubGroup.Get();
        }

        //Add a subgroup with the specified chance to the definition.
        public void Add(List<char> subgroup, double chance)
        {
            PossibleSubgroups.Add(subgroup, chance);
        }
    }
}
