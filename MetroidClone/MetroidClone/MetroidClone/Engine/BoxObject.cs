using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Engine
{
    //For GameObjects with a bounding box.
    class BoxObject : GameObject
    {
        public Rectangle BoundingBox { get; protected set; }
        public Rectangle TranslatedBoundingBox
        {
            get
            {
                Rectangle translatedBoundingBox = BoundingBox;
                translatedBoundingBox.Offset(Position.ToPoint());
                return translatedBoundingBox;
            }
        }

        protected Rectangle DrawBoundingBox
        {
            get
            {
                return new Rectangle(TranslatedBoundingBox.Left - (int)World.Camera.X, TranslatedBoundingBox.Top - (int)World.Camera.Y,
                    TranslatedBoundingBox.Width, TranslatedBoundingBox.Height);
            }
        }

        public override Vector2 CenterPosition { get { return BoundingBox.Center.ToVector2() + Position; } }

        public override void Draw()
        {
            //Draw the current image of the sprite. By default, the size of the bounding box is used.
            if (CurrentSprite != null && Visible)
                Drawing.DrawSprite(CurrentSprite, DrawPosition, (int)CurrentImage, ImageScaling * new Vector2(BoundingBox.Width, BoundingBox.Height));
        }
    }
}
