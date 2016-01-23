using System.Collections.Generic;

namespace MetroidClone.Engine
{
    //The definition of a special tile.
    //For example, a special tile might be "50% chance to get a normal tile, 30% chance to get a ladder, 20% chance to get nothing.".

    class SpecialTileDefinition : ISpecialTileDefinition
    {
        WeightedRandomCollection<char> PossibleTiles;

        public bool CanBeWall { get; set; }
        public Dictionary<string, char> SpecialKeywords { get; set; }

        public SpecialTileDefinition()
        {
            PossibleTiles = new WeightedRandomCollection<char>();
            SpecialKeywords = new Dictionary<string, char>();
            CanBeWall = true;
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
            return PossibleTiles.Get();
        }

        //Add a tile with the specified chance to the definition.
        public void Add(char tile, double chance)
        {
            PossibleTiles.Add(tile, chance);
        }
    }
}
