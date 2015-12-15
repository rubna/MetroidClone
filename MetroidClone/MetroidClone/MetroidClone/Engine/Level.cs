using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    class Level : GameObject
    {
        public bool[,] Grid = {
            { true, true, true, true, true, true, true, true, true, true },
            { true, false, false, false, false, true, false, false, false, true },
            { true, false, false, false, false, true, false, true, false, true },
            { true, false, false, false, false, false, false, true, false, true },
            { true, false, false, false, false, false, false, true, false, true },
            { true, false, false, false, false, false, false, false, true, true },
            { true, false, false, false, false, false, false, false, false, true },
            { true, true, true, true, true, true, true, true, true, true }};
        public Point TileSize = new Point(56, 32);

        public Point LevelDimensions
        {
            get
            {
                return new Point(Grid.GetLength(0), Grid.GetLength(1));
            }
        }

        public override void Create()
        {
            base.Create();
            for (int x = 0; x < LevelDimensions.X; x++)
                for (int y = 0; y < LevelDimensions.Y; y++)
                    if (Grid[x, y])
                        World.AddObject(new Wall(new Rectangle(x * TileSize.X, y * TileSize.Y, TileSize.X, TileSize.Y)));

        }

        public override void Draw()
        {
            //for (int xp = 0; xp < LevelDimensions.X; xp++)
                //for (int yp = 0; yp < LevelDimensions.Y; yp++)
                    //if (Grid[xp, yp])
                        //Drawing.DrawRectangle(new Rectangle(xp * TileSize.X, (yp * TileSize.Y), TileSize.X, TileSize.Y), Color.Black);
        }
    }
}
