using MetroidClone.Engine;
using MetroidClone.Metroid.Player_Attacks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetroidClone.Metroid
{
    class Player : PhysicsObject
    {
        float blinkTimer = 0;
        public int CollectedScrap = 100;
        int timeSinceOnGround = 0;
        const int maxFromPlatformTimeForJump = 5; //The maximum time you can still jump after having moved from a platform.
        float attackTimer = 0;

        int timeSinceLastJumpIntention = 10;
        const int maxTimeSinceLastJumpIntention = 5; //The maximum time you can press the jump button before landing on a platform.

        public int TimeSinceHWallCollision, TimeSinceVWallCollision;

        bool startedSlowingDownJump; //This is used to make sure that the player will jump the maximum height if releasing the jump button slightly before reaching it.

        public Weapon CurrentWeapon = Weapon.Nothing;
        public List<Weapon> UnlockedWeapons = new List<Weapon>() { Weapon.Nothing };
        public int HitPoints = 50, MaxHitPoints = 100;
        public int RocketAmmo = 5;
        public int Score = 0;

        public float movementSpeedModifier; //This can be used to change the movement speed.
        public float jumpHeightModifier; //This can be used to change the jump height.

        const float jumpSpeed = 8f; //The base jumping speed. Was: 10f
        const float gravity = 0.2f; //The base gravity. Was: 0.3f

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-12, -16, 24, 32);

            PlayAnimation("tempplayer", speed: 0f);

            Friction = new Vector2(0.85f, 1);
            Gravity = gravity;

            startedSlowingDownJump = false;

            TimeSinceHWallCollision = 0;
            TimeSinceVWallCollision = 0;

            movementSpeedModifier = 1;
            jumpHeightModifier = 1;
        }

        public override void Update(GameTime gameTime)
        {
            bool hasMovedLeft = false, hasMovedRight = false;

            //move around
            if (Input.KeyboardCheckDown(Keys.A) || Input.KeyboardCheckDown(Keys.Left) || Input.ThumbStickCheckDirection(true).X < 0)
            {
                Speed.X -= movementSpeedModifier * 0.5f;
                FlipX = true;
                PlayAnimation("tempplayer", Direction.Left, speed: 0.2f);
                hasMovedLeft = true;
            }

            if (Input.KeyboardCheckDown(Keys.D) || Input.KeyboardCheckDown(Keys.Right) || Input.ThumbStickCheckDirection(true).X > 0)
            {
                Speed.X += movementSpeedModifier * 0.5f;
                FlipX = false;
                PlayAnimation("tempplayer", Direction.Right, speed: 0.2f);
                hasMovedRight = true;
            }

            //You can still jump a small time after having walked from a platform
            if (OnGround)
                timeSinceOnGround = 0;
            else
                timeSinceOnGround++;

            //You can press jump a small time before landing on a platform and you'll still jump
            if (Input.KeyboardCheckPressed(Keys.W) || Input.KeyboardCheckPressed(Keys.Up) || Input.ThumbStickCheckDirection(true).Y > 0.75f || Input.GamePadCheckPressed(Buttons.A))
                timeSinceLastJumpIntention = 0;
            else
                timeSinceLastJumpIntention++;

            //jump
            if (timeSinceLastJumpIntention < maxTimeSinceLastJumpIntention && timeSinceOnGround < maxFromPlatformTimeForJump && Speed.Y >= 0)
            {
                Speed.Y = - jumpSpeed * jumpHeightModifier;
                startedSlowingDownJump = false;
            }

            if ((Speed.Y < 0 && (!Input.KeyboardCheckDown(Keys.W) && !Input.KeyboardCheckDown(Keys.Up) && Input.ThumbStickCheckDirection(true).Y <= 0.75f && !Input.GamePadCheckDown(Buttons.A))) && (Speed.Y < -3 || startedSlowingDownJump))
            {
                Speed.Y *= 0.9f;
                startedSlowingDownJump = true;
            }

            //drop through jumpthroughs
            if ((Input.KeyboardCheckDown(Keys.S) || Input.KeyboardCheckDown(Keys.Down) || Input.ThumbStickCheckDirection(true).Y < 0) && OnJumpThrough)
                Position.Y++;

            //attack
            if (Input.MouseButtonCheckDown(true) || (Input.ThumbStickCheckDown(false)))
            {
                if (attackTimer == 0)
                {
                    Attack();
                    switch ((int)CurrentWeapon)
                    {
                        case (int)Weapon.Nothing:
                            break;
                        case (int)Weapon.Gun:
                            {
                                attackTimer = 0.1f;
                                break;
                            }
                        case (int)Weapon.Rocket:
                            {
                                attackTimer = 0.2f;
                                break;
                            }
                    }
                }
            }

            if (Input.KeyboardCheckPressed(Keys.F) || Input.MouseWheelPressed() || Input.GamePadCheckPressed(Buttons.B))
            {
                if (attackTimer == 0 && UnlockedWeapons.Contains(Weapon.Wrench))
                {
                    World.AddObject(new PlayerMelee(), Position + GetFlip * Vector2.UnitX * 20);
                    attackTimer = 0.1f;
                }
            }

            //switch weapons
            if (Input.KeyboardCheckPressed(Keys.C) || Input.MouseWheelCheckScroll(true) || Input.MouseWheelCheckScroll(false) || Input.GamePadCheckPressed(Buttons.Y))
            {
                NextWeapon();
                Console.WriteLine(CurrentWeapon);
            }

            if (Input.KeyboardCheckPressed(Keys.Space) || Input.GamePadCheckPressed(Buttons.X))
            {
                CreateDrone();
            }

            base.Update(gameTime);

            //check for getting hurt
            if (blinkTimer == 0)
                foreach (Monster monster in World.GameObjects.OfType<Monster>().ToList())
                    if (TranslatedBoundingBox.Intersects(monster.TranslatedBoundingBox))
                        Hurt(Math.Sign(Position.X - monster.Position.X), monster.Damage);

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

            Rectangle offsetCollBox = TranslatedBoundingBox;
            offsetCollBox.Offset(0, 6);

            //If you've moved left...
            if (hasMovedLeft)
            {
                //Make sure the player moves down slopes correctly.
                if (GetCollisionWithSolid<SlopeRight>(offsetCollBox) != null)
                {
                    while (GetCollisionWithSolid<SlopeRight>(TranslatedBoundingBox) == null)
                        Position.Y += 1;
                    Position.Y -= 1;
                }
            }

            //If you've moved right...
            if (hasMovedRight)
            {
                //Make sure the player moves down slopes correctly.
                if (GetCollisionWithSolid<SlopeLeft>(offsetCollBox) != null)
                {
                    while (GetCollisionWithSolid<SlopeRight>(TranslatedBoundingBox) == null)
                        Position.Y += 1;
                    Position.Y -= 1;
                }
            }
        }

        void CreateDrone()
        {
            if (CollectedScrap < 25)
                return;
            World.AddObject(new Drone(), Position);
            CollectedScrap -= 25;
        }

        public override void Draw()
        {
            base.Draw();
            //mouse pointer, disabled when controller in use
            Point mousePos = Input.MouseCheckUnscaledPosition(Drawing);
            if (!Input.ControllerInUse)
                Drawing.DrawRectangle(new Rectangle(Input.MouseCheckUnscaledPosition(Drawing).X - 5, Input.MouseCheckUnscaledPosition(Drawing).Y - 5, 10, 10), Color.DarkKhaki);
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
                    if (RocketAmmo > 0)
                    {
                        World.AddObject(new PlayerRocket() { FlipX = FlipX }, Position);
                        RocketAmmo --;
                    }
                    break;
                }
            }
        }

        void Hurt(int xDirection, int damage)
        {
            HitPoints -= damage;
            Input.GamePadVibrate(0.1f * damage, 0.1f * damage, 0.1f);
            blinkTimer = 1;
            Input.GamePadVibrate(0.5f, 0.5f, 100);
            Visible = false;
            Speed = new Vector2(xDirection * 3, -2);
            if (HitPoints <= 0)
                Console.Write("You are dead");
        }

        void Collect(Scrap scrap)
        {
            CollectedScrap += scrap.ScrapAmount;
            scrap.Destroy();
        }

        void NextWeapon()
        {
            if (CurrentWeapon == Weapon.Gun && UnlockedWeapons.Contains(Weapon.Rocket))
                CurrentWeapon = Weapon.Rocket;
            if (CurrentWeapon == Weapon.Rocket && UnlockedWeapons.Contains(Weapon.Gun))
                CurrentWeapon = Weapon.Gun;
        }
    }
    public enum Weapon
    {
        Nothing, Gun, Wrench, Rocket
    }
}
