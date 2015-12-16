using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidClone.Engine
{
    // The world generator generates the game world. It places the rooms and decides where there should be exits, but calls the LevelGenerator to create the rooms.
    class WorldGenerator
    {
        LevelGenerator levelGenerator;
        public const int LevelWidth = 20, LevelHeight = 15;

        public WorldGenerator()
        {
            levelGenerator = new LevelGenerator(LevelWidth, LevelHeight);
        }

        public void Generate(World world)
        {
            //Define and initialize variables
            List<RoomExit>[,] roomExits = new List<RoomExit>[5, 5];
            List<string>[,] guaranteedSpecialBlocks = new List<string>[5, 5];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    roomExits[i, j] = new List<RoomExit>();
                    guaranteedSpecialBlocks[i, j] = new List<string>();
                }

            //Place the exits
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    //Place exits.
                    if (i < 4)
                    {
                        int nextY = World.Random.Next(LevelHeight / LevelGenerator.BlockHeight);
                        roomExits[i, j].Add(new RoomExit(new Point(LevelWidth / LevelGenerator.BlockWidth - 1, nextY), Direction.Right));
                        roomExits[i + 1, j].Add(new RoomExit(new Point(0, nextY), Direction.Left));
                    }
                    if (j < 4)
                    {
                        int nextX = World.Random.Next(LevelWidth / LevelGenerator.BlockWidth);
                        roomExits[i, j].Add(new RoomExit(new Point(nextX, LevelHeight / LevelGenerator.BlockHeight - 1), Direction.Down));
                        roomExits[i, j + 1].Add(new RoomExit(new Point(nextX, 0), Direction.Up));
                    }
                }

            //Special blocks (like the starting position of the player)
            guaranteedSpecialBlocks[0, 0].Add("PlayerStart");

            //And generate the levels.
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    levelGenerator.Generate(world, new Vector2(LevelWidth * world.TileWidth * i, LevelHeight * world.TileHeight * j), roomExits[i, j],
                        guaranteedSpecialBlocks[i, j]);
                }
        }
    }
}
