using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void DrawMap()
        {
            foreach (GameObject gameObject in World.GameObjects)
            {
                Point cell = getCell(gameObject.CenterPosition);
                if (gameObject.Visible && hasVisitedCell[cell.X, cell.Y])
                {
                    Color drawColor;
                    Point position = gameObject.Position.ToPoint();
                    if (gameObject is Wall)
                    {
                        drawColor = Color.Gray;
                        position = (gameObject as Wall).BoundingBox.Location;
                    }
                    else if (gameObject is Player)
                    {
                        drawColor = Color.DeepSkyBlue;
                        position = gameObject.Position.ToPoint();
                    }
                    else if (gameObject is GunPickup || gameObject is RocketPickup || gameObject is WrenchPickup)
                    {
                        drawColor = Color.LawnGreen;
                        position = gameObject.Position.ToPoint();
                    }
                    else
                        continue;

                    Drawing.DrawRectangle(new Rectangle((int) (position.X /  World.TileWidth) * 4,
                        (int) (position.Y / World.TileHeight) * 4, 4, 4), drawColor);
                }
            }
        }

        public override void DrawGUI()
        {
            if (Input.KeyboardCheckDown(Keys.M) || Input.GamePadCheckDown(Buttons.Back))
                DrawMap();
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
