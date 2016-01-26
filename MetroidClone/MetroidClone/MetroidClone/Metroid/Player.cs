using MetroidClone.Engine;
using MetroidClone.Engine.Asset;
using MetroidClone.Metroid.Monsters;
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
        public int CollectedScrap = 0;
        int timeSinceOnGround = 0;
        const int maxFromPlatformTimeForJump = 5; //The maximum time you can still jump after having moved from a platform.
        float attackTimer = 0;

        int timeSinceLastJumpIntention = 10;
        const int maxTimeSinceLastJumpIntention = 5; //The maximum time you can press the jump button before landing on a platform.

        public int TimeSinceHWallCollision, TimeSinceVWallCollision;

        bool startedSlowingDownJump; //This is used to make sure that the player will jump the maximum height if releasing the jump button slightly before reaching it.

        public bool Dead = false;

        float moveXAxis = 0;
        bool up = false;
        bool upPressed = false;
        bool down = false;

        public Weapon CurrentWeapon = Weapon.Nothing;
        public List<Weapon> UnlockedWeapons = new List<Weapon>() { Weapon.Nothing };
        public int HitPoints = 100, MaxHitPoints = 100;
        public int RocketAmmo = 5;
        public int MaximumRocketAmmo = 5;
        public int Score = 0;
        public int Timer = 0;

        public float Rotation = 0;
        public float AnimationRotation = 0;
        AnimationBone body, hipLeft, kneeLeft, footLeft, hipRight, kneeRight, footRight, head, 
                    shoulderLeft, shoulderRight, elbowLeft, elbowRight, handLeft, handRight, 
                    gun, launcher, wrench, gunNuzzle, launcherNuzzle,
                    antennaLeft1, antennaLeft2, antennaRight1, antennaRight2;

        float shotAnimationTimer = 0;
        float meleeAnimationTimer = 0;
        float shootDirection = 0;
        float fellThroughTimer = 0;

        public float movementSpeedModifier; //This can be used to change the movement speed.
        public float jumpHeightModifier; //This can be used to change the jump height.

        const float jumpSpeed = 8f; //The base jumping speed. Was: 10f
        const float gravity = 0.2f; //The base gravity. Was: 0.3f

        //Whether the player has the gun upgrade.
        public bool HasGunUpgrade
        {
            get { return hasGunUpgrade; }
            set
            {
                hasGunUpgrade = value;
                if (hasGunUpgrade)
                    gun.SetSprite("Items/gunUpgraded");
                else
                    gun.SetSprite("Items/gun");
            }
        }
        public bool hasGunUpgrade = false;
        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-10, -16, 20, 30);
            Depth = -10;
            SpriteScale = 0.07f;
            //make skeleton
            body = new AnimationBone(this, new Vector2(0, -1));
            head = new AnimationBone(body, new Vector2(0, -16));
            hipLeft = new AnimationBone(body, new Vector2(-5, -1)) { DepthOffset = 1 };
            hipRight = new AnimationBone(body, new Vector2(5, -1)) { DepthOffset = 1 };
            kneeLeft = new AnimationBone(hipLeft, new Vector2(0, 7)) { DepthOffset = 1 };
            kneeRight = new AnimationBone(hipRight, new Vector2(0, 7)) { DepthOffset = 1 };
            footLeft = new AnimationBone(kneeLeft, new Vector2(0, 7));
            footRight = new AnimationBone(kneeRight, new Vector2(0, 7));

            shoulderRight = new AnimationBone(body, new Vector2(-5.5f, -13f)) { DepthOffset = -3 };
            shoulderLeft = new AnimationBone(body, new Vector2(5.5f, -13)) { DepthOffset = 1 };
            elbowRight = new AnimationBone(shoulderRight, new Vector2(-8, 0));
            elbowLeft = new AnimationBone(shoulderLeft, new Vector2(8, 0)) { DepthOffset = 1 };
            handRight = new AnimationBone(elbowRight, new Vector2(-7, 0)) { DepthOffset = 1 };
            handLeft = new AnimationBone(elbowLeft, new Vector2(7, 0)) { DepthOffset = 1 };

            gun = new AnimationBone(handRight, new Vector2(-2, 0)) { DepthOffset = -4 };
            gunNuzzle = new AnimationBone(gun, new Vector2(20, -4));
            launcher = new AnimationBone(handRight, new Vector2(-2, 0)) { DepthOffset = -4 };
            launcherNuzzle = new AnimationBone(launcher, new Vector2(20, -12));
            wrench = new AnimationBone(handLeft, new Vector2(2, 0)) { DepthOffset = 1 };

            antennaLeft1 = new AnimationBone(head, new Vector2(3, -18)) { DepthOffset = -1 };
            antennaLeft2 = new AnimationBone(antennaLeft1, new Vector2(0, -6)) { DepthOffset = 1 };
            antennaRight1 = new AnimationBone(head, new Vector2(-3, -17)) { DepthOffset = -1 };
            antennaRight2 = new AnimationBone(antennaRight1, new Vector2(0, -6)) { DepthOffset = 1 };

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
            World.AddObject(gunNuzzle);

            World.AddObject(launcher);
            launcher.SetSprite("Items/playerrocket");
            launcher.SpriteScale = 0.2f;
            World.AddObject(launcherNuzzle);

            World.AddObject(wrench);
            wrench.SetSprite("Items/wrench");
            wrench.SpriteScale = 0.2f;
            wrench.TargetRotation = -90f;

            World.AddObject(antennaLeft1);
            antennaLeft1.SetSprite("Robot/RobotSpriteLAntennae1");
            antennaLeft1.TargetRotation = 5;
            World.AddObject(antennaLeft2);
            antennaLeft2.SetSprite("Robot/RobotSpriteLAntennae2");

            World.AddObject(antennaRight1);
            antennaRight1.SetSprite("Robot/RobotSpriteRAntennae1");
            antennaRight1.TargetRotation = 10;
            World.AddObject(antennaRight2);
            antennaRight2.SetSprite("Robot/RobotSpriteRAntennae2");
            antennaRight2.TargetRotation = 40;

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
                World.Tutorial.Moved = true;
                Speed.X += movementSpeedModifier * 0.5f * moveXAxis;
                FlipX = moveXAxis < 0;
                walking = true;
            }

            gun.Visible = CurrentWeapon == Weapon.Gun && meleeAnimationTimer <= 0;
            launcher.Visible = CurrentWeapon == Weapon.Rocket && meleeAnimationTimer <= 0;
            wrench.Visible = meleeAnimationTimer > 0;

            //play animations according to movement
            if (shotAnimationTimer > 0)
            {
                shotAnimationTimer -= 0.04f;
                FlipX = new Vector2(1, shootDirection).ToCartesian().X < 0;
            }
            if (walking && OnGround)
                PlayAnimationWalking();
            else
            if (fellThroughTimer > 0)
                PlayAnimationDuck();
            else
            if (!OnGround)
                PlayAnimationInAir();
            else
            if (down)
                PlayAnimationDuck();
            else
                PlayAnimationIdle();
            AnimationRotation %= 360;

            //You can still jump a small time after having walked from a platform
            if (OnGround)
                timeSinceOnGround = 0;
            else
                timeSinceOnGround++;

            //You can press jump a small time before landing on a platform and you'll still jump
            if (upPressed)
            {
                timeSinceLastJumpIntention = 0;
                if (World.Tutorial.Moved)
                    World.Tutorial.Jumped = true;
            }
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
                fellThroughTimer -= 0.075f;

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
                                World.Tutorial.GunShot = true;
                                attackTimer = 0.11f;
                                break;
                            }
                        case (int)Weapon.Rocket:
                            {
                                World.Tutorial.RocketShot = true;
                                attackTimer = 0.22f;
                                break;
                            }
                    }
                }
            }

            if (UnlockedWeapons.Contains(Weapon.Wrench))
            {
                //hold up wrench!
                if ((Input.MouseButtonCheckDown(false) || Input.MouseWheelPressed() || Input.GamePadCheckPressed(Buttons.B)) && attackTimer == 0)
                {
                    meleeAnimationTimer = 1;
                    FlipX = Input.MouseCheckUnscaledPosition(Drawing).X < DrawPosition.X;
                }
                else
                if (meleeAnimationTimer > 0)
                    meleeAnimationTimer -= 0.05f;

                //melee attack
                if ((Input.MouseButtonCheckReleased(false) || Input.MouseWheelPressed() || Input.GamePadCheckPressed(Buttons.B))
                     && attackTimer == 0)
                {
                    World.Tutorial.WrenchUsed = true;
                    World.AddObject(new PlayerMelee(), Position + GetFlip * Vector2.UnitX * 20);
                    attackTimer = 0.2f;
                }
            }

            /*//testing: adds monster
            if (Input.KeyboardCheckPressed(Keys.F))
            {
                World.AddObject(new Turret(), Input.MouseCheckUnscaledPosition(Drawing).ToVector2() + World.Camera);
                Console.WriteLine("Monster Added");
            }*/

            //switch weapons
            if (Input.KeyboardCheckPressed(Keys.Q) || Input.MouseWheelCheckScroll(true) || Input.MouseWheelCheckScroll(false) || Input.GamePadCheckPressed(Buttons.Y))
            {
                NextWeapon();
                Console.WriteLine(CurrentWeapon);
            }

            if (Input.KeyboardCheckPressed(Keys.E) || Input.GamePadCheckPressed(Buttons.X))
            {
                CreateDrone();
                World.Tutorial.DroneBuild = true;
            }
            Timer++;

            base.Update(gameTime);

            //check for getting hurt
            if (blinkTimer == 0)
            {
                foreach (Monster monster in World.GameObjects.OfType<Monster>())
                    if (TranslatedBoundingBox.Intersects(monster.TranslatedBoundingBox))
                    {
                        if (monster.Damage > 0)
                        Hurt(Math.Sign(Position.X - monster.Position.X), monster.Damage);
                    }

                foreach (Spikes spikes in World.GameObjects.OfType<Spikes>())
                    if (TranslatedBoundingBox.Intersects(spikes.TranslatedBoundingBox))
                    {
                        if (spikes.Damage > 0)
                            Hurt(Math.Sign(Position.X - spikes.Position.X), spikes.Damage);
                    }

                List<MonsterBullet> destroyedBullets = new List<MonsterBullet>();
                foreach (MonsterBullet bullet in World.GameObjects.OfType<MonsterBullet>())
                {
                    if (TranslatedBoundingBox.Intersects(bullet.TranslatedBoundingBox))
                    {
                        if (bullet.Damage > 0)
                        Hurt(Math.Sign(Position.X - bullet.Position.X), bullet.Damage);
                        destroyedBullets.Add(bullet);
                    }
                }

                for (int i = 0; i < destroyedBullets.Count; i++)
                {
                    destroyedBullets[i].Destroy();
                }
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

            //DEBUG TODO REMOVE
            if (Input.KeyboardCheckPressed(Keys.T))
                Position = Input.MouseCheckUnscaledPosition(Drawing).ToVector2() + World.Camera;
        }

        void CreateDrone()
        {
            if (CollectedScrap < 25)
                return;
            World.AddObject(new Drone(), Position);
            CollectedScrap -= 25;
            Score += 10;
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
                    PlayerBullet bullet = new PlayerBullet();
                    World.AddObject(bullet, gunNuzzle.Position);
                    if (Input.ControllerInUse)
                    {
                        Vector2 dir = Input.ThumbStickCheckDirection(false);
                        dir.Y = -dir.Y;
                        dir.Normalize();
                        bullet.Speed = 5 * dir;
                    }
                    else
                    {
                        bullet.Speed = Input.MouseCheckUnscaledPosition(Drawing).ToVector2() - DrawPosition;
                        bullet.Speed.Normalize();
                        bullet.Speed *= 5;
                    }
                    break;
                }
                case Weapon.Rocket:
                {
                    if (RocketAmmo > 0)
                    {
                        Audio.Play("Audio/Combat/Gunshots/Rocket/Rocket_Shoot");
                        World.AddObject(new PlayerRocket(), launcherNuzzle.Position);
                        RocketAmmo --;
                    }
                    break;
                }
            }
        }

        //if the player gets hit by a monster or bullet, the controller will vibrate, a soundeffect will be played,
        //the player will invincible for a short time and the player will get a slit knockback. if the hitpoints drop below 0,
        //the player will die.
        void Hurt(int xDirection, int damage)
        {
            Audio.Play("Audio/Combat/Hit_Hurt");
            HitPoints -= damage;
            Input.GamePadVibrate(0.1f * damage, 0.1f * damage, 100);
            blinkTimer = 1;
            Visible = false;
            Speed = new Vector2(xDirection * 3, -2);
            if (HitPoints <= 0)
                Die();
        }

        //sends a signal to MainGame, so the GameState can change to GameOver
        void Die()
        {
            Audio.Play("Audio/GameSounds/Game_Over");
            Input.GamePadVibrate(1, 1, 1000);
            Dead = true;
        }

        void NextWeapon()
        {
            if (CurrentWeapon == Weapon.Gun && UnlockedWeapons.Contains(Weapon.Rocket))
                CurrentWeapon = Weapon.Rocket;
            else if (CurrentWeapon == Weapon.Rocket && UnlockedWeapons.Contains(Weapon.Gun))
            {
                CurrentWeapon = Weapon.Gun;
                World.Tutorial.WeaponSwitched = true;
            }
        }
    }
    public enum Weapon
    {
        Nothing, Gun, Wrench, Rocket
    }
}
