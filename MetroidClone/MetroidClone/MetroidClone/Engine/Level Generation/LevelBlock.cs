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

        public LevelBlock()
        {
            Width = 5;
            Height = 5;

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
        public void UpdateLevelInfo()
        {
            char[] wallChars = { '1', '2' }; //Characters that count as a wall.

            //At the beginning, assume that there's a wall on each side.
            HasLeftWall = true;
            HasRightWall = true;
            HasTopWall = true;
            HasBottomWall = true;

            //Then, check if that's true.
            for (int i = 0; i < 5; i++)
            {
                if (!wallChars.Contains(Data[0, i]))
                    HasLeftWall = false;
                if (!wallChars.Contains(Data[4, i]))
                    HasRightWall = false;
                if (!wallChars.Contains(Data[i, 0]))
                    HasTopWall = false;
                if (!wallChars.Contains(Data[i, 4]))
                    HasBottomWall = false;
            }
        }

        //Place a level block in a level. The preferred walls can be specified. These can only be guaranteed
        // if the level has the wall type (for example, preferLeftWall can be ignored if HasLeftWall is false).
        //If a wall isn't preferred at a position, an exit is used instead if Has*Exit is true.
        public void Place(Level level, World world, int x, int y, bool preferLeftWall = true, bool preferRightWall = true,
            bool preferTopWall = true, bool preferBottomWall = true)
        {
            for (int i = x; i < x + Width; i++)
            {
                for (int j = y; j < y + Height; j++)
                {
                    char data = Data[i - x, j - y];
                    if (data == '1') //A wall
                        level.Grid[i, j] = true;
                    else if (data == '2') //Might be a wall
                    {
                        if (i == x) //Left wall
                        {
                            if (preferLeftWall)
                                level.Grid[i, j] = true;
                        }
                        if (i == x + Width - 1) //Right wall
                        {
                            if (preferRightWall)
                                level.Grid[i, j] = true;
                        }
                        if (j == y) //Top wall
                        {
                            if (preferTopWall)
                                level.Grid[i, j] = true;
                        }
                        if (j == y + Height - 1) //Bottom wall
                        {
                            if (preferBottomWall)
                                level.Grid[i, j] = true;
                        }
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
