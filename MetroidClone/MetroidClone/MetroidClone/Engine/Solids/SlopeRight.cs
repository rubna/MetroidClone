using Microsoft.Xna.Framework;

namespace MetroidClone.Engine
{
    class SlopeRight : BoxObject, ISolid
    {
        public SlopeRight(Rectangle boundingBox)
        {
            BoundingBox = boundingBox;
        }

        public override void Create()
        {
            base.Create();

            SetSprite("Tileset/slopeR");
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
            Drawing.DrawSprite(CurrentSprite, DrawBoundingBox.Location.ToVector2(), (int)CurrentImage, ImageScaling * new Vector2(BoundingBox.Width, BoundingBox.Height));
        }
    }
}
