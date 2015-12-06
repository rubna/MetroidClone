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

        public Level Level { get; protected set; }
        public readonly int Width, Height;

        //The width and height of the level can be given when creating a level.
        public LevelGenerator()
        {
            Width = 20;
            Height = 15;

            levelBlocks = new List<LevelBlock>();

            GetLevelBlocks();
        }

        public Level Generate()
        {
            Level = new Level();

            Level.Grid = new bool[Width, Height]; //Clear the level


            return Level;
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

            for (int i = 0; i < levelBlocksLines.Length; i++)
            {
                string line = levelBlocksLines[i];
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
                        currentLevelBlock.HasUpExit = true;
                    if (exits.Contains("D"))
                        currentLevelBlock.HasDownExit = true;
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
                        currentLevelBlock.UpdateLevelInfo();

                        currentLevelBlock = null;
                    }
                }
            }
        }
    }
}
