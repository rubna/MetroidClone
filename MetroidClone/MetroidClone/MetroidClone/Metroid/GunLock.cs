using MetroidClone.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class GunLock : PhysicsObject, ISolid
    {
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-5, -20, 10, 40);
            Gravity = 0;
        }

        bool ISolid.CollidesWith(Rectangle box)
        {
            return box.Intersects(TranslatedBoundingBox);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (PlayerBullet bullet in World.GameObjects.OfType<PlayerBullet>().ToList())
                if (CollidesWith(Position.X - bullet.Speed.X, Position.Y, bullet))
                    Destroy();
            base.Update(gameTime);
        }
        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(DrawBoundingBox, Color.Blue);
        }
    }
}
