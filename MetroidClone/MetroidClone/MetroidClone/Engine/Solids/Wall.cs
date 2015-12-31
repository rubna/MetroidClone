using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    class Wall : GameObject, ISolid
    {
        public Rectangle BoundingBox;

        public Wall(Rectangle boundingBox)
        {
            BoundingBox = boundingBox;
        }

        bool ISolid.CollidesWith(Rectangle box)
        {
            return box.Intersects(BoundingBox);
        }

        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(BoundingBox, Color.Black);
        }
    }
}
