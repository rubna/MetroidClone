using MetroidClone.Engine;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class WindowTile : BoxObject
    {
        Vector2 size, quarterSize;

        public override bool ShouldUpdate => false;

        public WindowTile(Rectangle boundingBox)
        {
            BoundingBox = boundingBox;

            size = new Vector2(BoundingBox.Width, BoundingBox.Height);
            quarterSize = new Vector2(BoundingBox.Width / 2f, BoundingBox.Height / 2f);
        }

        public override void Create()
        {
            base.Create();

            SetSprite("BackgroundTileset/backgroundwithwindow" + World.Random.Next(1, 4));
        }

        public override void Draw()
        {
            if (CurrentSprite != null && Visible)
                Drawing.DrawSprite(CurrentSprite, DrawBoundingBox.Location.ToVector2(), (int)CurrentImage, size);
        }
    }
}
