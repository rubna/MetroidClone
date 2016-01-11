using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    class SlopeLeft : BoxObject, ISolid
    {
        public SlopeLeft(Rectangle boundingBox)
        {
            BoundingBox = boundingBox;
        }

        bool ISolid.CollidesWith(Rectangle bbox)
        {
            if (bbox.Intersects(BoundingBox))
            {
                Vector2 bottomRight = new Vector2(bbox.Left, bbox.Bottom);
                if (bottomRight.DistanceToLine(new Vector2(BoundingBox.Left, BoundingBox.Top), new Vector2(BoundingBox.Right, BoundingBox.Bottom)) <= 0)
                    return true;
            }
            return false;
        }
        public override void Draw()
        {
            base.Draw();
            Drawing.DrawTriangle(new Vector2(DrawBoundingBox.Left, DrawBoundingBox.Bottom), new Vector2(DrawBoundingBox.Left, DrawBoundingBox.Top),
                new Vector2(DrawBoundingBox.Right, DrawBoundingBox.Bottom), Color.Black);
        }
    }
}
