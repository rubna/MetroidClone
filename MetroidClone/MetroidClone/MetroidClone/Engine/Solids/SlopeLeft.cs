using Microsoft.Xna.Framework;

namespace MetroidClone.Engine
{
    class SlopeLeft : BoxObject, ISolid
    {
        public SlopeLeft(Rectangle boundingBox)
        {
            BoundingBox = boundingBox;
        }

        public override void Create()
        {
            base.Create();

            SetSprite("Tileset/slopeL");
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
            Drawing.DrawSprite(CurrentSprite, DrawBoundingBox.Location.ToVector2(), (int)CurrentImage, ImageScaling * new Vector2(BoundingBox.Width, BoundingBox.Height));
        }
    }
}
