using System.Collections.Generic;

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
        public bool CanBeWall { get; set; }
        public Dictionary<string, char> SpecialKeywords { get; set; }

        public SpecialTileGroupDefinition()
        {
            PossibleSubgroups = new WeightedRandomCollection<List<char>>();
            SpecialKeywords = new Dictionary<string, char>();
            CanBeWall = false;
        }

        //Get a tile defined by the definition.
        public char GetTile(int blockID = 0, List<string> specialKeywords = null)
        {
            if (specialKeywords != null)
            {
                foreach (string keyword in specialKeywords)
                {
                    if (SpecialKeywords.ContainsKey(keyword))
                        return SpecialKeywords[keyword];
                }
            }

            if (blockID != previousBlockID) //Reset the data of the special group if needed.
            {
                CurrentSubGroup = (FairRandomCollection<char>) PossibleSubgroups.Get();
                previousBlockID = blockID;
            }

            char randomVal = CurrentSubGroup.Get();
            return randomVal;
        }

        //Add a subgroup with the specified chance to the definition.
        public void Add(List<char> subgroup, double chance)
        {
            PossibleSubgroups.Add(subgroup, chance);
        }
    }
}
