using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidClone.Engine.Solids
{
    class JumpThrough : GameObject, ISolid
    {
        Rectangle BoundingBox;

        public JumpThrough(Rectangle boundingBox)
        {
            boundingBox.Height = 1;
            BoundingBox = boundingBox;
        }

        bool ISolid.CollidesWith(Rectangle boundingBox)
        {
            Rectangle box = boundingBox;
            box.Y = box.Bottom-1;
            box.Height = 1;
            return BoundingBox.Intersects(box);
        }

        public override void Draw()
        {
            Drawing.DrawRectangle(BoundingBox, Color.Black);
            base.Draw();
        }
    }
}
