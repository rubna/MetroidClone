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
            int worldW = 7, worldH = 7; //The width and height of the world.

            //Define and initialize variables
            bool[,] isRoom = new bool[worldW, worldH];
            List<RoomExit>[,] roomExits = new List<RoomExit>[worldW, worldH];
            List<string>[,] guaranteedSpecialBlocks = new List<string>[worldW, worldH];
            for (int i = 0; i < worldW; i++)
                for (int j = 0; j < worldH; j++)
                {
                    roomExits[i, j] = new List<RoomExit>();
                    guaranteedSpecialBlocks[i, j] = new List<string>();
                    isRoom[i, j] = false;
                }

            isRoom[0, worldH / 2] = true; //Starting room
            guaranteedSpecialBlocks[0, worldH / 2].Add("PlayerStart");
            guaranteedSpecialBlocks[0, worldH / 2].Add("GunPickup");

            isRoom[1, worldH / 2] = true; //Room right of starting room.

            //Place the exits
            for (int i = 0; i < worldW; i++)
                for (int j = 0; j < worldH; j++)
                {
                    //Place exits.
                    if (i < worldW - 1 && isRoom[i, j] && isRoom[i + 1, j])
                    {
                        int nextY = World.Random.Next(LevelHeight / LevelGenerator.BlockHeight);
                        roomExits[i, j].Add(new RoomExit(new Point(LevelWidth / LevelGenerator.BlockWidth - 1, nextY), Direction.Right));
                        roomExits[i + 1, j].Add(new RoomExit(new Point(0, nextY), Direction.Left));
                    }
                    if (j < worldH - 1 && isRoom[i, j] && isRoom[i, j + 1])
                    {
                        int nextX = World.Random.Next(LevelWidth / LevelGenerator.BlockWidth);
                        roomExits[i, j].Add(new RoomExit(new Point(nextX, LevelHeight / LevelGenerator.BlockHeight - 1), Direction.Down));
                        roomExits[i, j + 1].Add(new RoomExit(new Point(nextX, 0), Direction.Up));
                    }
                }

            //And generate the levels.
            for (int i = 0; i < worldW; i++)
                for (int j = 0; j < worldH; j++)
                {
                    if (isRoom[i, j])
                        levelGenerator.Generate(world, new Vector2(LevelWidth * world.TileWidth * i, LevelHeight * world.TileHeight * j), roomExits[i, j],
                            guaranteedSpecialBlocks[i, j]);
                }
        }
    }
}
