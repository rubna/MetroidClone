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
        float blinkTimer = 0;
        int collectedScrap = 0;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-6, -8, 12, 16);

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
                FlipX = false;
                PlayAnimation("tempplayer", Direction.Right, speed: 0.2f);
            }
            if (Input.KeyboardCheckPressed(Keys.Up))
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

            foreach (Scrap scrap in World.GameObjects.OfType<Scrap>().ToList())
                if (TranslatedBoundingBox.Intersects(scrap.TranslatedBoundingBox))
                    Collect(scrap);

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
            Console.WriteLine(collectedScrap);
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

        void Collect(Scrap scrap)
        {
            collectedScrap += scrap.ScrapAmount;
            scrap.Destroy();
        }
    }
}
