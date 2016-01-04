using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class Scrap : Collectible
    {
        public int ScrapAmount;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-10, -10, 20, 20);
            ScrapAmount = World.Random.Next(5, 11);
        }

        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(DrawBoundingBox, Color.DarkSlateGray);
        }
    }
}
