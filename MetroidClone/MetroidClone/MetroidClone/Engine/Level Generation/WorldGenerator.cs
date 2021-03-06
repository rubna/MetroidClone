﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid;
using MetroidClone.Metroid.Monsters;

namespace MetroidClone.Engine
{
    // The world generator generates the game world. It places the rooms and decides where there should be exits, but calls the LevelGenerator to create the rooms.
    class WorldGenerator
    {
        LevelGenerator levelGenerator;
        public const int LevelWidth = 20, LevelHeight = 15;
        public const int WorldWidth = 8, WorldHeight = 5;

        public WorldGenerator()
        {
            levelGenerator = new LevelGenerator(LevelWidth, LevelHeight);
        }

        public void Generate(World world)
        {
            string[] areaBorderNameRight = { "SecondRightBorder", "ThirdRightBorder" }; //The names of area borders.
            string[] areaBorderNameLeft = { "SecondLeftBorder", "ThirdLeftBorder" };

            //Enemy types for each area
            List<List<Type>> enemyTypes = new List<List<Type>>();
            enemyTypes.Add(new List<Type>() { typeof(SlimeMonster), typeof(MeleeMonster) });
            enemyTypes.Add(new List<Type>() { typeof(SlimeMonster), typeof(ShootingMonster), typeof(MeleeMonster), typeof(Turret) });
            enemyTypes.Add(new List<Type>() { typeof(ShootingMonster), typeof(MeleeMonster), typeof(Turret) });

            //The first part of the second area has no turrets yet.
            List<Type> enemyTypesStartSecondArea = new List<Type>() { typeof(SlimeMonster), typeof(ShootingMonster), typeof(MeleeMonster) };

            //Define and initialize variables
            bool[,] isRoom = new bool[WorldWidth, WorldHeight]; //Whether this is a room.
            int[,] area = new int[WorldWidth, WorldHeight]; //The area (for example, 0 for the starting area)
            string[,] theme = new string[WorldWidth, WorldHeight]; //The general appearance.
            bool[,] CanHaveRightExit = new bool[WorldWidth, WorldHeight]; //Whether this room can potentially have a right exit.
            bool[,] CanHaveBottomExit = new bool[WorldWidth, WorldHeight]; //Whether this room can potentially have a right exit.
            List<RoomExit>[,] roomExits = new List<RoomExit>[WorldWidth, WorldHeight];
            List<string>[,] guaranteedSpecialBlocks = new List<string>[WorldWidth, WorldHeight]; //Guaranteed blocks.
            int[,] enemies = new int[WorldWidth, WorldHeight];
            for (int i = 0; i < WorldWidth; i++)
                for (int j = 0; j < WorldHeight; j++)
                {
                    roomExits[i, j] = new List<RoomExit>();
                    guaranteedSpecialBlocks[i, j] = new List<string>();
                    isRoom[i, j] = false;
                    area[i, j] = 0;
                    theme[i, j] = "";
                    CanHaveRightExit[i, j] = true;
                    CanHaveBottomExit[i, j] = true;
                    enemies[i, j] = 0;
                }

            //Your own position and the position of the boss.
            int startingY = WorldHeight / 2 + World.Random.Next(-1, 1);
            int bossY = WorldHeight / 2 + World.Random.Next(-1, 1);

            isRoom[0, startingY] = true; //Starting room
            guaranteedSpecialBlocks[0, startingY].Add("PlayerStart");

            isRoom[1, startingY] = true; //Room right of starting room.
            guaranteedSpecialBlocks[1, startingY].Add("GunPickup");
            guaranteedSpecialBlocks[1, startingY].Add(areaBorderNameRight[0]);

            //Other rooms (main areas).
            int areaTwoBorderStart = 3, areaThreeBorderStart = 5;
            for (int i = 2; i < WorldWidth; i++)
                for (int j = 0; j < WorldHeight; j++)
                {
                    bool isTopOrBottomArea = j == 0 || j == WorldHeight - 1;
                    isRoom[i, j] = true;

                    //Set the area
                    if (i >= areaThreeBorderStart + World.Random.Next(2) - (isTopOrBottomArea ? 1 : 0))
                        area[i, j] = 2;
                    else if (i >= areaTwoBorderStart + World.Random.Next(2) - (isTopOrBottomArea ? 1 : 0))
                        area[i, j] = 1;
                    else
                        area[i, j] = 0;

                    //Set the theme
                    if (World.Random.Next(100) < (isTopOrBottomArea ? 60 : 20)) //Generate cramped rooms. There are more of 'em at the top and bottom.
                        theme[i, j] = "Cramped";
                    else if (World.Random.Next(100) < 20) //Generate an open room
                        theme[i, j] = "Open";
                    else if (area[i, j] == 2 && World.Random.Next(100) < 50) //Generate a room with lots of spikes
                        theme[i, j] = "Spiky";
                    else if (World.Random.Next(100) < 10) //Create a room with windows
                        theme[i, j] = "Windowed";

                    //These areas have a normal amount of enemies, with more enemies in later areas.
                    if (area[i, j] == 0)
                        enemies[i, j] = World.Random.Next(2, 3);
                    else
                        enemies[i, j] = World.Random.Next(3, 6) + area[i, j];
                }

            //There should only be one enemy in the third room.
            enemies[2, startingY] = 1;

            //Add a wrench pickup and a rocket pickup. The rocket pickup should have some distance from the starting area.
            int wrenchPos, rocketPos;
            do
            {
                wrenchPos = World.Random.Next(1, WorldHeight - 1);
            }
            while (Math.Abs(wrenchPos - startingY) < 1);

            if (World.Random.Next(2) == 0)
                rocketPos = 0;
            else
                rocketPos = WorldHeight - 1;
             
            guaranteedSpecialBlocks[areaTwoBorderStart + 1, wrenchPos].Add("WrenchPickup");
            guaranteedSpecialBlocks[areaThreeBorderStart + 1, rocketPos].Add("RocketPickup");

            //Other rooms (secondary areas)
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < WorldHeight; j++)
                {
                    if (j != startingY)
                    {
                        isRoom[i, j] = true;
                        area[i, j] = 2;
                        CanHaveBottomExit[i, j] = false;

                        //Set if we need a right exit.
                        if ((j == startingY - 1 || j == startingY + 1) && i == 0)
                            CanHaveRightExit[i, j] = true;
                        else if ((j == 0 || j == WorldHeight - 1) && i == 1)
                            CanHaveRightExit[i, j] = true;
                        else
                            CanHaveRightExit[i, j] = false;

                        //And do the same for the bottom exit.
                        if (j != startingY - 1)
                            CanHaveBottomExit[i, j] = true;
                        else
                            CanHaveBottomExit[i, j] = false;

                        //These areas often have lots of enemies
                        enemies[i, j] = World.Random.Next(6, 10);
                    }
                }

            //Add three max health upgrades somewhere
            guaranteedSpecialBlocks[World.Random.Next(3, WorldWidth - 1), World.Random.Next(0, WorldHeight - 1)].Add("MaxHPDropPickup");
            guaranteedSpecialBlocks[WorldWidth - 1, World.Random.Next(bossY)].Add("MaxHPDropPickup");
            guaranteedSpecialBlocks[WorldWidth - 1, World.Random.Next(bossY + 1, WorldHeight)].Add("MaxHPDropPickup");

            //Add the special bonus gun upgrade
            guaranteedSpecialBlocks[0, 0].Add("GunUpgradePickup");

            //Add the huge health upgrade
            guaranteedSpecialBlocks[0, WorldHeight - 1].Add("HugeMaxHPDropPickup");

            //And the drone upgrade
            guaranteedSpecialBlocks[1, WorldHeight - 1].Add("DroneUpgradePickup");

            //Add the boss room.
            CanHaveBottomExit[WorldWidth - 1, bossY - 1] = false; //The room above can't have a bottom exit.
            CanHaveBottomExit[WorldWidth - 1, bossY] = false; //The boss room can't have a bottom exit.
            enemies[WorldWidth - 1, bossY] = 0; //The boss room has no normal enemy spawns
            theme[WorldWidth - 1, bossY] = "Boss"; //The theme is "boss"
            //Add a right exit to it.
            roomExits[WorldWidth - 1, bossY].Add(new RoomExit(new Point(LevelWidth / LevelGenerator.BlockWidth - 1, LevelHeight / LevelGenerator.BlockHeight - 1), Direction.Right));
            guaranteedSpecialBlocks[WorldWidth - 1, bossY].Add("LeftBossDoorBorder");
            guaranteedSpecialBlocks[WorldWidth - 1, bossY].Add("RightBossDoorBorder");
            guaranteedSpecialBlocks[WorldWidth - 1, bossY].Add("BossPortal");
            //Add nine "normal tiles"
            for (int i = 0; i < 9; i++)
                guaranteedSpecialBlocks[WorldWidth - 1, bossY].Add("NormalBossRoomTile");
            guaranteedSpecialBlocks[WorldWidth - 2, bossY].Add("RightRocketBorder");  //Add a border so players must have the rocket launcher.

            //Place the exits
            for (int i = 0; i < WorldWidth; i++)
                for (int j = 0; j < WorldHeight; j++)
                {
                    //Place exits.
                    if (i < WorldWidth - 1 && isRoom[i, j] && isRoom[i + 1, j] && CanHaveRightExit[i, j])
                    {
                        int nextY;
                        if (j == bossY && i == WorldWidth - 2)
                            nextY = LevelHeight / LevelGenerator.BlockHeight - 1;
                        else
                            nextY = World.Random.Next(LevelHeight / LevelGenerator.BlockHeight);
                        roomExits[i, j].Add(new RoomExit(new Point(LevelWidth / LevelGenerator.BlockWidth - 1, nextY), Direction.Right));
                        roomExits[i + 1, j].Add(new RoomExit(new Point(0, nextY), Direction.Left));

                        if (area[i, j] < area[i + 1, j])
                            guaranteedSpecialBlocks[i, j].Add(areaBorderNameRight[area[i + 1, j] - 1]);
                        else if (area[i, j] > area[i + 1, j])
                            guaranteedSpecialBlocks[i + 1, j].Add(areaBorderNameLeft[area[i, j] - 1]);
                    }
                    if (j < WorldHeight - 1 && isRoom[i, j] && isRoom[i, j + 1] && CanHaveBottomExit[i, j] && (area[i, j] == area[i, j + 1]))
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
                    List<Type> levelEnemyTypes = enemyTypes[area[i, j]];
                    if (i == 3 && area[i, j] == 1)
                        levelEnemyTypes = enemyTypesStartSecondArea;

                    if (isRoom[i, j])
                        levelGenerator.Generate(world, new Vector2(LevelWidth * World.TileWidth * i, LevelHeight * World.TileHeight * j), roomExits[i, j],
                            guaranteedSpecialBlocks[i, j], theme[i, j], enemies[i, j], levelEnemyTypes);
                }

            //Add the "hack this game" object
            world.AddObject(new HackThisGame());

            //Add the map object
            world.AddObject(new Map());

            //Add the GUI object
            world.AddObject(new GUI());

            //Autotile the world.
            Autotile(world);
        }

        //Automatically choose the correct tiles for relevant objects within the game world.
        void Autotile(World world)
        {
            //Create an array for objects that are walls and initialize it.
            bool[,] isWall = new bool[LevelWidth * WorldWidth, LevelHeight * WorldHeight],
                isLeftSlope = new bool[LevelWidth * WorldWidth, LevelHeight * WorldHeight],
                isRightSlope = new bool[LevelWidth * WorldWidth, LevelHeight * WorldHeight];

            //Do the same for an array that defines background tile positions.
            bool[,] doNotCreateBackground = new bool[LevelWidth * WorldWidth, LevelHeight * WorldHeight];
            doNotCreateBackground.Initialize();

            //Set the isWall variable to true at the position of all walls.
            IEnumerable<Wall> walls = world.GameObjects.Where(w => w is Wall).Select(w => w as Wall);
            foreach (Wall wall in walls)
            {
                isWall[wall.BoundingBox.X / World.TileWidth, wall.BoundingBox.Y / World.TileHeight] = true;
            }

            //Left slopes
            IEnumerable<SlopeLeft> lSlopes = world.GameObjects.Where(s => s is SlopeLeft).Select(s => s as SlopeLeft);
            foreach (SlopeLeft slope in lSlopes)
            {
                isLeftSlope[slope.BoundingBox.X / World.TileWidth, slope.BoundingBox.Y / World.TileHeight] = true;
            }

            //Right slopes
            IEnumerable<SlopeRight> rSlopes = world.GameObjects.Where(s => s is SlopeRight).Select(s => s as SlopeRight);
            foreach (SlopeRight slope in rSlopes)
            {
                isRightSlope[slope.BoundingBox.X / World.TileWidth, slope.BoundingBox.Y / World.TileHeight] = true;
            }

            foreach (Wall wall in walls)
            {
                string tileName = "Tileset/foreground";
                wall.BasicConnectionSprite = "Tileset/connection";

                Point positionIndex = new Point(wall.BoundingBox.Left / World.TileWidth,
                    wall.BoundingBox.Top / World.TileHeight);

                bool isTop = positionIndex.Y <= 0,
                    isRight = positionIndex.X >= LevelWidth * WorldWidth - 1,
                    isBottom = positionIndex.Y >= LevelHeight * WorldHeight - 1,
                    isLeft = positionIndex.X <= 0;

                if (isTop || isWall[positionIndex.X, positionIndex.Y - 1])
                {
                    tileName += "U";
                }
                if (isRight || isWall[positionIndex.X + 1, positionIndex.Y])
                {
                    tileName += "R";
                }
                if (isBottom || isWall[positionIndex.X, positionIndex.Y + 1])
                {
                    tileName += "D";
                }
                if (isLeft || isWall[positionIndex.X - 1, positionIndex.Y])
                {
                    tileName += "L";
                }

                if (!isTop && !isLeft && tileName.Contains("U") && tileName.Contains("L") && !isWall[positionIndex.X - 1, positionIndex.Y - 1])
                {
                    wall.ShouldShowTopLeftConnection = true;
                }

                if (!isTop && !isRight && tileName.Contains("U") && tileName.Contains("R") && ! isWall[positionIndex.X + 1, positionIndex.Y - 1])
                {
                    wall.ShouldShowTopRightConnection = true;
                }

                if (!isBottom && !isLeft && tileName.Contains("D") && tileName.Contains("L") && ! isWall[positionIndex.X - 1, positionIndex.Y + 1])
                {
                    wall.ShouldShowBottomLeftConnection = true;
                }

                if (!isBottom && !isRight && tileName.Contains("D") && tileName.Contains("R") && ! isWall[positionIndex.X + 1, positionIndex.Y + 1])
                {
                    wall.ShouldShowBottomRightConnection = true;
                }

                wall.SetSprite(tileName);

                //Create a background image
                if (tileName.Contains("URDL"))
                {
                    doNotCreateBackground[positionIndex.X, positionIndex.Y] = true;
                }
            }

            /*for (int i = 0; i < LevelWidth * WorldWidth; i++)
                for (int j = 0; j < LevelHeight * WorldHeight; j++)
                {
                    if (!doNotCreateBackground[i, j])
                    {
                        BackgroundTile bgt = new BackgroundTile(new Rectangle(i * (int) World.TileWidth, j * (int) World.TileHeight,
                            (int) World.TileWidth, (int) World.TileHeight));
                        world.AddObject(bgt);
                        bgt.SetSprite("BackgroundTileset/background" + World.Random.Next(1, 5));
                    }
                }*/
        }
    }
}
