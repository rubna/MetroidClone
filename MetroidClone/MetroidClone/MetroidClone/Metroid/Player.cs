using MetroidClone.Engine;
using MetroidClone.Engine.Asset;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidClone.Metroid
{
    class Player : PhysicsObject
    {
        float bulletSpeed = 5;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-8, -8, 16, 16);

            PlayAnimation("tempplayer", speed: 0f);
        }

        public override void Update(GameTime gameTime)
        {
            //move around
            if (Input.KeyboardCheckDown(Keys.Left))
            {
                Speed.X -= 0.5f;
                FlipX = true;
                PlayAnimation("tempplayer", Direction.Left, speed: 0.2f);
            }
            if (Input.KeyboardCheckDown(Keys.Right))
            {
                Speed.X += 0.5f;
                FlipX = true;
                PlayAnimation("tempplayer", Direction.Right, speed: 0.2f);
            }
            if (Input.KeyboardCheckPressed(Keys.Up))
                Speed.Y = -4.5f;

            //shoot
            if (Input.KeyboardCheckPressed(Keys.X))
                Shoot(GetFlip);

            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
        }

        void Shoot(int direction)
        {
            World.AddObject(new Bullet() { Speed = new Vector2(direction * bulletSpeed, 0) }, Position);
        }
    }
}
