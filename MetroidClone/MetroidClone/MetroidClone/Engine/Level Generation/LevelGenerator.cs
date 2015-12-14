using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    public enum Direction { Left, Right, Up, Down }

    class LevelGenerator
    {
        List<LevelBlock> levelBlocks;
        Dictionary<char, ISpecialTileDefinition> specialTiles;

        public World World { get; protected set; }
        public Level Level { get; protected set; }
        public readonly int Width, Height, BlockWidth, BlockHeight;

        static Random random = World.Random;

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

            //By default, all sides are walls.
            for (int i = 0; i < hBlocks; i++)
            {
                for (int j = 0; j < vBlocks; j++)
                {
                    basicGrid[i, j] = new LevelBlockRequirements(SideType.Wall);
                }
            }

            List<Point> roomExits = new List<Point>();

            //The left exit
            roomExits.Add(new Point(0, random.Next(vBlocks)));
            basicGrid[0, roomExits[0].Y].LeftSideType = SideType.Exit;

            //The right exit
            roomExits.Add(new Point(hBlocks - 1, random.Next(vBlocks)));
            basicGrid[hBlocks - 1, roomExits[1].Y].RightSideType = SideType.Exit;

            //The top exit
            roomExits.Add(new Point(random.Next(hBlocks), 0));
            basicGrid[roomExits[2].X, 0].TopSideType = SideType.Exit;

            //The bottom exit
            roomExits.Add(new Point(random.Next(hBlocks), vBlocks - 1));
            basicGrid[roomExits[3].X, vBlocks - 1].BottomSideType = SideType.Exit;

            //Connect the exits
            ConnectPoints(basicGrid, roomExits);

            //Connect each now unconnected point to a connected point
            ConnectUnconnectedPoints(basicGrid);

            //Prune unneeded walls
            PruneWalls(basicGrid);

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
                        levelGrid[i, j] = GetPossibleLevelBlock(basicGrid[i, j]);
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

        //Connect any number of points on the level grid.
        void ConnectPoints(LevelBlockRequirements[,] basicGrid, List<Point> points)
        {
            //Choose a random point and connect that to each other point.
            //This automatically guarantees that each point is connected to each other point, as
            //you can always travel to the first point and then to the target point.
            Point point = points.GetRandomItem();

            for (int i = 0; i < points.Count; i++)
            {
                Point otherPoint = points[i];
                if (point == otherPoint)
                    continue;

                //Check how far we have to travel
                Point distanceDifference = new Point(otherPoint.X - point.X, otherPoint.Y - point.Y);

                //And store the directions we have to take into a RandomCollection of directions
                RandomCollection<Direction> directionsToTravel = new RandomCollection<Direction>();

                if (distanceDifference.X < 0)
                    directionsToTravel.Add(Direction.Left, -distanceDifference.X);
                if (distanceDifference.X > 0)
                    directionsToTravel.Add(Direction.Right, distanceDifference.X);
                if (distanceDifference.Y < 0)
                    directionsToTravel.Add(Direction.Up, -distanceDifference.Y);
                if (distanceDifference.Y > 0)
                    directionsToTravel.Add(Direction.Down, distanceDifference.Y);

                //Then actually travel from the first point to the second one.
                Point position = point;
                while (directionsToTravel.Count != 0)
                {
                    switch (directionsToTravel.Take())
                    {
                        case Direction.Left:
                            basicGrid[position.X, position.Y].LeftSideType = SideType.Exit;
                            position.X -= 1;
                            basicGrid[position.X, position.Y].RightSideType = SideType.Exit;
                            break;
                        case Direction.Right:
                            basicGrid[position.X, position.Y].RightSideType = SideType.Exit;
                            position.X += 1;
                            basicGrid[position.X, position.Y].LeftSideType = SideType.Exit;
                            break;
                        case Direction.Up:
                            basicGrid[position.X, position.Y].TopSideType = SideType.Exit;
                            position.Y -= 1;
                            basicGrid[position.X, position.Y].BottomSideType = SideType.Exit;
                            break;
                        case Direction.Down:
                            basicGrid[position.X, position.Y].BottomSideType = SideType.Exit;
                            position.Y += 1;
                            basicGrid[position.X, position.Y].TopSideType = SideType.Exit;
                            break;
                    }
                }
            }
        }

        //Connect all completely unconnected points to a connected point, making sure all cells are reachable.
        void ConnectUnconnectedPoints(LevelBlockRequirements[,] basicGrid)
        {
            int hBlocks = Width / BlockWidth, vBlocks = Height / BlockHeight;

            //Store which blocks weren't connected.
            bool[,] isConnected = new bool[hBlocks, vBlocks];
            List<Point> unconnectedPoints = new List<Point>();
            for (int i = 0; i < hBlocks; i++)
            {
                for (int j = 0; j < vBlocks; j++)
                {
                    isConnected[i, j] = ! basicGrid[i, j].IsOnlyWalls();
                    if (! isConnected[i, j])
                        unconnectedPoints.Add(new Point(i, j));
                }
            }

            bool connectedAnythingThisRound;

            while (unconnectedPoints.Count > 0)
            {
                connectedAnythingThisRound = false;

                List<Point> removedUnconnectedPoints = new List<Point>(); //A list to store points we remove as changing a list during foreach is impossible

                unconnectedPoints.Shuffle(); //Shuffle the points so that they connect in a more random way.

                foreach (Point point in unconnectedPoints)
                {
                    int i = point.X, j = point.Y; //Temporary store the x and y of this point.

                    List<Point> possiblePoints = new List<Point>(); //Points to connect to.
                    if (i > 0 && isConnected[i - 1, j])
                        possiblePoints.Add(new Point(i - 1, j));
                    if (i < hBlocks - 1 && isConnected[i + 1, j])
                        possiblePoints.Add(new Point(i + 1, j));
                    if (j > 0 && isConnected[i, j - 1])
                        possiblePoints.Add(new Point(i, j - 1));
                    if (j < vBlocks - 1 && isConnected[i, j + 1])
                        possiblePoints.Add(new Point(i, j + 1));

                    if (possiblePoints.Count != 0)
                    {
                        //Connect this point to a random other one.
                        ConnectPoints(basicGrid, new List<Point>() { point, possiblePoints.GetRandomItem() });
                        isConnected[i, j] = true; //Mark this point as connected.
                        connectedAnythingThisRound = true;
                        removedUnconnectedPoints.Add(point);
                    }
                }

                foreach (Point point in removedUnconnectedPoints)
                    unconnectedPoints.Remove(point);

                //Give up if we couldn't connect anything at all.
                if (! connectedAnythingThisRound)
                    break;
            }
        }

        //Remove walls where they are too thick for no reason.
        //For example, if two generation blocks on top of each other would declare a wall on the side of the other block, this'd remove
        //one of the two walls.
        void PruneWalls(LevelBlockRequirements[,] basicGrid)
        {
            int hBlocks = Width / BlockWidth, vBlocks = Height / BlockHeight;
            for (int i = 0; i < hBlocks; i++)
            {
                for (int j = 0; j < vBlocks; j++)
                {
                    LevelBlockRequirements block = basicGrid[i, j];

                    //We only have to look to the right and bottom, as other blocks check for the left and top (their right and bottom) automatically.

                    //Check to the right
                    if (i < hBlocks - 1 && block.RightSideType == SideType.Wall)
                    {
                        LevelBlockRequirements rightBlock = basicGrid[i + 1, j];
                        if (rightBlock.LeftSideType == SideType.Wall)
                        {
                            switch (random.Next(2))
                            {
                                case 0:
                                    block.RightSideType = SideType.Any;
                                    break;
                                case 1:
                                    rightBlock.LeftSideType = SideType.Any;
                                    break;
                            }
                        }
                    }

                    //Check to the bottom
                    if (j < vBlocks - 1 && block.BottomSideType == SideType.Wall)
                    {
                        LevelBlockRequirements bottomBlock = basicGrid[i, j + 1];
                        if (bottomBlock.BottomSideType == SideType.Wall)
                        {
                            switch (random.Next(2))
                            {
                                case 0:
                                    block.BottomSideType = SideType.Any;
                                    break;
                                case 1:
                                    bottomBlock.TopSideType = SideType.Any;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        //Get a block that meets certain requirements.
        LevelBlock GetPossibleLevelBlock(LevelBlockRequirements requirements)
        {
            WeightedRandomCollection<LevelBlock> possibleLevelBlocks = new WeightedRandomCollection<LevelBlock>();

            //Check all level blocks and see which ones would meet the requirements.
            foreach (LevelBlock levelBlock in levelBlocks)
            {
                if (levelBlock.MeetsRequirements(requirements))
                    possibleLevelBlocks.Add(levelBlock, levelBlock.AppearancePreference);
            }

            //Then return one.
            return possibleLevelBlocks.Get();
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
                    if (exits.Contains("!"))
                        currentLevelBlock.ExitsMustMatch = true;

                    //The second argument (optional) should contain information about how often a block appears.
                    if (arguments.Length >= 2)
                    {
                        string appearancePreferenceString = arguments[1];
                        try
                        {
                            currentLevelBlock.AppearancePreference = double.Parse(appearancePreferenceString, CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            currentLevelBlock.AppearancePreference = 1;
                        }
                    }
                }
                else if (currentLevelBlock != null)
                {
                    if (!string.IsNullOrWhiteSpace(line)) //Set the level data.
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

        //Parse a single special tile definition and store it in memory.
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
                Dictionary<string, char> specialKeywords = new Dictionary<string, char>(); //Special keywords that can be given by level block when placing the tile.
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
                    else if (parts[currentPart].Contains(">")) //The part is a special keyword
                    {
                        string[] keywordchar = parts[currentPart].Split('>');
                        if (keywordchar.Length != 2 || keywordchar[1].Length != 1)
                            throw new Exception("Error in levelblocks definition! Couldn't parse special keyword definition.");
                        specialKeywords.Add(keywordchar[0], keywordchar[1][0]);
                        expectingNext = "OR";
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

                specialTile.SpecialKeywords = specialKeywords;

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
