using MetroidClone.Engine;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;
using MetroidClone.Metroid.Monsters;

namespace MetroidClone.Metroid
{
    abstract class Monster : PhysicsObject
    {
        //Basic properties
        public float HitPoints = 1;
        public int Damage = 10;
        protected Vector2 SpeedOnHit = Vector2.Zero;
        const float jumpSpeed = 8f;

        protected float BaseSpeed = 0.4f;

        //Basic AI variables
        protected MonsterState State;
        protected enum MonsterState { Moving, Attacking, Jumping, PatrollingLeft, PatrollingRight, ChangeState }
        protected int StateTimer;
        enum JumpType { AlwaysMove, MoveInTheEnd }
        Direction jumpDirection = Direction.None;
        JumpType jumpType;
        int previousXPos = 0;

        //For the boss room
        public bool HasUnlimitedLookingDistance = false;

        public Monster()
        {
            Depth = -1;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //check collision player attacks
            foreach (IPlayerAttack attackInterface in World.GameObjects.OfType<IPlayerAttack>().ToList())
            {
                PhysicsObject attack = attackInterface as PhysicsObject;
                if (TranslatedBoundingBox.Intersects(attack.TranslatedBoundingBox))
                {
                    Hurt(Math.Sign(Position.X - attack.Position.X), true, attackInterface.Damage);
                    attack.Destroy();
                }
            }
        }

        void Hurt(int xDirection, bool hitByPlayer, float damage = 1f)
        {
            // if an attack from the player hits
            HitPoints -= damage;
            if (SpeedOnHit != Vector2.Zero)
                Speed = new Vector2(xDirection * SpeedOnHit.X, SpeedOnHit.Y);
            if (HitPoints <= 0)
            {
                Destroy();
                if (hitByPlayer)
                {
                    World.Player.Score += 20;
                    Console.Write("Score: ");
                    Console.WriteLine(World.Player.Score);
                }
            }
        }

        public override void Destroy()
        {
            World.Tutorial.MonsterKilled = true;
            // When a monster is destroyed, you have a chance that a healthpack, rocket ammo, scrap metal or nothing will drop
            // The chance of dropping healthpacks and rocket ammo increases if the player is in need of these drops
            float ammoChance = (1 - ((float)World.Player.RocketAmmo / (float)World.Player.MaximumRocketAmmo)) * 40;
            float scrapChance = (this is Turret) ? 60 : 40; //Turrets drop more scrap
            float healthChance = (1 - ((float)World.Player.HitPoints / (float)World.Player.MaxHitPoints)) * 40;
            float randomLoot = World.Random.Next(101);
            if (randomLoot <= healthChance)
                World.AddObject(new HealthDrop(Damage * 2), Position);
            else if (randomLoot <= ammoChance + healthChance)
                World.AddObject(new RocketAmmo(), Position);
            else if (randomLoot <= scrapChance + ammoChance + healthChance)
                World.AddObject(new Scrap(), Position);
            base.Destroy();
        }

        //Check if a bullet could reach the player
        protected bool CanReachPlayer()
        {
            Vector2 emulatedBulletPos = Position;
            Vector2 dir = World.Player.Position - Position;
            dir.Normalize();

            while ((emulatedBulletPos - World.Player.Position).Length() > 8)
            {
                emulatedBulletPos += MonsterBullet.baseSpeed * dir;

                if (InsideWall(new Rectangle((int) emulatedBulletPos.X - 4, (int) emulatedBulletPos.Y - 4, 8, 8)))
            {
                    return false;
            }
            }

            return true;
        }

        protected void DoBasicAI(bool canShoot = true)
        {
            //calculates the distance between monster and player. if player is close enough, the monster will move / attack the player
            float distance = (Position - World.Player.Position).Length();

            //if the player is quite near, move and/or attack the player.
            if (distance <= 10 * World.TileWidth || HasUnlimitedLookingDistance)
            {
                if (State == MonsterState.Jumping)
                {
                    //Move left/right if required.
                    if (jumpType == JumpType.AlwaysMove || (jumpType == JumpType.MoveInTheEnd && Speed.Y > -2))
                    {
                        if (jumpDirection == Direction.Left && Position.X > World.Camera.X + 10) //Move left as long as we're not on near the edge
                            Speed.X -= BaseSpeed;
                        else if (jumpDirection == Direction.Right && Position.X < World.Camera.X + (World.TileWidth * WorldGenerator.LevelWidth) - 10) //Move right as long as we're not on near the edge
                            Speed.X += BaseSpeed;
                    }

                    //When we landed again.
                    if (OnGround)
                    {
                        StateTimer = 60;
                        State = MonsterState.ChangeState;
                    }
                    else
                    {
                        //Shoot if this is taking too long.
                        if (canShoot && CanReachPlayer() && (StateTimer >= 40 && StateTimer % 10 == 0))
                            Attack();

                        StateTimer++;
                    }
                }
                else
                {
                    //If we're attacking...
                    if (State == MonsterState.Attacking)
                    {
                        if (StateTimer == 40 || StateTimer == 50 || StateTimer == 60)
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
                        //set flipX according to preferdir
                        if (preferDir != Direction.None)
                            FlipX = preferDir == Direction.Left;

                        //Move if needed

                        //Left
                        if (preferDir == Direction.Left && Position.X > World.Camera.X + 10) //Move left as long as we're not near the edge
                            Speed.X -= BaseSpeed;
                        //Right
                        else if (preferDir == Direction.Right && Position.X < World.Camera.X + (World.TileWidth * WorldGenerator.LevelWidth) - 10) //Move right as long as we're not near the edge
                            Speed.X += BaseSpeed;

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

                    if (StateTimer >= 60)
                    {
                        StateTimer = 0;
                        //If we can reach the player, shoot.
                        if (canShoot && CanReachPlayer())
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

                                StateTimer = -80 - World.Random.Next(50); //Give it some more time to do its thing.
                            }
                            previousXPos = (int)Math.Round(Position.X);
                        }
                    }
                    else
                        StateTimer++;
                }
            }
            else
            {
                StateTimer = 0;
            }
        }

        protected virtual void Attack()
        {
            //Do nothing by default
        }
    }
}
