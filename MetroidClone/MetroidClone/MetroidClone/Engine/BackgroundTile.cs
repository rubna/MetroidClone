using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    /*class BackgroundTile : BoxObject
    {
        Vector2? size;

        public override bool ShouldUpdate => false;

        public BackgroundTile(Rectangle boundingBox)
        {
            Depth = 200;
            BoundingBox = boundingBox;
            size = new Vector2(BoundingBox.Width, BoundingBox.Height);
        }

        public override void Create()
        {
            base.Create();

            SetSprite("Sprites/BackgroundTileset/backgroundwithwindow");
        }

        public override void Draw()
        {
            //Draw the background tile at the bounding box.
            Drawing.DrawSprite(CurrentSprite, DrawBoundingBox.Location.ToVector2(), 0, size);
        }
    }*/
}
