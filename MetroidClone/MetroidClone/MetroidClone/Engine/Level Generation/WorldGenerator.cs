using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid;

namespace MetroidClone.Engine
{
    // The world generator generates the game world. It places the rooms and decides where there should be exits, but calls the LevelGenerator to create the rooms.
    class WorldGenerator
    {
        LevelGenerator levelGenerator;
        public const int LevelWidth = 20, LevelHeight = 15;
        public const int WorldWidth = 7, WorldHeight = 7;

        public WorldGenerator()
        {
            levelGenerator = new LevelGenerator(LevelWidth, LevelHeight);
        }

        public void Generate(World world)
        {
            string[] areaBorderName = { "FirstRightBorder", "SecondRightBorder" }; //The names of area borders.

            //Define and initialize variables
            bool[,] isRoom = new bool[WorldWidth, WorldHeight]; //Whether this is a room.
            int[,] area = new int[WorldWidth, WorldHeight]; //The area (for example, 0 for the starting area)
            string[,] theme = new string[WorldWidth, WorldHeight]; //The general appearance.
            List<RoomExit>[,] roomExits = new List<RoomExit>[WorldWidth, WorldHeight];
            List<string>[,] guaranteedSpecialBlocks = new List<string>[WorldWidth, WorldHeight]; //Guaranteed blocks.
            for (int i = 0; i < WorldWidth; i++)
                for (int j = 0; j < WorldHeight; j++)
                {
                    roomExits[i, j] = new List<RoomExit>();
                    guaranteedSpecialBlocks[i, j] = new List<string>();
                    isRoom[i, j] = false;
                    area[i, j] = 0;
                    theme[i, j] = "";
                }

            int startingY = WorldHeight / 2 + World.Random.Next(-2, 3);

            isRoom[0, startingY] = true; //Starting room
            guaranteedSpecialBlocks[0, startingY].Add("PlayerStart");
            guaranteedSpecialBlocks[0, startingY].Add("GunPickup");

            isRoom[1, startingY] = true; //Room right of starting room.

            //Areas
            for (int i = 0; i < areaBorderName.Length; i++)
            {

            }

            //Other rooms.
            for (int i = 2; i < WorldWidth; i++)
                for (int j = 0; j < WorldHeight; j++)
                {
                    isRoom[i, j] = true;
                    if (World.Random.Next(100) < (j == 0 || j == WorldHeight - 1 ? 60 : 20)) //Generate cramped rooms. There are more of 'em at the top and bottom.
                        theme[i, j] = "Cramped";
                    else if (World.Random.Next(100) < 20) //Generate an open room
                        theme[i, j] = "Open";
                }

            //Place the exits
            for (int i = 0; i < WorldWidth; i++)
                for (int j = 0; j < WorldHeight; j++)
                {
                    //Place exits.
                    if (i < WorldWidth - 1 && isRoom[i, j] && isRoom[i + 1, j])
                    {
                        int nextY = World.Random.Next(LevelHeight / LevelGenerator.BlockHeight);
                        roomExits[i, j].Add(new RoomExit(new Point(LevelWidth / LevelGenerator.BlockWidth - 1, nextY), Direction.Right));
                        roomExits[i + 1, j].Add(new RoomExit(new Point(0, nextY), Direction.Left));

                        if (area[i, j] != area[i + 1, j])
                            guaranteedSpecialBlocks[i, j].Add(areaBorderName[area[i, j]]);
                    }
                    if (j < WorldHeight - 1 && isRoom[i, j] && isRoom[i, j + 1] && area[i, j] == area[i, j + 1])
                    {
                        int nextX = World.Random.Next(LevelWidth / LevelGenerator.BlockWidth);
                        roomExits[i, j].Add(new RoomExit(new Point(nextX, LevelHeight / LevelGenerator.BlockHeight - 1), Direction.Down));
                        roomExits[i, j + 1].Add(new RoomExit(new Point(nextX, 0), Direction.Up));
                    }
                }

            //And generate the levels.
            for (int i = 0; i < WorldWidth; i++)
                for (int j = 0; j < WorldHeight; j++)
                {
                    if (isRoom[i, j])
                        levelGenerator.Generate(world, new Vector2(LevelWidth * world.TileWidth * i, LevelHeight * world.TileHeight * j), roomExits[i, j],
                            guaranteedSpecialBlocks[i, j], theme[i, j]);
                }

            //Add the "hack this game" object
            world.AddObject(new HackThisGame());

            //Add the map object
            world.AddObject(new Map());
        }
    }
}
