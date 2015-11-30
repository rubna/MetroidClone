using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    class PlayerBullet : PhysicsObject
    {
        public override void Create()
        {
            BoundingBox = new Rectangle(-4, -4, 8, 8);
            XFriction = 1;
            Gravity = 0;
            CollideWithWalls = false;
            base.Create();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (InsideWall(TranslatedBoundingBox))
                Destroy();
        }

        public override void Draw()
        {
            Drawing.DrawRectangle(TranslatedBoundingBox, Color.Blue);
            base.Draw();
        }
    }
}
