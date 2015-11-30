using MetroidClone.Engine;
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
        float blinkTimer = 0;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-8, -8, 16, 16);
        }

        public override void Update(GameTime gameTime)
        {
            //move around
            if (Input.KeyboardCheckDown(Keys.Left))
            {
                Speed.X -= 0.5f;
                FlipX = true;
            }
            if (Input.KeyboardCheckDown(Keys.Right))
            {
                Speed.X += 0.5f;
                FlipX = false;
            }
            if (Input.KeyboardCheckPressed(Keys.Up) && InsideWall(Position.X, Position.Y + 1, BoundingBox))
                Speed.Y = -4.5f;

            //shoot
            if (Input.KeyboardCheckPressed(Keys.X))
                Shoot(GetFlip);

            base.Update(gameTime);

            //check for getting hurt
            if (blinkTimer == 0)
            {
                foreach (Monster monster in World.GameObjects.OfType<Monster>().ToList())
                    if (TranslatedBoundingBox.Intersects(monster.TranslatedBoundingBox))
                        Hurt(Math.Sign(Position.X - monster.Position.X));
            }

            //blink
            if (blinkTimer > 0)
            {
                blinkTimer -= 0.01f;
                if (blinkTimer % 0.05f < 0.0001f)
                {
                    Visible = !Visible;
                }
                if (blinkTimer<=0)
                {
                    blinkTimer = 0;
                    Visible = true;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            //Drawing.DrawRectangle(TranslatedBoundingBox, Color.Red);
            if (Visible)
                Drawing.DrawCircle(new Vector2(TranslatedBoundingBox.Center.X, TranslatedBoundingBox.Center.Y), BoundingBox.Width / 2, Color.Red);
        }

        void Shoot(int xDirection)
        {
            World.AddObject(new PlayerBullet() { Speed = new Vector2(xDirection * bulletSpeed, 0) }, Position);
        }

        void Hurt(int xDirection)
        {
            blinkTimer = 1;
            Visible = false;
            Speed = new Vector2(xDirection * 3, -2);
        }
    }
}
