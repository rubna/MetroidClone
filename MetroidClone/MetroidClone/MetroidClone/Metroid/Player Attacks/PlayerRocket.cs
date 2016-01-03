using MetroidClone.Metroid.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidClone.Engine;

namespace MetroidClone.Metroid.Player_Attacks
{
    class PlayerRocket : PhysicsObject, IPlayerAttack
    {
        public override void Create()
        {
            base.Create();
            Gravity = 0f;
            Friction.X = 0.99f;
            BoundingBox = new Rectangle(-4, -2, 8, 4);
        }

        public override void Update(GameTime gameTime)
        {
            Speed.X += GetFlip * 0.2f;
            base.Update(gameTime);
            if (Math.Abs(Speed.X) < 0.01f || PositionPrevious.Y != Position.Y)
                Destroy();
        }

        public override void Draw()
        {
            Drawing.DrawRectangle(TranslatedBoundingBox,Color.Green);
            base.Draw();
        }

        public override void Destroy()
        {
            base.Destroy();
            World.AddObject(new PlayerExplosion(), Position);
        }
    }
}
