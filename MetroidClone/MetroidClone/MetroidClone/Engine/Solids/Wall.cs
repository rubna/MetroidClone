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
        protected Rectangle DrawBoundingBox
        {
            get
            {
                return new Rectangle(BoundingBox.Left - (int)World.Camera.X, BoundingBox.Top - (int)World.Camera.Y,
                    BoundingBox.Width, BoundingBox.Height);
            }
        }

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
            Drawing.DrawRectangle(DrawBoundingBox, Color.Black);
        }
    }
}
