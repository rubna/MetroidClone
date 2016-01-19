using MetroidClone.Engine;
using MetroidClone.Metroid.Player_Attacks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetroidClone.Metroid
{
    partial class Player : PhysicsObject
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

        float moveXAxis = 0;
        bool up = false;
        bool upPressed = false;
        bool down = false;

        public Weapon CurrentWeapon = Weapon.Gun;
        public List<Weapon> UnlockedWeapons = new List<Weapon>() { Weapon.Nothing };
        public int HitPoints = 100, MaxHitPoints = 100;
        public int RocketAmmo = 5;
        public int Score = 0;

        public float Rotation = 0;
        public float AnimationRotation = 0;
        AnimationBone body, hipLeft, kneeLeft, footLeft, hipRight, kneeRight, footRight, head, 
                    shoulderLeft, shoulderRight, elbowLeft, elbowRight, handLeft, handRight, gun;

        float shotAnimationTimer = 0;
        float shootDirection = 0;
        float fellThroughTimer = 0;

        public float movementSpeedModifier; //This can be used to change the movement speed.
        public float jumpHeightModifier; //This can be used to change the jump height.

        const float jumpSpeed = 8f; //The base jumping speed. Was: 10f
        const float gravity = 0.2f; //The base gravity. Was: 0.3f

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-10, -16, 20, 32);
            Depth = -10;

            //make skeleton
            body = new AnimationBone(this, new Vector2(0, -1));
            head = new AnimationBone(body, new Vector2(0, -18));
            hipLeft = new AnimationBone(body, new Vector2(-5, -1)) { DepthOffset = 1 };
            hipRight = new AnimationBone(body, new Vector2(5, -1)) { DepthOffset = 1 };
            kneeLeft = new AnimationBone(hipLeft, new Vector2(0, 8)) { DepthOffset = 1 };
            kneeRight = new AnimationBone(hipRight, new Vector2(0, 8)) { DepthOffset = 1 };
            footLeft = new AnimationBone(kneeLeft, new Vector2(0, 8));
            footRight = new AnimationBone(kneeRight, new Vector2(0, 8));

            shoulderRight = new AnimationBone(body, new Vector2(-5.5f, -13.5f));
            shoulderLeft = new AnimationBone(body, new Vector2(5.5f, -14)) { DepthOffset = 1 };
            elbowRight = new AnimationBone(shoulderRight, new Vector2(-8, 0));
            elbowLeft = new AnimationBone(shoulderLeft, new Vector2(8, 0)) { DepthOffset = 1 };
            handRight = new AnimationBone(elbowRight, new Vector2(-7, 0)) { DepthOffset = 1 };
            handLeft = new AnimationBone(elbowLeft, new Vector2(7, 0)) { DepthOffset = 1 };

            gun = new AnimationBone(handRight, new Vector2(-2, 0)) { DepthOffset = -4 };

            World.AddObject(body);
            body.SetSprite("Robot/RobotSpriteBody");
            World.AddObject(head);
            head.SetSprite("Robot/RobotSpriteHead");

            World.AddObject(hipLeft);
            hipLeft.SetSprite("Robot/RobotSpriteLLeg1");
            World.AddObject(hipRight);
            hipRight.SetSprite("Robot/RobotSpriteRLeg1");
            World.AddObject(kneeLeft);
            kneeLeft.SetSprite("Robot/RobotSpriteLLeg2");
            World.AddObject(kneeRight);
            kneeRight.SetSprite("Robot/RobotSpriteRLeg2");
            World.AddObject(footLeft);
            footLeft.SetSprite("Robot/RobotSpriteLFoot");
            World.AddObject(footRight);
            footRight.SetSprite("Robot/RobotSpriteRFoot");

            World.AddObject(shoulderLeft);
            shoulderLeft.SetSprite("Robot/RobotSpriteLArm2");
            World.AddObject(shoulderRight);
            shoulderRight.SetSprite("Robot/RobotSpriteRArm2");
            World.AddObject(elbowLeft);
            elbowLeft.SetSprite("Robot/RobotSpriteLArm1");
            World.AddObject(elbowRight);
            elbowRight.SetSprite("Robot/RobotSpriteRArm1");
            World.AddObject(handLeft);
            handLeft.SetSprite("Robot/RobotSpriteLHand");
            World.AddObject(handRight);
            handRight.SetSprite("Robot/RobotSpriteRHand");

            World.AddObject(gun);
            gun.SetSprite("Items/gun");
            gun.SpriteScale = 0.2f;
            gun.TargetRotation = 90;

            Friction = new Vector2(0.85f, 1);
            Gravity = gravity;

            startedSlowingDownJump = false;

            TimeSinceHWallCollision = 0;
            TimeSinceVWallCollision = 0;

            movementSpeedModifier = 1;
            jumpHeightModifier = 1;
            //PlayAnimation("tempplayer", speed: 0f);
        }

        public override void Update(GameTime gameTime)
        {
            bool walking = false;

            //set movement axes
            moveXAxis = 0;
            upPressed = false;

            //We haven't moved left or right yet.
            bool hasMovedLeft = false, hasMovedRight = false;

            //move around
            if (Input.KeyboardCheckDown(Keys.A) || Input.KeyboardCheckDown(Keys.Left) || Input.ThumbStickCheckDirection(true).X < 0)
            {
                moveXAxis--;
                hasMovedLeft = true;
            }
            if (Input.KeyboardCheckDown(Keys.D) || Input.KeyboardCheckDown(Keys.Right) || Input.ThumbStickCheckDirection(true).X > 0)
            {
                moveXAxis++;
                hasMovedRight = true;
            }
            if (Input.KeyboardCheckPressed(Keys.W) || Input.KeyboardCheckPressed(Keys.Up) || Input.ThumbStickCheckDirection(true).Y > 0.75f || Input.GamePadCheckPressed(Buttons.A))
            {
                upPressed = !up;
                up = true;
            }
            else
                up = false;
            if (Input.KeyboardCheckDown(Keys.S) || Input.KeyboardCheckDown(Keys.Down) || Input.ThumbStickCheckDirection(true).Y < 0)
                down = true;
            else
                down = false;

            //move horizontally
            if (moveXAxis != 0)
            {
                Speed.X += movementSpeedModifier * 0.5f * moveXAxis;
                FlipX = moveXAxis < 0;
                walking = true;
            }

            //play animations according to movement
            if (shotAnimationTimer > 0)
            {
                shotAnimationTimer -= 0.04f;
                FlipX = new Vector2(1, shootDirection).ToCartesian().X < 0;
            }
            if (walking && OnGround)
            {
                AnimationRotation += 8;
                PlayAnimationWalking();
            }
            else
            if (fellThroughTimer > 0)
            {
                AnimationRotation += 4;
                PlayAnimationLegsDuck();
                PlayAnimationArmsLooseDuck();
            }
            else
            if (!OnGround)
            {
                PlayAnimationInAir();
            }
            else
            if (Input.KeyboardCheckDown(Keys.Down))
            {
                AnimationRotation += 4;
                PlayAnimationLegsDuck();
                PlayAnimationArmsGunDuck();
            }
            else
            {
                AnimationRotation += 4;
                PlayAnimationIdle();
            }
            AnimationRotation %= 360;

            //You can still jump a small time after having walked from a platform
            if (OnGround)
                timeSinceOnGround = 0;
            else
                timeSinceOnGround++;

            //You can press jump a small time before landing on a platform and you'll still jump
            if (upPressed)
                timeSinceLastJumpIntention = 0;
            else
                timeSinceLastJumpIntention++;

            //jump
            if (timeSinceLastJumpIntention < maxTimeSinceLastJumpIntention && timeSinceOnGround < maxFromPlatformTimeForJump && Speed.Y >= 0)
            {
                Speed.Y = - jumpSpeed * jumpHeightModifier;
                startedSlowingDownJump = false;

                hasMovedLeft = false;
                hasMovedRight = false;
            }

            if ((Speed.Y < 0 && (!Input.KeyboardCheckDown(Keys.W) && !Input.KeyboardCheckDown(Keys.Up) && Input.ThumbStickCheckDirection(true).Y <= 0.75f && !Input.GamePadCheckDown(Buttons.A))) && (Speed.Y < -3 || startedSlowingDownJump))
            {
                Speed.Y *= 0.9f;
                startedSlowingDownJump = true;
            }
            
            //drop through jumpthroughs
            if (down && OnJumpThrough)
            {
                Position.Y++;
                fellThroughTimer = 1;
            }
            if (fellThroughTimer > 0)
                fellThroughTimer -= 0.05f;

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

            //testing: adds monster
            if (Input.MouseButtonCheckPressed(false))
            {
                World.AddObject(new ShootingMonster(), Input.MouseCheckPosition().ToVector2() + World.Camera);
                Console.WriteLine("Monster Added");
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
            {
                foreach (Monster monster in World.GameObjects.OfType<Monster>())
                    if (TranslatedBoundingBox.Intersects(monster.TranslatedBoundingBox))
                        Hurt(Math.Sign(Position.X - monster.Position.X), monster.Damage);
                foreach (MonsterBullet bullet in World.GameObjects.OfType<MonsterBullet>())
                    if (TranslatedBoundingBox.Intersects(bullet.TranslatedBoundingBox))
                        Hurt(Math.Sign(Position.X - bullet.Position.X), bullet.Damage);
            }

            //And check for scrap
            foreach (Scrap scrap in World.GameObjects.OfType<Scrap>())
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
            if (hasMovedLeft && Speed.Y >= 0)
            {
                //Make sure the player moves down slopes correctly.
                if (GetCollisionWithSolid<SlopeRight>(offsetCollBox) != null)
                {
                    while (GetCollisionWithSolid<SlopeRight>(TranslatedBoundingBox) == null)
                        Position.Y += 1;
                    Position.Y -= 1;
                    OnGround = true;
                }
            }

            offsetCollBox = TranslatedBoundingBox;
            offsetCollBox.Offset(0, 6);

            //If you've moved right...
            if (hasMovedRight && Speed.Y >= 0)
            {
                //Make sure the player moves down slopes correctly.
                if (GetCollisionWithSolid<SlopeLeft>(offsetCollBox) != null)
                {
                    while (GetCollisionWithSolid<SlopeLeft>(TranslatedBoundingBox) == null)
                        Position.Y += 1;
                    Position.Y -= 1;
                    OnGround = true;
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
            Rectangle box = BoundingBox;
            box.Offset(DrawPosition.ToPoint());
            //Drawing.DrawRectangle(box, Color.Red);
            base.Draw();
            //mouse pointer, disabled when controller in use
            Point mousePos = Input.MouseCheckUnscaledPosition(Drawing);
            if (!Input.ControllerInUse)
                Drawing.DrawRectangle(new Rectangle(Input.MouseCheckUnscaledPosition(Drawing).X - 5, Input.MouseCheckUnscaledPosition(Drawing).Y - 5, 10, 10), Color.DarkKhaki);

            //Drawing.DrawSprite(gun, handRight.DrawPosition, 0);
            //Drawing.DrawRectangle(TranslatedBoundingBox, Color.Red);
        }

        void Attack()
        {
            shootDirection = (Input.MouseCheckUnscaledPosition(Drawing).ToVector2() - DrawPosition).Angle();
            shotAnimationTimer = 1;

            switch (CurrentWeapon)
            {
                case Weapon.Nothing:
                    break;
                case Weapon.Gun:
                {
                    Audio.Play("Audio/Combat/Gunshots/Laser/Laser_Shoot01");
                    World.AddObject(new PlayerBullet(), Position);
                    break;
                }
                case Weapon.Rocket:
                {
                        if (RocketAmmo > 0)
                        {
                            Audio.Play("Audio/Combat/Gunshots/Rocket/Rocket_Shoot");
                            World.AddObject(new PlayerRocket(), Position);
                            RocketAmmo --;
                        }
                    break;
                }
            }
        }

        void Hurt(int xDirection, int damage)
        {
            Audio.Play("Audio/Combat/Hit_Hurt");
            HitPoints -= damage;
            Input.GamePadVibrate(0.1f * damage, 0.1f * damage, 0.1f);
            blinkTimer = 1;
            Visible = false;
            Speed = new Vector2(xDirection * 3, -2);
            if (HitPoints <= 0)
                Die();
        }

        void Die()
        {
            Audio.Play("Audio/GameSounds/Game_Over");
            Input.GamePadVibrate(1, 1, 1000);
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
