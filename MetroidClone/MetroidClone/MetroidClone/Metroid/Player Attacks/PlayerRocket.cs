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
        private Vector2 direction;

        public override void Create()
        {
            base.Create();
            Console.WriteLine(Speed);
            Gravity = 0f;
            Friction.X = 0.99f;
            BoundingBox = new Rectangle(-4, -2, 8, 4);
            if (Input.ControllerCheckConnected() && Input.ThumbStickCheckDirection(false) != Vector2.Zero)
                direction = Input.ThumbStickCheckDirection(false);
            else
                direction = Input.MouseCheckPosition().ToVector2() - Position;
        }

        public override void Update(GameTime gameTime)
        {
            Speed += direction * 0.2f;
            base.Update(gameTime);
            if (Speed.Length() < 0.01f)
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
