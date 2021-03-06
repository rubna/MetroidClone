﻿using MetroidClone.Metroid;
using MetroidClone.Metroid.Monsters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetroidClone.Engine
{
    class LevelBlock
    {
        public char[,] Data;
        public readonly int Width, Height;

        public bool HasLeftExit, HasRightExit, HasTopExit, HasBottomExit;
        public bool HasLeftWall, HasRightWall, HasTopWall, HasBottomWall;

        public bool ExitsMustMatch; //True if this block may not be placed on positions where the exits don't match exactly.

        public double AppearancePreference; //The preference for this block to appear if it's possible to place it.

        public string Group, Theme; //The Group is used for special blocks that should be guaranteed in a room. The Theme is used to give
        //a certain room a certain appearance.

        static Random random = World.Random;

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

            ExitsMustMatch = false;

            AppearancePreference = 1;

            Group = "";
            Theme = "";
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
            bool preferRightWall = true, bool preferTopWall = true, bool preferBottomWall = true, bool isBottomOfRoom = false, int numberOfEnemies = 0,
            List<Type> possibleEnemyTypes = null)
        {
            possibleEnemyTypes = possibleEnemyTypes ?? new List<Type>();

            int BlockID = random.Next(int.MaxValue);

            bool[,] isActuallyEmpty = new bool[Width, Height];
            bool[,] isActuallyWall = new bool[Width, Height];
            FairRandomCollection<Vector2> possibleEnemyPositions = new FairRandomCollection<Vector2>();
            FairRandomCollection<Vector2> possibleTurretPositions = new FairRandomCollection<Vector2>();

            //Place the tiles in the level
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    char data = Data[i, j];

                    //Check special tiles until we find a normal tile.
                    int loopCount = 0;

                    //Some tiles can be turned into walls if they end up on the sides of the level.
                    bool specialTileTurnsIntoWallOnSides = false;

                    while (specialTiles.ContainsKey(data))
                    {
                        specialTileTurnsIntoWallOnSides = specialTileTurnsIntoWallOnSides || specialTiles[data].CanBeWall;
                        List<string> specialKeywords = new List<string>(); //Contains the special keywords that are true at this moment.

                        if (preferLeftWall) specialKeywords.Add("LEFTWALL");
                        if (preferRightWall) specialKeywords.Add("RIGHTWALL");
                        if (preferTopWall) specialKeywords.Add("TOPWALL");
                        if (preferBottomWall) specialKeywords.Add("BOTTOMWALL");

                        data = specialTiles[data].GetTile(BlockID, specialKeywords);

                        if (loopCount > 1000)
                            throw new Exception("Possible infinite loop detected! Please change the level definition file.");
                    }

                    //If the special tile can be turned into a wall, check if this should happen.
                    if (specialTileTurnsIntoWallOnSides)
                    {
                        if (i == 0) //Left wall
                        {
                            if (preferLeftWall)
                                data = '1';
                        }
                        if (i == Width - 1) //Right wall
                        {
                            if (preferRightWall)
                                data = '1';
                        }
                        if (j == 0) //Top wall
                        {
                            if (preferTopWall)
                                data = '1';
                        }
                        if (j == Height - 1) //Bottom wall
                        {
                            if (preferBottomWall)
                                data = '1';
                        }
                    }

                    int baseX = x + World.TileWidth * i, baseY = y + World.TileHeight * j;
                    float centerX = baseX + World.TileWidth / 2, centerY = baseY + World.TileHeight / 2;
                    Rectangle stdCollisionRect = new Rectangle(baseX, baseY, World.TileWidth, World.TileHeight);

                    //If it's nothing or something the player can move through, and the bottom of the room, then add a jumpthrough.
                    if (data == '.' | data == '/' | data == '\\' && j == Height - 1 && isBottomOfRoom)
                        world.AddObject(new JumpThrough(new Rectangle(baseX, baseY, World.TileWidth, 1)));

                    if (data == '1') //A wall
                    {
                        world.AddObject(new Wall(stdCollisionRect));

                        isActuallyWall[i, j] = true;
                        if (j != 0 && isActuallyEmpty[i, j - 1])
                        {
                            //We can place an enemy here!
                            possibleEnemyPositions.Add(new Vector2(centerX, centerY - World.TileHeight + 10));
                        }
                    }
                    else if (data == 'T') //A tile
                        world.AddObject(new WindowTile(stdCollisionRect));
                    else if (data == 'D') //A gun door
                        world.AddObject(new GunDoor(), baseX + World.TileWidth / 2, baseY);
                    else if (data == 'M') //A melee door
                        world.AddObject(new MeleeDoor(), baseX + World.TileWidth / 2, baseY);
                    else if (data == 'C') //A rocket door
                        world.AddObject(new RocketDoor(), baseX + World.TileWidth / 2, baseY);
                    else if (data == 'B') //A boss door
                        world.AddObject(new BossDoor(), baseX + World.TileWidth / 2, baseY);
                    else if (data == 'O')
                        world.AddObject(new Portal(), baseX + 6, baseY);
                    else if (data == 'S') //Spikes
                        world.AddObject(new Spikes(), baseX, baseY + (int)(World.TileHeight * (100f / 128f)));
                    else if (data == 'G') //A gun pickup block
                        world.AddObject(new GunPickup(), centerX, centerY);
                    else if (data == 'U') //A gun upgrade pickup block
                        world.AddObject(new GunUpgrade(), centerX, centerY);
                    else if (data == 'W') //A wrench pickup block
                        world.AddObject(new WrenchPickup(), centerX, centerY);
                    else if (data == 'R') //A rocket pickup block
                        world.AddObject(new RocketPickup(), centerX, centerY);
                    else if (data == 'H') //A health pickup block
                        world.AddObject(new HealthDrop(10), centerX, centerY);
                    else if (data == 'A') //First health upgrade
                        world.AddObject(new MaxHPUpgrade(), centerX, centerY);
                    else if (data == 'E') //Huge health upgrade
                        world.AddObject(new HugeMaxHPUpgrade(), centerX, centerY);
                    else if (data == 'F') //Drone upgrade
                        world.AddObject(new DroneRangeUpgrade(), centerX, centerY);
                    else if (data == 'P') //The player
                    {
                        world.Player = new Player();
                        world.AddObject(world.Player, centerX, centerY);
                    }
                    else if (data == 'J') //A jumpthrough block
                        world.AddObject(new JumpThrough(new Rectangle(baseX, baseY, World.TileWidth, 1)));
                    else if (data == '\\') //A slope
                        world.AddObject(new SlopeLeft(stdCollisionRect));
                    else if (data == '/') //A slope
                        world.AddObject(new SlopeRight(stdCollisionRect));
                    else if (data == '.')
                    {
                        //Nothing
                        isActuallyEmpty[i, j] = true;

                        if (j != 0 && isActuallyWall[i, j - 1])
                        {
                            //We can place a turret here!
                            possibleTurretPositions.Add(new Vector2(centerX, baseY + 15));
                        }
                    }
                }
            }


            //Place some enemies.
            for (int i = 0; i < numberOfEnemies; i++)
            {
                if (possibleEnemyTypes.Contains(typeof(Turret)) && World.Random.Next(3) == 0)
                {
                    if (possibleTurretPositions.Count != 0)
                        world.AddObject(new Turret(), possibleTurretPositions.Get());
                }
                else if (possibleEnemyPositions.Count != 0)
                {
                    world.AddObject(Activator.CreateInstance(possibleEnemyTypes.Except(new List<Type>() { typeof(Turret) }).ToList().GetRandomItem())
                        as GameObject, possibleEnemyPositions.Get());
                }
            }
        }

        //Check if the level block meets the given requirements.
        public bool MeetsRequirements(LevelBlockRequirements requirements)
        {
            return (
                //If the requirements dictate that a wall or exit should be at a certain position, check if the walls and exits match.
                (requirements.LeftSideType != SideType.Wall | HasLeftWall &&
                requirements.LeftSideType != SideType.Exit | HasLeftExit &&
                requirements.RightSideType != SideType.Wall | HasRightWall &&
                requirements.RightSideType != SideType.Exit | HasRightExit &&
                requirements.TopSideType != SideType.Wall | HasTopWall &&
                requirements.TopSideType != SideType.Exit | HasTopExit &&
                requirements.BottomSideType != SideType.Wall | HasBottomWall &&
                requirements.BottomSideType != SideType.Exit | HasBottomExit) &&
                //Check if the exits match completely if required.
                ((!ExitsMustMatch) ||
                (requirements.LeftSideType == SideType.Exit | !HasLeftExit &&
                requirements.RightSideType == SideType.Exit | !HasRightExit &&
                requirements.TopSideType == SideType.Exit | !HasTopExit &&
                requirements.BottomSideType == SideType.Exit | !HasBottomExit)) &&
                //And check if the group matches
                (Group == requirements.Group) &&
                //And check if the theme matches (if this group has a theme)
                (Theme == "" || requirements.Theme.Contains(Theme)));
        }
    }
}
