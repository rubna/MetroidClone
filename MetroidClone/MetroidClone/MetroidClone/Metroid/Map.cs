using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MetroidClone.Metroid
{
    //Class for displaying the game map
    class Map : GameObject
    {
        bool[,] hasVisitedCell;

        public override bool ShouldDrawGUI => true; //This object has a GUI event.

        public Map()
        {
            Depth = 10; //Should be drawn below other GUI items.

            hasVisitedCell = new bool[WorldGenerator.WorldWidth, WorldGenerator.WorldHeight];
        }

        public void DrawMap(Point mapCenter)
        {
            const int mapSquareSize = 8;

            //The cell with the player should always be at the center.
            Vector2 positionModifier = getCell(new Vector2(World.Player.Position.X, World.Player.Position.Y)).ToVector2() + new Vector2(0.5f, 0.5f);
            positionModifier.X *= mapSquareSize * WorldGenerator.LevelWidth;
            positionModifier.Y *= mapSquareSize * WorldGenerator.LevelHeight;
            positionModifier = mapCenter.ToVector2() - positionModifier;

            //Draw all visited game cells
            Point playerPos = getCell(World.Player.Position);
            for (int i = 0; i < WorldGenerator.WorldWidth; i++)
                for (int j = 0; j < WorldGenerator.WorldHeight; j++)
                {
                    if (hasVisitedCell[i, j])
                    {
                        Color drawColor = Color.Black; //By default, game cells are black.

                        //However, if the player is here, it's blueish.
                        if (playerPos.X == i && playerPos.Y == j)
                            drawColor = new Color(0, 0, 80);

                        Drawing.DrawRectangleUnscaled(new Rectangle(i * WorldGenerator.LevelWidth * mapSquareSize + (int) positionModifier.X,
                            j * WorldGenerator.LevelHeight * mapSquareSize + (int) positionModifier.Y,
                            mapSquareSize * WorldGenerator.LevelWidth, mapSquareSize * WorldGenerator.LevelHeight), drawColor);
                    }
                }

            //Draw all game objects (except some that shouldn't be drawn).
            foreach (GameObject gameObject in World.GameObjects)
            {
                Point cell = getCell(gameObject.CenterPosition);
                if (gameObject.Visible && hasVisitedCell[cell.X, cell.Y])
                {
                    int hsizemod = 1, vsizemod = 1;

                    Color drawColor;
                    Point position = gameObject.Position.ToPoint();
                    if (gameObject is Wall)
                    {
                        drawColor = Color.White;
                        position = (gameObject as Wall).BoundingBox.Location;
                    }
                    else if (gameObject is Player)
                    {
                        drawColor = Color.DeepSkyBlue;
                        position = gameObject.Position.ToPoint();
                    }
                    else if (gameObject is GunPickup || gameObject is RocketPickup || gameObject is WrenchPickup)
                    {
                        drawColor = Color.DarkGreen;
                        position = gameObject.Position.ToPoint();
                    }
                    else if (gameObject is BossDoor) //Boss door
                    {
                        if ((gameObject as Door).Activated)
                            drawColor = Color.Gray;
                        else
                            drawColor = Color.LawnGreen;
                        position = gameObject.Position.ToPoint();
                        vsizemod = 2;
                    }
                    else if (gameObject is MeleeDoor) //Melee door
                    {
                        if ((gameObject as Door).Activated)
                            drawColor = Color.Gray;
                        else
                            drawColor = new Color(253, 91, 82);
                        position = gameObject.Position.ToPoint();
                        vsizemod = 2;
                    }
                    else if (gameObject is RocketDoor) //Rocket door
                    {
                        if ((gameObject as Door).Activated)
                            drawColor = Color.Gray;
                        else
                            drawColor = new Color(238, 167, 3);
                        position = gameObject.Position.ToPoint();
                        vsizemod = 2;
                    }
                    else if (gameObject is Door) //Other doors
                    {
                        if ((gameObject as Door).Activated)
                            drawColor = Color.Gray;
                        else
                            drawColor = Color.LawnGreen;
                        position = gameObject.Position.ToPoint();
                        vsizemod = 2;
                    }
                    else
                        continue;

                    Drawing.DrawRectangleUnscaled(new Rectangle(position.X / World.TileWidth * mapSquareSize + (int) positionModifier.X,
                         position.Y / World.TileHeight * mapSquareSize + (int) positionModifier.Y, mapSquareSize * hsizemod, mapSquareSize * vsizemod), drawColor);
                }
            }
        }

        public override void DrawGUI()
        {
            if (Input.KeyboardCheckDown(Keys.M) || Input.GamePadCheckDown(Buttons.Back))
            {
                //Draw a black background (and get the center position of the map)
                Point mapCenter = Drawing.DrawOverlayStart();

                //Draw the map
                DrawMap(mapCenter);
            }
        }

        public override void Update(GameTime gameTime)
        {
            Point currentCell = getCell(World.Player.Position);
            hasVisitedCell[currentCell.X, currentCell.Y] = true;
        }

        //Get the index of the world cell the position is in.
        Point getCell(Vector2 position)
        {
            return new Point((int)(position.X / (WorldGenerator.LevelWidth * World.TileWidth)),
                (int)(position.Y / (WorldGenerator.LevelHeight * World.TileHeight)));
        }
    }
}
