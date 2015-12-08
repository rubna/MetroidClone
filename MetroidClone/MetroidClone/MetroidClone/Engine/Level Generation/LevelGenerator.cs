using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    class LevelGenerator
    {
        List<LevelBlock> levelBlocks;
        Dictionary<char, ISpecialTileDefinition> specialTiles;

        public World World { get; protected set; }
        public Level Level { get; protected set; }
        public readonly int Width, Height, BlockWidth, BlockHeight;

        //The width and height of the level can be given when creating a level.
        public LevelGenerator()
        {
            Width = 20;
            Height = 15;
            BlockWidth = 5;
            BlockHeight = 5;

            levelBlocks = new List<LevelBlock>();
            specialTiles = new Dictionary<char, ISpecialTileDefinition>();

            GetLevelBlocks();
        }

        public void Generate(World world)
        {
            Level = new Level();
            World = world;
            World.Level = Level;
            World.AddObject(Level);

            Level.Grid = new bool[Width, Height]; //Clear the level

            //Create the basic grid for the level. This will contain the main path through the level.
            int hBlocks = Width / BlockWidth, vBlocks = Height / BlockHeight;
            LevelBlockRequirements[,] basicGrid = new LevelBlockRequirements[hBlocks, vBlocks];
            for (int i = 0; i < hBlocks; i++)
            {
                for (int j = 0; j < vBlocks; j++)
                {
                    basicGrid[i, j] = new LevelBlockRequirements();
                    if (i == 0)
                        basicGrid[i, j].LeftSideType = SideType.Wall;
                    if (i == hBlocks - 1)
                        basicGrid[i, j].RightSideType = SideType.Wall;
                    if (j == 0)
                        basicGrid[i, j].TopSideType = SideType.Wall;
                    if (j == vBlocks - 1)
                        basicGrid[i, j].BottomSideType = SideType.Wall;
                }
            }

            List<Point> roomExits = new List<Point>();

            //The left exit
            roomExits.Add(new Point(0, 0));
            basicGrid[0, 2].LeftSideType = SideType.Exit;

            //The right exit
            roomExits.Add(new Point(hBlocks - 1, 1));
            basicGrid[hBlocks - 1, 1].RightSideType = SideType.Exit;

            //The top exit
            roomExits.Add(new Point(2, 0));
            basicGrid[hBlocks - 1, 0].TopSideType = SideType.Exit;

            //The bottom exit
            roomExits.Add(new Point(1, vBlocks - 1));
            basicGrid[1, vBlocks - 1].BottomSideType = SideType.Exit;

            //Create the actual level grid
            LevelBlock[,] levelGrid = new LevelBlock[hBlocks, vBlocks];

            //Choose the level blocks
            for (int i = 0; i < hBlocks; i++)
            {
                for (int j = 0; j < vBlocks; j++)
                {
                    //Get a block that would fit at this position.
                    try
                    {
                        levelGrid[i, j] = GetPossibleLevelBlocks(basicGrid[i, j]).GetRandomItem();
                    }
                    catch (Exception e)
                    {
                        throw new Exception("There's no block that would fit here! Please create more blocks and add them to LevelBlocks.txt.", e);
                    }
                }
            }

            //And place them
            for (int i = 0; i < hBlocks; i++)
            {
                for (int j = 0; j < vBlocks; j++)
                {
                    levelGrid[i, j].Place(World, specialTiles, i * BlockWidth, j * BlockHeight, basicGrid[i, j].LeftSideType == SideType.Wall,
                        basicGrid[i, j].RightSideType == SideType.Wall, basicGrid[i, j].TopSideType == SideType.Wall,
                        basicGrid[i, j].BottomSideType == SideType.Wall);
                }
            }
        }

        //Get all the blocks that meet certain requirements.
        List<LevelBlock> GetPossibleLevelBlocks(LevelBlockRequirements requirements)
        {
            List<LevelBlock> possibleLevelBlocks = new List<LevelBlock>();
            
            //Check all level blocks and see which ones would meet the requirements.
            foreach (LevelBlock levelBlock in levelBlocks)
            {
                if (levelBlock.MeetsRequirements(requirements))
                    possibleLevelBlocks.Add(levelBlock);
            }

            return possibleLevelBlocks;
        }

        //Reads the possible level blocks from the level file
        void GetLevelBlocks()
        {
            string levelBlocksData = File.ReadAllText("Content/LevelBlocks.txt");
            //Handle the different types of enter (\r\n, \r, and \n)
            levelBlocksData = levelBlocksData.Replace("\r\n", "\n");
            levelBlocksData = levelBlocksData.Replace("\r", "\n");
            string[] levelBlocksLines = levelBlocksData.Split('\n');
            int thisBlockDataHeight = 0; //The current height of a level block.

            LevelBlock currentLevelBlock = null;

            int totalIgnoreCount = 0;

            for (int i = 0; i < levelBlocksLines.Length; i++)
            {
                string line = levelBlocksLines[i];

                //IGNORE and ATTENTION can be used if some lines shouldn't be processed.
                if (line.StartsWith("IGNORE"))
                    totalIgnoreCount++;
                if (line.StartsWith("ATTENTION") && totalIgnoreCount > 0)
                    totalIgnoreCount--;
                if (totalIgnoreCount > 0)
                    continue;

                if (line.StartsWith("BLOCK"))
                {
                    currentLevelBlock = new LevelBlock(BlockWidth, BlockHeight);
                    thisBlockDataHeight = 0;
                    levelBlocks.Add(currentLevelBlock);

                    line = line.Replace("BLOCK", ""); //Delete the BLOCK part of the line.

                    //If there's a space at the beginning now, remove that, too.
                    if (line.StartsWith(" "))
                        line = line.Remove(0, 1);

                    //Now check the arguments of the block.
                    string[] arguments = line.Split(' ');

                    //The first argument should contain information about the exits of the block.
                    string exits = arguments[0];

                    if (exits.Contains("U"))
                        currentLevelBlock.HasTopExit = true;
                    if (exits.Contains("D"))
                        currentLevelBlock.HasBottomExit = true;
                    if (exits.Contains("L"))
                        currentLevelBlock.HasLeftExit = true;
                    if (exits.Contains("R"))
                        currentLevelBlock.HasRightExit = true;

                    //TODO: the second argument can now contain information about difficulty etc.
                }
                else if (currentLevelBlock != null)
                {
                    if (! string.IsNullOrWhiteSpace(line)) //Set the level data.
                    {
                        for (int j = 0; j < line.Length; j++)
                        {
                            currentLevelBlock.Data[j, thisBlockDataHeight] = line[j];
                        }
                        thisBlockDataHeight++;
                    }
                    else //An empty line means the end of the block definition of this block.
                    {
                        currentLevelBlock.UpdateLevelInfo(GetWallTiles());

                        currentLevelBlock = null;
                    }
                }
                else if (line.StartsWith("DEFINE ")) //This is a special tile definition
                {
                    ParseSpecialTileDefinition(line.Remove(0, "DEFINE ".Length));
                }
            }
        }
        
        void ParseSpecialTileDefinition(string definition)
        {
            string[] parts = definition.Split(' ');
            int currentPart = 0;
            char character = parts[currentPart][0]; //The character that's used to refer to the definition.

            if (parts.Length > currentPart && parts[++currentPart] == "AS")
            {
                ISpecialTileDefinition specialTile = null;
                bool specialTileIsWall = false;
                string expectingNext = "";
                bool hasPreviousPercentage = false;
                double previousPercentage = 1;
                
                while (parts.Length > ++currentPart)
                {
                    if (expectingNext != "") //The part is part of a two-word term
                    {
                        if (parts[currentPart] != expectingNext)
                            throw new Exception("Error in levelblocks definition! Expecting " + expectingNext);
                        expectingNext = "";
                    }
                    else if (parts[currentPart] == "WALL") //The part defines that this can be a wall
                    {
                        specialTileIsWall = true;
                        expectingNext = "OR";
                    }
                    else if (parts[currentPart] == "GROUP") //The part defines that this is a group
                    {
                        specialTile = new SpecialTileGroupDefinition();
                        expectingNext = "OF";
                    }
                    else if (parts[currentPart].Contains("%")) //The part is a percentage
                    {
                        string percentageNumber = parts[currentPart].Replace("%", "");
                        if (!double.TryParse(percentageNumber, out previousPercentage))
                            throw new Exception("Error in levelblocks definition! Couldn't parse percentage.");
                        previousPercentage /= 100;
                        hasPreviousPercentage = true;
                    }
                    else if (parts[currentPart].Length == 1) //It is a normal character
                    {
                        specialTile = specialTile ?? new SpecialTileDefinition();
                        if (!hasPreviousPercentage)
                            previousPercentage = 1;

                        if (specialTile is SpecialTileGroupDefinition)
                        {
                            List<char> currentSubgroup = new List<char>();
                            currentSubgroup.Add(parts[currentPart][0]);
                            while ((parts.Length > currentPart + 1) && (parts[currentPart + 1].Length == 1))
                            {
                                currentSubgroup.Add(parts[++currentPart][0]);
                            }
                            (specialTile as SpecialTileGroupDefinition).Add(currentSubgroup, previousPercentage);
                        }
                        else if (specialTile is SpecialTileDefinition)
                        {
                            (specialTile as SpecialTileDefinition).Add(parts[currentPart][0], previousPercentage);
                        }
                        hasPreviousPercentage = false;
                    }
                }

                if (specialTileIsWall)
                    specialTile.CanBeWall = true;

                specialTiles.Add(character, specialTile);
            }
        }

        //Get all tiles that can be walls. This includes '1', the normal wall character, but also any
        //special tiles that define that they can be a wall.
        List<char> GetWallTiles()
        {
            List<char> knownWallCharacters = new List<char>();

            knownWallCharacters.Add('1'); //'1' is the normal character for a wall, so this is always a wall.

            foreach (KeyValuePair<char, ISpecialTileDefinition> specialTile in specialTiles)
            {
                if (specialTile.Value.CanBeWall)
                    knownWallCharacters.Add(specialTile.Key);
            }

            return knownWallCharacters;
        }
    }
}
