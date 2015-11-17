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
            { true, false, false, true, false, false, false, false, true, true },
            { true, false, true, true, true, false, false, false, false, true },
            { true, true, true, true, true, true, true, true, true, true }};
        public Point TileSize = new Point(24, 20);

        public Point LevelDimensions
        {
            get
            {
                return new Point(Grid.GetLength(0), Grid.GetLength(1));
            }
        }

        public override void Draw()
        {
            for (int xp = 0; xp < LevelDimensions.X; xp++)
                for (int yp = 0; yp < LevelDimensions.Y; yp++)
                    if (Grid[xp, yp])
                        Drawing.DrawRectangle(new Rectangle(xp * TileSize.X, (yp * TileSize.Y), TileSize.X, TileSize.Y), Color.Black);
        }
    }
}
