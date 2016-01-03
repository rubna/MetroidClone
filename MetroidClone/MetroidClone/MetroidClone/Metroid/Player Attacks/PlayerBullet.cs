using MetroidClone.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    class PlayerBullet : PhysicsObject, IPlayerAttack
    {
        public override void Create()
        {
            BoundingBox = new Rectangle(-4, -4, 8, 8);
            Friction.X = 1;
            Gravity = 0;
            CollideWithWalls = false;
            Speed.X = 5 * GetFlip;
            base.Create();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Speed.X < 0.01f || Position.Y != PositionPrevious.Y)
                Destroy();
        }

        public override void Draw()
        {
            Drawing.DrawRectangle(TranslatedBoundingBox, Color.Blue);
            base.Draw();
        }
    }
}
