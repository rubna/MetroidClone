using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine.Solids
{
    class SlopeRight : GameObject, ISolid
    {
        public Rectangle BoundingBox;

        public SlopeRight(Rectangle boundingBox)
        {
            BoundingBox = boundingBox;
        }

        bool ISolid.CollidesWith(Rectangle bbox)
        {
            if (bbox.Intersects(BoundingBox))
            {
                Vector2 bottomRight = new Vector2(bbox.Right, bbox.Bottom);
                if (bottomRight.DistanceToLine(new Vector2(BoundingBox.Left, BoundingBox.Bottom), new Vector2(BoundingBox.Right, BoundingBox.Top)) <= 0)
                    return true;
            }
            return false;
        }
        public override void Draw()
        {
            base.Draw();
            Drawing.DrawTriangle(new Vector2(BoundingBox.Left, BoundingBox.Bottom), new Vector2(BoundingBox.Right, BoundingBox.Top), new Vector2(BoundingBox.Right, BoundingBox.Bottom), Color.Black);
        }
    }
}
