using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class Player : PhysicsObject
    {
        public override void Create()
        {
            base.Create();
            OriginalBoundingBox = new Rectangle(0, 0, 20, 20);
        }

        public override void Draw()
        {
            base.Draw();
            //Rectangle translatedBoundingBox = BoundingBox;
            //translatedBoundingBox.Offset(Position.ToPoint());
            Drawing.DrawRectangle(BoundingBox, Color.Red);
        }
    }
}
