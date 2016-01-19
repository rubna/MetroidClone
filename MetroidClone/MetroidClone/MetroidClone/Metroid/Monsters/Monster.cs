using MetroidClone.Engine;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MetroidClone.Metroid.Abstract;

namespace MetroidClone.Metroid
{
    abstract class Monster : PhysicsObject
    {
        protected MonsterState State;
        protected enum MonsterState { Moving, Attacking, Jumping, PatrollingLeft, PatrollingRight, ChangeState }

        public int HitPoints = 1;
        public int Damage = 10;
        protected int ScoreOnKill = 1;
        protected Vector2 SpeedOnHit = Vector2.Zero;

        int randomLoot;

        public Monster()
        {
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
                    attack.Destroy();
                    Hurt(Math.Sign(Position.X - attack.Position.X), true);
                }
            }
        }

        void Hurt(int xDirection, bool hitByPlayer)
        {
            HitPoints--;
            if (SpeedOnHit != Vector2.Zero)
                Speed = new Vector2(xDirection * SpeedOnHit.X, SpeedOnHit.Y);
            if (HitPoints <= 0)
            {
                Destroy();
                if (hitByPlayer)
                {
                    World.Player.Score += ScoreOnKill;
                    Console.Write("Score: ");
                    Console.WriteLine(World.Player.Score);
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            // When a monster is destroyed, you have a chance that a rocket or a health pack will drop
            if (World.Player.UnlockedWeapons.Contains(Weapon.Rocket))
            {
                randomLoot = World.Random.Next(100);
                if (randomLoot < 5)
                    World.AddObject(new RocketAmmo(), Position.X, Position.Y);
                if (randomLoot > 4 && randomLoot < 10)
                    World.AddObject(new HealthDrop(), Position.X, Position.Y);
            }
            else
            {
                randomLoot = World.Random.Next(100);
                if (randomLoot < 5)
                    World.AddObject(new HealthDrop(), Position.X, Position.Y);
            }
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
    }
}
