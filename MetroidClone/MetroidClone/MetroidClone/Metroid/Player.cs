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
        int collectedScrap = 100;
        int timeSinceOnGround = 0;
        const int maxFromPlatformTimeForJump = 5; //The maximum time you can still jump after having moved from a platform.
        float attackTimer = 0;

        int timeSinceLastJumpIntention = 0;
        const int maxTimeSinceLastJumpIntention = 5; //The maximum time you can press the jump button before landing on a platform.

        public int TimeSinceHWallCollision, TimeSinceVWallCollision;

        bool startedSlowingDownJump; //This is used to make sure that the player will jump the maximum height if releasing the jump button slightly before reaching it.

        public Weapon CurrentWeapon = Weapon.Nothing;
        public List<Weapon> UnlockedWeapons = new List<Weapon>() { Weapon.Nothing };
        public int HitPoints = 100;
        public int RocketAmmo = 5;
        public int Score = 0;

        public float movementSpeedModifier; //This can be used to change the movement speed.
        public float jumpHeightModifier; //This can be used to change the jump height.

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

            movementSpeedModifier = 1;
            jumpHeightModifier = 1;
        }

        public override void Update(GameTime gameTime)
        {
            //move around
            if (Input.KeyboardCheckDown(Keys.A) || Input.KeyboardCheckDown(Keys.Left) || Input.ThumbStickCheckDirection(true).X < 0)
            {
                Speed.X -= movementSpeedModifier * 0.5f;
                FlipX = true;
                PlayAnimation("tempplayer", Direction.Left, speed: 0.2f);
            }

            if (Input.KeyboardCheckDown(Keys.D) || Input.KeyboardCheckDown(Keys.Right) || Input.ThumbStickCheckDirection(true).X > 0)
            {
                Speed.X += movementSpeedModifier * 0.5f;
                FlipX = false;
                PlayAnimation("tempplayer", Direction.Right, speed: 0.2f);
            }

            //You can still jump a small time after having walked from a platform
            if (OnGround)
                timeSinceOnGround = 0;
            else
                timeSinceOnGround++;

            //You can press jump a small time before landing on a platform and you'll still jump
            if (Input.KeyboardCheckDown(Keys.W) || Input.KeyboardCheckDown(Keys.Up) || Input.ThumbStickCheckDirection(true).Y > 0.9f || Input.GamePadCheckDown(Buttons.A))
                timeSinceLastJumpIntention = 0;
            else
                timeSinceLastJumpIntention++;

            //jump
            if (timeSinceLastJumpIntention < maxTimeSinceLastJumpIntention && timeSinceOnGround < maxFromPlatformTimeForJump && Speed.Y >= 0)
            {
                Speed.Y = -10f * jumpHeightModifier;
                startedSlowingDownJump = false;
            }

            //jump
            if ((Input.KeyboardCheckPressed(Keys.W) || Input.KeyboardCheckPressed(Keys.Up) || Input.ThumbStickCheckDirection(true).Y > 0.9f || Input.GamePadCheckDown(Buttons.A)) && OnGround)
            {
                Speed.Y = -10f * jumpHeightModifier;
                startedSlowingDownJump = true;
            }
            if ((Speed.Y < 0 && (!Input.KeyboardCheckDown(Keys.W) && !Input.KeyboardCheckDown(Keys.Up) && Input.ThumbStickCheckDirection(true).Y <= 0.9f && !Input.GamePadCheckDown(Buttons.A))) && (Speed.Y < -3 || startedSlowingDownJump))
            {
                Speed.Y *= 0.9f;
                startedSlowingDownJump = true;
            }
            
            //drop through jumpthroughs
            if ((Input.KeyboardCheckPressed(Keys.S) || Input.KeyboardCheckPressed(Keys.Down) || Input.ThumbStickCheckDirection(true).Y < 0) && OnJumpThrough)
                Position.Y++;

            //attack
            if (Input.MouseButtonCheckDown(true) || (Input.ThumbStickCheckDown(false)))
            {
                if (attackTimer == 0)
                {
                Attack();
                    switch ((int)CurrentWeapon)
                    {
                        case 0:
                            break;
                        case 1:
                            {
                                attackTimer = 0.05f;
                                break;
                            }
                        case 2:
                            {
                                attackTimer = 0.1f;
                                break;
                            }
                        case 3:
                            {
                                attackTimer = 0.2f;
                                break;
                            }
                    }
                }
            }

            //switch weapons
            if (Input.KeyboardCheckPressed(Keys.C) || Input.MouseButtonCheckPressed(false) || Input.GamePadCheckPressed(Buttons.Y))
            {
                NextWeapon();
                Console.WriteLine(CurrentWeapon);
            }

            if (Input.KeyboardCheckPressed(Keys.Space) || Input.GamePadCheckPressed(Buttons.B))
            {
                CreateDrone();
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

            if (attackTimer > 0)
            {
                attackTimer -= 0.01f;
                if (attackTimer<=0)
                    attackTimer = 0;
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
            //mouse pointer, disabled when controller in use
            if (!Input.ControllerInUse)
                Drawing.DrawRectangle(new Rectangle(Input.MouseCheckPosition().X - 5, Input.MouseCheckPosition().Y - 5, 10, 10), Color.DarkKhaki);
            //Drawing.DrawRectangle(TranslatedBoundingBox, Color.Red);
        }

        void Attack()
        {
            switch (CurrentWeapon)
            {
                case Weapon.Nothing:
                    break;
                case Weapon.Gun:
                {
                    World.AddObject(new PlayerBullet() { FlipX = FlipX }, Position);
                    break;
                }
                case Weapon.Rocket:
                {
                    World.AddObject(new PlayerMelee(), Position + GetFlip * Vector2.UnitX * 20);
                    break;
                }
                case Weapon.Wrench:
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
