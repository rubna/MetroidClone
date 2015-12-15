using MetroidClone.Engine;
using MetroidClone.Engine.Asset;
using MetroidClone.Metroid.Player_Attacks;
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
        float blinkTimer = 0;
        int collectedScrap = 0;

        public Weapon CurrentWeapon = Weapon.Nothing;
        public List<Weapon> UnlockedWeapons = new List<Weapon>() { Weapon.Nothing };

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-12, -16, 24, 32);

            PlayAnimation("tempplayer", speed: 0f);
        }

        public override void Update(GameTime gameTime)
        {
            //move around
            if (Input.KeyboardCheckDown(Keys.Left) || Input.ThumbStickCheckDirection(true).X < 0)
            {
                Speed.X -= 0.5f;
                FlipX = true;
                PlayAnimation("tempplayer", Direction.Left, speed: 0.2f);
            }

            if (Input.KeyboardCheckDown(Keys.Right) || Input.ThumbStickCheckDirection(true).X > 0)
            {
                Speed.X += 0.5f;
                FlipX = false;
                PlayAnimation("tempplayer", Direction.Right, speed: 0.2f);
            }

            //jump
            if ((Input.KeyboardCheckPressed(Keys.Up) || Input.GamePadCheckPressed(Buttons.A)) && OnGround)
                Speed.Y = -6f;
            if (Speed.Y < 0 && (!Input.KeyboardCheckDown(Keys.Up) || !Input.GamePadCheckDown(Buttons.A)))
            {
                Speed.Y *= 0.9f;
            }

            //drop through jumpthroughs
            if ((Input.KeyboardCheckPressed(Keys.Down) || Input.ThumbStickCheckDirection(true).Y < 0) && OnJumpThrough)
                Position.Y++;

            //attack
            if (Input.KeyboardCheckPressed(Keys.X) || (Input.ThumbStickCheckDown(false)))
                Attack(Input.ThumbStickCheckDirection(false));

            //switch weapons
            if (Input.KeyboardCheckPressed(Keys.C) || Input.GamePadCheckPressed(Buttons.Y))
            {
                NextWeapon();
                Console.WriteLine(CurrentWeapon);
            }

            if (Input.KeyboardCheckPressed(Keys.Z) || Input.GamePadCheckPressed(Buttons.B))
            {
                CreateDrone();
            }

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

        void CreateDrone()
        {
            if (collectedScrap < 25)
                return;
            World.AddObject(new Drone(), Position);
            collectedScrap -= 25;
        }

        public override void Draw()
        {
            base.Draw();
            //Drawing.DrawRectangle(TranslatedBoundingBox, Color.Red);
        }

        void Attack(Vector2 shootAngle)
        {
            shootAngle *= 1 / shootAngle.Length();
            switch ((int)CurrentWeapon)
            {
                case 0:
                    break;
                case 1:
                {
                    World.AddObject(new PlayerBullet() { FlipX = FlipX }, Position);
                    break;
                }
                case 2:
                {
                    World.AddObject(new PlayerMelee(), Position + GetFlip * Vector2.UnitX * 20);
                    break;
                }
                case 3:
        {
                    World.AddObject(new PlayerRocket() { FlipX = FlipX }, Position);
                    break;
                }
                default: break;
            }
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

        void NextWeapon()
        {
            CurrentWeapon++;
            if ((int)CurrentWeapon > 3)
            {
                CurrentWeapon = 0;
                if (!UnlockedWeapons.Contains(CurrentWeapon))
                    NextWeapon();
            }
            if (!UnlockedWeapons.Contains(CurrentWeapon))
                NextWeapon();
        }
    }
    public enum Weapon
    {
        Nothing, Gun, Wrench, Rocket
    }
}
