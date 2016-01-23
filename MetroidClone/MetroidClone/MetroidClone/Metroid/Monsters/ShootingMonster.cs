using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidClone.Engine;
using Microsoft.Xna.Framework.Input;

namespace MetroidClone.Metroid.Monsters
{
    partial class ShootingMonster : Monster
    {
        //distance between player and monster
        float distance = 0;

        //Damage of the monster itself (not used for bullets)
        public int AttackDamage = 5;
        float AnimationRotation = 0;
        float shotAnimationTimer = 0;

        const float jumpSpeed = 8f;
        const float baseSpeed = 0.4f;

        int stateTimer;

        enum JumpType { AlwaysMove, MoveInTheEnd }

        Direction jumpDirection = Direction.None;
        JumpType jumpType;

        int previousXPos = 0;

        AnimationBone body, hipLeft, kneeLeft, hipRight, kneeRight, head,
                    shoulderLeft, shoulderRight, elbowLeft, elbowRight, gun;

        Vector2 startingPos;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-13, -27, 26, 40);
            SpeedOnHit = new Vector2(3, -2);
            HitPoints = 5;
            Damage = 5;

            SpriteScale = 0.2f;

            //animation rig
            body = new AnimationBone(this, new Vector2(0, -8));
            head = new AnimationBone(body, new Vector2(3, -28));
            hipLeft = new AnimationBone(body, new Vector2(6, 0)) { DepthOffset = 2 };
            kneeLeft = new AnimationBone(hipLeft, new Vector2(0, 8)) { DepthOffset = -1 };
            hipRight = new AnimationBone(body, new Vector2(-6, 0)) { DepthOffset = 1 };
            kneeRight = new AnimationBone(hipRight, new Vector2(0, 8)) { DepthOffset = -1 };

            shoulderLeft = new AnimationBone(body, new Vector2(6, -20));
            elbowLeft = new AnimationBone(shoulderLeft, new Vector2(16, 0));
            shoulderRight = new AnimationBone(body, new Vector2(-6, -20));
            elbowRight = new AnimationBone(shoulderRight, new Vector2(-16, 0));
            gun = new AnimationBone(elbowRight, new Vector2(-10, 0)) { DepthOffset = -1 };

            World.AddObject(body);
            body.SetSprite("Enemy1/Body");
            World.AddObject(head);
            head.SetSprite("Enemy1/Head");
            World.AddObject(hipLeft);
            hipLeft.SetSprite("Enemy1/LLeg");
            World.AddObject(kneeLeft);
            kneeLeft.SetSprite("Enemy1/LFoot");
            World.AddObject(hipRight);
            hipRight.SetSprite("Enemy1/RLeg");
            World.AddObject(kneeRight);
            kneeRight.SetSprite("Enemy1/RFoot");

            World.AddObject(shoulderLeft);
            shoulderLeft.SetSprite("Enemy1/LArm1");
            World.AddObject(elbowLeft);
            elbowLeft.SetSprite("Enemy1/LArm2");
            World.AddObject(shoulderRight);
            shoulderRight.SetSprite("Enemy1/RArm1");
            World.AddObject(elbowRight);
            elbowRight.SetSprite("Enemy1/RArm2");
            World.AddObject(gun);
            gun.SetSprite("Enemy1/Enemygun");


            State = MonsterState.ChangeState;
            stateTimer = 60;

            Gravity = 0.2f;

            startingPos = Position;
        }

        public override void Update(GameTime gameTime)
        {
            //Only update if we're inside the view
            if (!World.PointOutOfView(Position))
            {
            base.Update(gameTime);
            AnimationRotation += 4;

            if (Input.KeyboardCheckPressed(Keys.LeftAlt))
            {
                Speed.Y = -5;
            }

            if (!OnGround)
            {
                PlayAnimationLegsInAir();
            }
            else
            if (Input.KeyboardCheckDown(Keys.Space))
            {
                PlayAnimationLegsWalking();
                if (shotAnimationTimer > 0)
                    PlayAnimationArmsShooting((World.Player.Position - Position).Angle());
                else
                    PlayAnimationArmsWalking();
                AnimationRotation += 4;
            }
            else
            {
                PlayAnimationLegsIdle();
                if (shotAnimationTimer > 0)
                    PlayAnimationArmsShooting((World.Player.Position - Position).Angle());
                else
                    PlayAnimationArmsIdle();
            }

            //has shot
            if (shotAnimationTimer > 0)
                shotAnimationTimer -= 0.05f;
            
            //calculates the distance between monster and player. if player is close enough, the monster will attack player
            distance = (Position - World.Player.Position).Length();

            //if the player is in range, the monster will shoot every second. if the player gets to far away, the attack cooldown
            //will decrease, so that the monster will still attack if the player gets in range and out range over and over
            if (distance <= 10 * World.TileWidth)
            {
                if (State == MonsterState.Jumping)
                {
                    //Move left/right if required.
                    if (jumpType == JumpType.AlwaysMove || (jumpType == JumpType.MoveInTheEnd && Speed.Y > -2))
                    {
                            if (jumpDirection == Direction.Left && Position.X > World.Camera.X + 10) //Move left as long as we're not on near the edge
                            Speed.X -= baseSpeed;
                            else if (jumpDirection == Direction.Right && Position.X < World.Camera.X + (World.TileWidth * WorldGenerator.LevelWidth) - 10) //Move right as long as we're not on near the edge
                            Speed.X += baseSpeed;
                    }

                    //When we landed again.
                    if (OnGround)
                    {
                        stateTimer = 60;
                        State = MonsterState.ChangeState;
                    }
                }
                else
                {
                    //If we're attacking...
                    if (State == MonsterState.Attacking)
                    {
                        if (stateTimer == 40 || stateTimer == 50 || stateTimer == 60)
                        {
                            //Evaluate if we should still attack.
                            if (CanReachPlayer())
                                Attack();
                            else
                            {
                                State = MonsterState.Moving;
                            }
                        }
                    }
                        //If we need to do something else (which is moving (in some way))
                    else if (State != MonsterState.ChangeState)
                    {
                        Direction preferDir = Direction.None;

                        //Check in which direction we want to move
                        if (State == MonsterState.PatrollingLeft)
                            preferDir = Direction.Left;
                        else if (State == MonsterState.PatrollingRight)
                            preferDir = Direction.Right;
                        else
                        {
                            if (World.Player.Position.X < Position.X - 20)
                                preferDir = Direction.Left;
                            else if (World.Player.Position.X > Position.X + 20)
                                preferDir = Direction.Right;
                        }
                        //Move if needed

                        //Left
                            if (preferDir == Direction.Left && Position.X > World.Camera.X + 10) //Move left as long as we're not near the edge
                            Speed.X -= baseSpeed;    
                        //Right
                            else if (preferDir == Direction.Right && Position.X < World.Camera.X + (World.TileWidth * WorldGenerator.LevelWidth) - 10) //Move right as long as we're not near the edge
                            Speed.X += baseSpeed;

                        //Jump
                        if ((World.Player.Position.Y < Position.Y - 20 || HadHCollision) && OnGround && World.Random.Next(10) == 1)
                        {
                            Speed.Y = -jumpSpeed;
                            State = MonsterState.Jumping;
                            jumpDirection = preferDir;

                            if (World.Random.Next(2) == 1)
                                jumpType = JumpType.AlwaysMove;
                            else
                                jumpType = JumpType.MoveInTheEnd;
                        }
                    }

                    if (stateTimer >= 60)
                    {
                        stateTimer = 0;
                        //If we can reach the player, shoot.
                        if (CanReachPlayer())
                            State = MonsterState.Attacking;
                        //else, try moving in the general direction of the player.
                        else
                        {
                            //Move towards the player in most situations
                            if (Math.Abs(Position.X - World.Player.Position.X) > 20 && previousXPos != Math.Round(Position.X))
                                State = MonsterState.Moving;
                            else //Move randomly if moving towards towards the player would be useless.
                            {
                                if (World.Random.Next(2) == 1 && !InsideWall(-3, 0, TranslatedBoundingBox))
                                    State = MonsterState.PatrollingLeft;
                                else
                                    State = MonsterState.PatrollingRight;

                                    stateTimer = -80 - World.Random.Next(50); //Give it some more time to do its thing.
                            }
                                previousXPos = (int)Math.Round(Position.X);
                        }
                    }
                    else
                        stateTimer++;
                }
            }
            else
            {
                stateTimer = 0;
                }
            }
            else
            {
                stateTimer = 0;
                if (World.PointOutOfView(Position, -50)) //If the position is very near the view edge, reset it.
                    Position = startingPos;
            }
        }

        void Attack()
        {
            shotAnimationTimer = 1;
            FlipX = (Position.X - World.Player.Position.X) > 0;
            World.AddObject(new MonsterBullet(AttackDamage), Position);
        }

        public override void Draw()
        {
            base.Draw();
            //Drawing.DrawRectangle(DrawBoundingBox, Color.Purple);
        }
    }
}
