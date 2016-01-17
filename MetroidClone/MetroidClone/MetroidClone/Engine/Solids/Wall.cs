using Microsoft.Xna.Framework;

namespace MetroidClone.Engine
{
    class Wall : BoxObject, ISolid
    {
        public string BasicConnectionSprite = "";

        public bool ShouldShowTopLeftConnection { set; protected get; }
        public bool ShouldShowTopRightConnection { set; protected get; }
        public bool ShouldShowBottomLeftConnection { set; protected get; }
        public bool ShouldShowBottomRightConnection { set; protected get; }

        Vector2 size, quarterSize;

        public override bool ShouldUpdate => false;

        public Wall(Rectangle boundingBox)
        {
            BoundingBox = boundingBox;

            size = new Vector2(BoundingBox.Width, BoundingBox.Height);
            quarterSize = new Vector2(BoundingBox.Width / 2f, BoundingBox.Height / 2f);
        }

        bool ISolid.CollidesWith(Rectangle box)
        {
            return box.Intersects(BoundingBox);
        }

        public override void Draw()
        {
            //Draw the wall at the bounding box.
            Vector2 drawPos = DrawBoundingBox.Location.ToVector2();

            if (CurrentSprite != null && Visible)
                Drawing.DrawSprite(CurrentSprite, drawPos, (int)CurrentImage, size);

            if (ShouldShowTopLeftConnection)
                Drawing.DrawSprite(BasicConnectionSprite + "UL", drawPos, (int)CurrentImage, quarterSize);
            if (ShouldShowTopRightConnection)
                Drawing.DrawSprite(BasicConnectionSprite + "UR", new Point(DrawBoundingBox.Center.X, (int) drawPos.Y).ToVector2(), (int)CurrentImage, quarterSize);
            if (ShouldShowBottomLeftConnection)
                Drawing.DrawSprite(BasicConnectionSprite + "DL", new Point((int) drawPos.X, DrawBoundingBox.Center.Y).ToVector2(), (int)CurrentImage, quarterSize);
            if (ShouldShowBottomRightConnection)
                Drawing.DrawSprite(BasicConnectionSprite + "RD", new Point(DrawBoundingBox.Center.X, DrawBoundingBox.Center.Y).ToVector2(), (int)CurrentImage, quarterSize);
        }
    }
}
