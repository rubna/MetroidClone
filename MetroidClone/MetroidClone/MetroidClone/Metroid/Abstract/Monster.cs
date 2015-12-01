using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidClone.Metroid
{
    abstract class Monster : PhysicsObject
    {
        protected int HitPoints = 1;
        protected Vector2 SpeedOnHit = Vector2.Zero;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //check collision player bullets
            foreach (PlayerBullet bullet in World.GameObjects.OfType<PlayerBullet>().ToList())
                if (TranslatedBoundingBox.Intersects(bullet.TranslatedBoundingBox))
                {
                    bullet.Destroy();
                    Hurt(Math.Sign(Position.X - bullet.Position.X));
                }
        }

        void Hurt(int xDirection)
        {
            HitPoints--;
            if (SpeedOnHit != Vector2.Zero)
                Speed = new Vector2(xDirection * SpeedOnHit.X, SpeedOnHit.Y);
            if (HitPoints <= 0)
                Destroy();
        }
    }
}
