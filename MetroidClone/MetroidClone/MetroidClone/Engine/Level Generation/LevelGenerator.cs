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

            //Create the actual level grid
            LevelBlock[,] levelGrid = new LevelBlock[hBlocks, vBlocks];

            //Choose the level blocks
            for (int i = 0; i < hBlocks; i++)
            {
                for (int j = 0; j < vBlocks; j++)
                {
                    //Get a block that would fit at this position.
                    levelGrid[i, j] = GetPossibleLevelBlocks(basicGrid[i, j]).GetRandomItem();
                }
            }

            specialTiles['a'] = new SpecialTileDefinition();
            (specialTiles['a'] as SpecialTileDefinition).Add('1', 0.5);
            (specialTiles['a'] as SpecialTileDefinition).Add('.', 0.5);
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
                    currentLevelBlock = new LevelBlock();
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
                        currentLevelBlock.UpdateLevelInfo(/*TODO: Wall definitions*/);

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
            /*
            Examples:
            a AS 50% 1 50% .
            b AS GROUP OF 50% 1 50% .
            c AS GROUP OF 1 .*/
            string[] parts = definition.Split(' ');
            char character = parts[0][0]; //The character that's used to refer to the definition.

            if (parts[1] == "AS")
            {

            }
        }
    }
}
