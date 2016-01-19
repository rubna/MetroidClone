using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidClone.Engine;


namespace MetroidClone.Metroid
{
    class ShootingMonster : Monster
    {
        float distance = 0;
        public int AttackDamage = 5;

        const float jumpSpeed = 8f;
        const float baseSpeed = 0.4f;

        int stateTimer;

        enum JumpType { AlwaysMove, MoveInTheEnd }

        Direction jumpDirection = Direction.None;
        JumpType jumpType;

        int previousXPos = 0;

        public override void Create()
        {
            base.Create();
            BoundingBox = new Rectangle(-10, -10, 20, 20);
            SpeedOnHit = new Vector2(3, -2);
            HitPoints = 10;
            Damage = 5;
            ScoreOnKill = 20;

            State = MonsterState.ChangeState;
            stateTimer = 60;

            Gravity = 0.2f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            //calculates the distance between monster and player. if player is close enough, the monster will attack player
            distance = (Position - World.Player.Position).Length();
            if (distance <= 10 * World.TileWidth)
            {
                if (State == MonsterState.Jumping)
                {
                    //Move left/right if required.
                    if (jumpType == JumpType.AlwaysMove || (jumpType == JumpType.MoveInTheEnd && Speed.Y > -2))
                    {
                        if (jumpDirection == Direction.Left)
                            Speed.X -= baseSpeed;
                        else if (jumpDirection == Direction.Right)
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
                    //If we need to do something else (which is moving)
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
                        if (preferDir == Direction.Left)
                            Speed.X -= baseSpeed;    
                        //Right
                        else if (preferDir == Direction.Right)
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

                                stateTimer = -80 - World.Random.Next(50); //Give it some more time.
                            }
                            previousXPos = (int) Math.Round(Position.X);
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

        void Attack()
        {
            World.AddObject(new MonsterBullet(AttackDamage), Position);
        }

        public override void Draw()
        {
            base.Draw();
            Drawing.DrawRectangle(DrawBoundingBox, Color.Purple);
        }
    }
}
