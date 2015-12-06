using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    class LevelGenerator
    {
        Level Level;
        int Width, Height;

        public LevelGenerator()
        {
            Width = 20;
            Height = 15;

            GetLevelBlocks();
        }

        public Level Generate()
        {
            Level = new Level();

            bool[,] grid = new bool[Width, Height];

            for (int i = 0; i < Width; i++)
                grid[i, Height - 1] = true;

            Level.Grid = grid;

            return Level;
        }

        //Reads the possible level blocks from the level file
        void GetLevelBlocks()
        {
            string levelBlocksData = File.ReadAllText("Content/LevelBlocks.txt");
            string[] levelBlocksLines = levelBlocksData.Split('\n');

            LevelBlock

            for (int i = 0; i < levelBlocksLines.Length; i++)
            {
                string line = levelBlocksLines[i];
                if (line.StartsWith("BLOCK"))
                {

                }
            }
        }
    }
}
