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
        int timeSinceOnGround = 0;
        const int maxFromPlatformTimeForJump = 5; //The maximum time you can still jump after having moved from a platform.

        int timeSinceLastJumpIntention = 0;
        const int maxTimeSinceLastJumpIntention = 5; //The maximum time you can press the jump button before landing on a platform.

        public int TimeSinceHWallCollision, TimeSinceVWallCollision;

        bool startedSlowingDownJump; //This is used to make sure that the player will jump the maximum height if releasing the jump button slightly before reaching it.

        public Weapon CurrentWeapon = Weapon.Nothing;
        public List<Weapon> UnlockedWeapons = new List<Weapon>() { Weapon.Nothing };
        public int HitPoints = 100;
        public int RocketAmmo = 5;
        public int Score = 0;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-12, -16, 24, 32);

            PlayAnimation("tempplayer", speed: 0f);

            Friction = new Vector2(0.85f, 1);
            Gravity = 0.3f;

            startedSlowingDownJump = false;

            TimeSinceHWallCollision = 0;
            TimeSinceVWallCollision = 0;
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

            //You can still jump a small time after having walked from a platform
            if (OnGround)
                timeSinceOnGround = 0;
            else
                timeSinceOnGround++;

            //You can press jump a small time before landing on a platform and you'll still jump
            if (Input.KeyboardCheckPressed(Keys.Up))
                timeSinceLastJumpIntention = 0;
            else
                timeSinceLastJumpIntention++;

            //jump
            if (timeSinceLastJumpIntention < maxTimeSinceLastJumpIntention && timeSinceOnGround < maxFromPlatformTimeForJump && Speed.Y >= 0)
            {
                Speed.Y = -10f;
                startedSlowingDownJump = false;
            }

            if (Speed.Y < 0 && !Input.KeyboardCheckDown(Keys.Up) && (Speed.Y < -3 || startedSlowingDownJump))
            {
                Speed.Y *= 0.9f;
                startedSlowingDownJump = true;
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
                {
                    if (TranslatedBoundingBox.Intersects(monster.TranslatedBoundingBox))
                    {
                        HitPoints = HitPoints - monster.Damage;
                        Hurt(Math.Sign(Position.X - monster.Position.X));
            }
                }
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

            //Check if we had a collision with a wall horizontally and, if so, update the wall collision time.
            if (HadHCollision)
                TimeSinceHWallCollision = 0;
            else
                TimeSinceHWallCollision++;

            //The same but now vertically
            if (HadVCollision)
                TimeSinceVWallCollision = 0;
            else
                TimeSinceVWallCollision++;
        }


        public override void Draw()
        {
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
                        if (RocketAmmo > 0)
                        {
                    World.AddObject(new PlayerRocket() { FlipX = FlipX }, Position);
                            RocketAmmo --;
                        }
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
            if (HitPoints <= 0)
            Console.Write("You are dead");
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
