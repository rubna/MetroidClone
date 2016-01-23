using Microsoft.Xna.Framework;

namespace MetroidClone.Engine
{
    class JumpThrough : BoxObject, ISolid
    {
        public JumpThrough(Rectangle boundingBox)
        {
            boundingBox.Height = 1;
            BoundingBox = boundingBox;
            Depth = 20;
        }

        bool ISolid.CollidesWith(Rectangle boundingBox)
        {
            Rectangle box = boundingBox;
            box.Y = box.Bottom - 1;
            box.Height = 1;
            return BoundingBox.Intersects(box);
        }

        public override void Draw()
        {
            //Bounding box width and height should be equal for drawing.
            Drawing.DrawSprite("Tileset/platform", DrawBoundingBox.Location.ToVector2(), size: new Vector2(BoundingBox.Width));
        }
    }
}
