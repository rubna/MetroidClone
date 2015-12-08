using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    class LevelBlock
    {
        public char[,] Data;
        public readonly int Width, Height;

        public bool HasLeftExit, HasRightExit, HasTopExit, HasBottomExit;
        public bool HasLeftWall, HasRightWall, HasTopWall, HasBottomWall;

        public LevelBlock(int width, int height)
        {
            Width = width;
            Height = height;

            Data = new char[Width, Height];

            HasLeftExit = false;
            HasRightExit = false;
            HasTopExit = false;
            HasBottomExit = false;

            HasLeftWall = false;
            HasRightWall = false;
            HasTopWall = false;
            HasBottomWall = false;
        }

        //Updates information about level wall etc.
        //knownWallCharacters should contain a list of characters that count as a wall.
        public void UpdateLevelInfo(List<char> knownWallCharacters)
        {
            //At the beginning, assume that there's a wall on each side.
            HasLeftWall = true;
            HasRightWall = true;
            HasTopWall = true;
            HasBottomWall = true;

            //Then, check if that's true.
            for (int i = 0; i < Math.Max(Width, Height); i++)
            {
                if (i < Height)
                {
                    if (!knownWallCharacters.Contains(Data[0, i]))
                        HasLeftWall = false;
                    if (!knownWallCharacters.Contains(Data[i, Height - 1]))
                        HasBottomWall = false;
                }
                if (i < Width)
                {
                    if (!knownWallCharacters.Contains(Data[Width - 1, i]))
                        HasRightWall = false;
                    if (!knownWallCharacters.Contains(Data[i, 0]))
                        HasTopWall = false;
                }
            }
        }

        //Place a level block in a level. The preferred walls can be specified. These can only be guaranteed
        // if the level has the wall type (for example, preferLeftWall can be ignored if HasLeftWall is false).
        //If a wall isn't preferred at a position, an exit is used instead if Has*Exit is true.
        public void Place(World world, Dictionary<char, ISpecialTileDefinition> specialTiles, int x, int y, bool preferLeftWall = true,
            bool preferRightWall = true, bool preferTopWall = true, bool preferBottomWall = true)
        {
            int BlockID = (new Random()).Next(int.MaxValue);

            Level level = world.Level;
            for (int i = x; i < x + Width; i++)
            {
                for (int j = y; j < y + Height; j++)
                {
                    char data = Data[i - x, j - y];

                    //Check special tiles until we find a normal tile.
                    int loopCount = 0;

                    //Some tiles can be turned into walls if they end up on the sides of the level.
                    bool specialTileTurnsIntoWallOnSides = false;

                    while (specialTiles.ContainsKey(data))
                    {
                        specialTileTurnsIntoWallOnSides = specialTileTurnsIntoWallOnSides || specialTiles[data].CanBeWall;
                        data = specialTiles[data].GetTile(BlockID);

                        if (loopCount > 1000)
                            throw new Exception("Possible infinite loop detected! Please change the level definition file.");
                    }

                    //If the special tile can be turned into a wall, check if this should happen.
                    if (specialTileTurnsIntoWallOnSides)
                    {
                        if (i == x) //Left wall
                        {
                            if (preferLeftWall)
                                data = '1';
                        }
                        if (i == x + Width - 1) //Right wall
                        {
                            if (preferRightWall)
                                data = '1';
                        }
                        if (j == y) //Top wall
                        {
                            if (preferTopWall)
                                data = '1';
                        }
                        if (j == y + Height - 1) //Bottom wall
                        {
                            if (preferBottomWall)
                                data = '1';
                        }
                    }

                    if (data == '1') //A wall
                        level.Grid[i, j] = true;
                    else if (data == '.')
                    {
                        //Nothing
                    }
                }
            }
        }

        //Check if the level block meets the given requirements.
        public bool MeetsRequirements(LevelBlockRequirements requirements)
        {
            return (requirements.LeftSideType != SideType.Wall | HasLeftWall &&
                    requirements.LeftSideType != SideType.Exit | HasLeftExit &&
                    requirements.RightSideType != SideType.Wall | HasRightWall &&
                    requirements.RightSideType != SideType.Exit | HasRightExit &&
                    requirements.TopSideType != SideType.Wall | HasTopWall &&
                    requirements.TopSideType != SideType.Exit | HasTopExit &&
                    requirements.BottomSideType != SideType.Wall | HasBottomWall &&
                    requirements.BottomSideType != SideType.Exit | HasBottomExit);
        }
    }
}
