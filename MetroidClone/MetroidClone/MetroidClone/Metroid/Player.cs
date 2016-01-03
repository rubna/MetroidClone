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

        public float Rotation = 0;
        public float AnimationRotation = 0;
        AnimationBone body, hipLeft, kneeLeft, footLeft, hipRight, kneeRight, footRight;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-12, -16, 24, 32);

            //make skeleton
            body = new AnimationBone(this) { Offset = new Vector2(0, 0) };
            hipLeft = new AnimationBone(body) { Offset = new Vector2(-8, 5) };
            kneeLeft = new AnimationBone(hipLeft) { Offset = new Vector2(0, 8) };
            footLeft = new AnimationBone(kneeLeft) { Offset = new Vector2(0, 8) };

            hipRight = new AnimationBone(body) { Offset = new Vector2(8, 5) };
            kneeRight = new AnimationBone(hipRight) { Offset = new Vector2(0, 8) };
            footRight = new AnimationBone(kneeRight) { Offset = new Vector2(0, 8) };

            World.AddObject(body);
            World.AddObject(hipLeft);
            World.AddObject(kneeLeft);
            World.AddObject(footLeft);
            World.AddObject(hipRight);
            World.AddObject(kneeRight);
            World.AddObject(footRight);

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

            //jump
            if (Input.KeyboardCheckPressed(Keys.Up) && OnGround)
                Speed.Y = -6f;
            if (Speed.Y < 0 && !Input.KeyboardCheckDown(Keys.Up))
            {
                Speed.Y *= 0.9f;
            }

            //drop through jumpthroughs
            if (Input.KeyboardCheckPressed(Keys.Down) && OnJumpThrough)
                Position.Y++;

            //attack
            if (Input.KeyboardCheckPressed(Keys.X))
                Attack();

            //switch weapons
            if (Input.KeyboardCheckPressed(Keys.C))
            {
                NextWeapon();
                Console.WriteLine(CurrentWeapon);
            }

            base.Update(gameTime);

            //check for getting hurt
            if (blinkTimer == 0)
            {
                foreach (Monster monster in World.GameObjects.OfType<Monster>().ToList())
                    if (TranslatedBoundingBox.Intersects(monster.TranslatedBoundingBox))
                        Hurt(Math.Sign(Position.X - monster.Position.X));
            }

            //Collect scrap
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

            //animation
            PlayAnimationWalking();
        }

        void PlayAnimationWalking()
        {
            AnimationRotation += 8;
            AnimationRotation %= 360;


            hipLeft.ImageRotation = VectorExtensions.LengthDirectionX(45, AnimationRotation + 180);
            hipRight.ImageRotation = VectorExtensions.LengthDirectionX(45, AnimationRotation);
            kneeLeft.ImageRotation = VectorExtensions.LengthDirectionX(45, AnimationRotation + 180 - 45);
            kneeRight.ImageRotation = VectorExtensions.LengthDirectionX(45, AnimationRotation - 45);

        }

        public override void Draw()
        {
            Drawing.DrawRectangle(TranslatedBoundingBox, Color.Red);
            base.Draw();
        }

        void Attack()
        {
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
